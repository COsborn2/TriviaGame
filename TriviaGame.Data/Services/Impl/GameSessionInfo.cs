using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using TriviaGame.Data.Models;

namespace TriviaGame.Data.Services.Impl
{
    public class GameSessionInfo
    {
        public GameSessionInfo(string gameId)
        {
            GameId = gameId;
        }

        public GameSessionInfo(string gameId, params Player[] players) : this(gameId)
        {
            foreach (var player in players)
            {
                PlayerIds.Add(player.ConnectionId, player);
            }
        }

        public string GameId { get; set; }
        
        [JsonIgnore]
        public Dictionary<string, Player> PlayerIds { get; set; } = new();

        public IEnumerable<Player> Players => PlayerIds.Select(x => x.Value);

        public TriviaBoard TriviaBoard { get; set; }

        private int _totalAnswers;

        public int TotalAnswers
        {
            get => TriviaBoard.Answers.Any() ? TriviaBoard.Answers.Count : _totalAnswers;
            set => _totalAnswers = value;
        }

        public Player Host { get; set; }
    }

    public class Player
    {
        public string ConnectionId { get; set; }

        public Team Team { get; set; }
    }

    public enum Team
    {
        Unknown = 0,
        Left = 1,
        Right = 2
    }
}