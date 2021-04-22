using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using TriviaGame.Data.Models;
using TriviaGame.Data.Services.Interfaces;
using TriviaGame.Data.Utilities;

namespace TriviaGame.Data.Services.Impl
{
    public class GameBoardHub : Hub<IGameBoardHub>
    {
        private readonly ITriviaService _triviaService;

        // Maps connection_id to group_id (gameId)
        private static readonly ConcurrentDictionary<string, string> ConnectionIdMapping = new();
        // Maps group_id to session information
        private static readonly ConcurrentDictionary<string, GameSessionInfo> SessionInfo = new();

        public GameBoardHub(ITriviaService triviaService)
        {
            _triviaService = triviaService ?? throw new ArgumentNullException(nameof(triviaService));
        }

        private async Task<GameSessionInfo> CreateOrAddToGameGroup(string gameId)
        {
            var connId = Context.ConnectionId;

            var player = new Player
            {
                Team = Team.Unknown,
                ConnectionId = connId
            };
            var gameSessionInfo = new GameSessionInfo(gameId, player);

            // This is a new game
            if (!SessionInfo.TryGetValue(gameId, out _))
            {
                var (board, totalAnswers) = _triviaService.GetRandomTriviaBoardWithNoAnswers();
                gameSessionInfo.TriviaBoard = board;
                gameSessionInfo.TotalAnswers = totalAnswers;
            }

            await Groups.AddToGroupAsync(connId, gameId, CancellationToken.None);

            // Update connectionId mapping to new gameId
            ConnectionIdMapping.AddOrUpdate(connId, gameId, (_, _) => gameId);

            SessionInfo.AddOrUpdate(gameId, gameSessionInfo, (_, info) =>
            {
                if (!info.PlayerIds.ContainsKey(connId))
                {
                    info.PlayerIds.Add(connId, player);
                }

                return info;
            });

            gameSessionInfo = SessionInfo[gameId];
            await Clients.Group(gameId).ConfirmPlayerAdded(player);

            return gameSessionInfo;
        }

        private async Task<GameSessionInfo> RemoveUserFromGroup()
        {
            var connId = Context.ConnectionId;

            if (!ConnectionIdMapping.TryGetValue(connId, out var gameId)) return null;

            if (!SessionInfo.TryGetValue(gameId, out var curSessionInfo)) return null;

            if (curSessionInfo.PlayerIds.TryGetValue(connId, out var player))
            {
                curSessionInfo.PlayerIds.Remove(connId);
                ConnectionIdMapping.Remove(connId, out _);

                await Clients.Group(gameId).ConfirmPlayerRemoved(player);
            }

            // If game session contains no players, close
            if (!curSessionInfo.PlayerIds.Any())
            {
                SessionInfo.Remove(gameId, out _);
            }

            await Groups.RemoveFromGroupAsync(connId, curSessionInfo.GameId);

            return curSessionInfo;
        }

        private bool TryGetCurrentGameInfo(out GameSessionInfo gameSessionInfo)
        {
            gameSessionInfo = null;
            return TryGetCurrentGameId(out var gameId) && SessionInfo.TryGetValue(gameId, out gameSessionInfo);
        }

        private bool TryGetCurrentPlayer(out Player player, GameSessionInfo gameSessionInfo = null)
        {
            player = null;
            if (gameSessionInfo is null && !TryGetCurrentGameInfo(out gameSessionInfo)) return false;

            player = gameSessionInfo.Players.SingleOrDefault(x => x.ConnectionId == Context.ConnectionId);

            return true;
        }

        private bool TryGetCurrentGameId(out string gameId)
        {
            return ConnectionIdMapping.TryGetValue(Context.ConnectionId, out gameId);
        }

        public async Task CreateGame()
        {
            var gameSessionInfo = await CreateOrAddToGameGroup(Guid.NewGuid().ToString());

            await Clients.Caller.ReceiveGameInformation(gameSessionInfo);
        }

        public async Task JoinGame(string gameId)
        {
            var gameSessionInfo = await CreateOrAddToGameGroup(gameId);

            await Clients.Caller.ReceiveGameInformation(gameSessionInfo);
        }

        public async Task<GameSessionInfo> LeaveGame()
        {
            await LeaveHost();

            return await RemoveUserFromGroup();
        }

        public async Task PlayerUpdated(Player player)
        {
            // If our connectionId doesn't match the one in the player object - stop
            if (player.ConnectionId != Context.ConnectionId) return;
            if (!TryGetCurrentGameInfo(out var sessionInfo)) return;

            var connId = Context.ConnectionId;
            if (sessionInfo.PlayerIds.ContainsKey(connId))
            {
                sessionInfo.PlayerIds.Remove(connId);
                sessionInfo.PlayerIds.Add(connId, player);
            }

            if (!SessionInfo.TryUpdate(sessionInfo.GameId, sessionInfo, sessionInfo)) return;

            await Clients.Group(sessionInfo.GameId).PlayerTeamUpdated(player);
        }

        public async Task LeaveHost()
        {
            if (!TryGetCurrentGameId(out var gameId)) return;
            if (!TryGetCurrentGameInfo(out var gameSessionInfo)) return;

            // Only continue if current player is host
            if (gameSessionInfo.Host is null || gameSessionInfo.Host.ConnectionId != Context.ConnectionId) return;

            gameSessionInfo.Host = null;
            if (!SessionInfo.TryUpdate(gameId, gameSessionInfo, gameSessionInfo)) return;

            await Clients.Group(gameId).HostChanged(null);
            await Clients.Caller.ReceiveGameInformation(gameSessionInfo);
        }

