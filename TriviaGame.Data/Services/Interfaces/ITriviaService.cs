using IntelliTect.Coalesce;
using TriviaGame.Data.Models;

namespace TriviaGame.Data.Services.Interfaces
{
    [Coalesce, Service]
    public interface ITriviaService
    {
        TriviaBoard GetRandomTriviaBoard();
        TriviaBoard GetRandomTriviaBoardWithNoAnswers();
    }
}