using System.Collections.Generic;
using System.Linq;
using TriviaGame.Data.Models;

namespace TriviaGame.Data.Services.Impl
{
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
}