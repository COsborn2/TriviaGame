using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TriviaGame.Data.Models;
using TriviaGame.Data.Services.Interfaces;

namespace TriviaGame.Data.Services.Impl
{
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