        public async Task HostGame()
        {
            if (!TryGetCurrentGameId(out var gameId)) return;
            if (!TryGetCurrentGameInfo(out var gameSessionInfo)) return;

            if (gameSessionInfo.Host is not null) return;

            if (!gameSessionInfo.PlayerIds.TryGetValue(Context.ConnectionId, out var player)) return;

            gameSessionInfo.Host = player;
            SessionInfo.TryUpdate(gameId, gameSessionInfo, gameSessionInfo);

            var sessionToSendToHost = gameSessionInfo.Copy();
            await Clients.Group(gameId).HostChanged(player);

            var board = _triviaService.GetTriviaBoardOfId(gameSessionInfo.TriviaBoard.TriviaBoardId);

            var answers = board.Answers.Select(x =>
            {
                var guessedTriviaAnswer = gameSessionInfo.TriviaBoard.Answers
                    .SingleOrDefault(xi => xi.TriviaAnswerId == x.TriviaAnswerId);
                return guessedTriviaAnswer ?? x;
            });

            // TODO: Don't blindly replace all the answers here, only the ones that aren't already known
            sessionToSendToHost.TriviaBoard.Answers = answers.ToList();
            sessionToSendToHost.TriviaBoard.Question = board.Question;

            await Clients.Caller.ReceiveGameInformation(sessionToSendToHost);
        }

        public async Task AwardAnswerToTeam(TriviaAnswer answer)
        {
            if (!TryGetCurrentGameInfo(out var gameSessionInfo)) return;

            // Make sure caller is host
            if (gameSessionInfo.Host.ConnectionId != Context.ConnectionId) return;

            var existingAnswers = gameSessionInfo.TriviaBoard.Answers
                .Select((x, i) => (x, i))
                .ToDictionary(x => x.x.TriviaAnswerId, g => g.i);

            if (existingAnswers.TryGetValue(answer.TriviaAnswerId, out var existingIndex))
            {
                // Only updating the WonBy for now - can probably just set the entire TriviaAnswer here
                gameSessionInfo.TriviaBoard.Answers[existingIndex].WonBy = answer.WonBy;
            }
            else
            {
                gameSessionInfo.TriviaBoard.Answers.Add(answer);
            }

            SessionInfo.TryUpdate(gameSessionInfo.GameId, gameSessionInfo, gameSessionInfo);

            await Clients.Group(gameSessionInfo.GameId).TriviaAnswersRevealed(answer);
        }

        public async Task ChangeBuzzerState(bool newState)
        {
            if (!TryGetCurrentGameInfo(out var gameSessionInfo)) return;

            // Make sure caller is host
            if (gameSessionInfo.Host.ConnectionId != Context.ConnectionId) return;

            gameSessionInfo.BuzzersEnabled = newState;

            SessionInfo.TryUpdate(gameSessionInfo.GameId, gameSessionInfo, gameSessionInfo);

            await Clients.Group(gameSessionInfo.GameId).BuzzerStateChanged(newState);
        }

        public async Task BuzzerPressed()
        {
            if (!TryGetCurrentGameInfo(out var gameSessionInfo)) return;

            if (!gameSessionInfo.BuzzersEnabled) return;

            if (!TryGetCurrentPlayer(out var player, gameSessionInfo)) return;

            if (player.BuzzerPosition > 0) return;

            var lastBuzzerPosition = gameSessionInfo.Players.Max(x => x.BuzzerPosition);
            player.BuzzerPosition = lastBuzzerPosition + 1;

            SessionInfo.TryUpdate(gameSessionInfo.GameId, gameSessionInfo, gameSessionInfo);

            await Clients.Group(gameSessionInfo.GameId)
                .ConfirmBuzzerPressReceived(player.ConnectionId, player.BuzzerPosition);
        }

        public async Task ClearOldBuzzerPositions()
        {
            if (!TryGetCurrentGameInfo(out var gameSessionInfo)) return;

            if (gameSessionInfo.Host?.ConnectionId != Context.ConnectionId) return;

            foreach (var player in gameSessionInfo.Players)
            {
                player.BuzzerPosition = 0;
            }

            SessionInfo.TryUpdate(gameSessionInfo.GameId, gameSessionInfo, gameSessionInfo);

            await Clients.Group(gameSessionInfo.GameId).ClearOldBuzzerPositions();
        }

        public async Task RevealGameQuestion()
        {
            if (!TryGetCurrentGameInfo(out var gameSessionInfo)) return;

            // Make sure caller is host
            if (gameSessionInfo.Host.ConnectionId != Context.ConnectionId) return;

            var board = _triviaService.GetTriviaBoardOfId(gameSessionInfo.TriviaBoard.TriviaBoardId);

            gameSessionInfo.TriviaBoard.Question = board.Question;

            SessionInfo.TryUpdate(gameSessionInfo.GameId, gameSessionInfo, gameSessionInfo);

            await Clients.Group(gameSessionInfo.GameId).ReceiveGameInformation(gameSessionInfo);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Player player = null;
            if (TryGetCurrentGameInfo(out var session))
            {
                session.PlayerIds.TryGetValue(Context.ConnectionId, out player);
            }

            var sessionInfo = await LeaveGame();

            if (sessionInfo is not null && player is not null)
            {
                await Clients.Group(sessionInfo.GameId).ConfirmPlayerRemoved(player);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
