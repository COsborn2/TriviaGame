using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TriviaGame.Data.Models;

namespace TriviaGame.Data
{
    public static class TriviaBoardsDbSetExtensions
    {
        public static int GetRandomTriviaBoardId(this DbSet<TriviaBoard> triviaBoards)
        {
            return triviaBoards
                .OrderBy(x => Guid.NewGuid())
                .Select(x => x.TriviaBoardId)
                .First();
        }
    }
}
