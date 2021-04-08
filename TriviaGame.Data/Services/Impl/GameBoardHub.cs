using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using TriviaGame.Data.Services.Interfaces;

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

            var gameSessionInfo = new GameSessionInfo(gameId, connId);

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
                if (!info.PlayerIds.Contains(connId))
                {
                    info.PlayerIds.Add(connId);
                }
            
                return info;
            });

            gameSessionInfo = SessionInfo[gameId];
            await Clients.Group(gameId).ConfirmPlayerAdded(gameSessionInfo.PlayerIds.ToList());

            return gameSessionInfo;
        }

        private async Task<GameSessionInfo> RemoveUserFromGroup()
        {
            var connId = Context.ConnectionId;

            if (!ConnectionIdMapping.TryGetValue(connId, out var gameId)) return null;

            if (!SessionInfo.TryGetValue(gameId, out var curSessionInfo)) return null;

            if (curSessionInfo.PlayerIds.Contains(connId))
            {
                curSessionInfo.PlayerIds.Remove(connId);
                ConnectionIdMapping.Remove(connId, out _);
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

        public async Task LeaveHost()
        {
            if (!TryGetCurrentGameId(out var gameId)) return;
            if (!TryGetCurrentGameInfo(out var gameSessionInfo)) return;
            
            // Only continue if current player is host
            if (gameSessionInfo.HostId != Context.ConnectionId) return;

            gameSessionInfo.HostId = null;
            SessionInfo.TryUpdate(gameId, gameSessionInfo, gameSessionInfo);

            await Clients.Group(gameId).HostChanged(string.Empty);
        }

        public async Task HostGame()
        {
            if (!TryGetCurrentGameId(out var gameId)) return;
            if (!TryGetCurrentGameInfo(out var gameSessionInfo)) return;

            if (!string.IsNullOrWhiteSpace(gameSessionInfo.HostId)) return;
            
            gameSessionInfo.HostId = Context.ConnectionId;
            SessionInfo.TryUpdate(gameId, gameSessionInfo, gameSessionInfo);

            await Clients.Group(gameId).HostChanged(Context.ConnectionId);

            var answers = _triviaService
                .GetTriviaBoardOfId(gameSessionInfo.TriviaBoard.TriviaBoardId)
                .Answers;
            await Clients.Caller.TriviaAnswersRevealed(answers);
        }
        
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var sessionInfo = await LeaveGame();

            if (sessionInfo is not null)
            {
                await Clients.Group(sessionInfo.GameId).ConfirmPlayerRemoved(sessionInfo.PlayerIds.ToList());
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}