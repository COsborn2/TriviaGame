using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using TriviaGame.Data.Models;

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
                gameSessionInfo.TriviaBoard = _triviaService.GetRandomTriviaBoard();
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

        public async Task LeaveGame()
        {
            await RemoveUserFromGroup();
        }
        
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var sessionInfo = await RemoveUserFromGroup();

            if (sessionInfo is not null)
            {
                await Clients.Group(sessionInfo.GameId).ConfirmPlayerRemoved(sessionInfo.PlayerIds.ToList());
            }

            await base.OnDisconnectedAsync(exception);
        }
    }

    public class GameSessionInfo
    {
        public GameSessionInfo(string gameId)
        {
            GameId = gameId;
        }

        public GameSessionInfo(string gameId, params string[] connectIds) : this(gameId)
        {
            foreach (var connectId in connectIds)
            {
                PlayerIds.Add(connectId);
            }
        }

        public string GameId { get; set; }
        
        public HashSet<string> PlayerIds { get; set; } = new();

        public TriviaBoard TriviaBoard { get; set; }

        private int _totalAnswers;

        public int TotalAnswers
        {
            get => TriviaBoard.Answers.Any() ? TriviaBoard.Answers.Count : _totalAnswers;
            set => _totalAnswers = value;
        }
    }

    public interface IGameBoardHub
    {
        Task ConfirmPlayerAdded(List<string> currentUserIds);

        Task ConfirmPlayerRemoved(List<string> currentUserIds);

        Task ReceiveGameInformation(GameSessionInfo gameSessionInfo);
    }
}