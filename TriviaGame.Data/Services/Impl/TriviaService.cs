using System;
using System.Linq;
using IntelliTect.Coalesce;
using Microsoft.EntityFrameworkCore;
using TriviaGame.Data.Models;

namespace TriviaGame.Data.Services.Impl
{
    [Coalesce, Service]
    public interface ITriviaService
    {
        TriviaBoard GetRandomTriviaBoard();
        TriviaBoard GetRandomTriviaBoardWithNoAnswers();
    }
    
    public class TriviaService : ITriviaService
    {
        private readonly AppDbContext _context;

        public TriviaService(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public TriviaBoard GetRandomTriviaBoard()
        {
            var id = _context.TriviaBoards.GetRandomTriviaBoardId();
            var gameBoard = _context.TriviaBoards
                .Include(x => x.Answers)
                .First(x => x.TriviaBoardId == id);

            gameBoard.Answers = gameBoard.Answers.OrderByDescending(x => x.Points).ToList();
            
            for (var i = 0; i < gameBoard.Answers.Count; i++)
            {
                gameBoard.Answers[i].Position = i;
            }

            return gameBoard;
        }

        public TriviaBoard GetRandomTriviaBoardWithNoAnswers()
        {
            var id = _context.TriviaBoards.GetRandomTriviaBoardId();
            return _context.TriviaBoards
                .Include(x => x.Answers)
                .First(x => x.TriviaBoardId == id);
        }
    }
}