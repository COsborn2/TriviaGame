using System;
using System.Collections.Generic;
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

        private TriviaBoard GetBoardWithAnswers(int id)
        {
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

        public TriviaBoard GetRandomTriviaBoard()
        {
            var id = _context.TriviaBoards.GetRandomTriviaBoardId();

            return GetBoardWithAnswers(id);
        }

        public (TriviaBoard board, int totalAnswers) GetRandomTriviaBoardWithNoAnswers()
        {
            var id = _context.TriviaBoards.GetRandomTriviaBoardId();
            var board = _context.TriviaBoards
                .Include(x => x.Answers)
                .First(x => x.TriviaBoardId == id);
            var answerCount = board.Answers.Count;
            board.Answers = new List<TriviaAnswer>();

            return (board, answerCount);
        }

        public TriviaBoard GetTriviaBoardOfId(int id)
        {
            return GetBoardWithAnswers(id);
        }
    }
}
