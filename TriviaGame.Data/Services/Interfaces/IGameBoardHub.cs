using System.Collections.Generic;
using System.Threading.Tasks;
using TriviaGame.Data.Models;
using TriviaGame.Data.Services.Impl;

namespace TriviaGame.Data.Services.Interfaces
{
    public interface IGameBoardHub
    {
        Task ConfirmPlayerAdded(List<string> currentUserIds);

        Task ConfirmPlayerRemoved(List<string> currentUserIds);

        Task ReceiveGameInformation(GameSessionInfo gameSessionInfo);

        Task HostChanged(string hostId);

        Task TriviaAnswersRevealed(ICollection<TriviaAnswer> answers);
    }
}