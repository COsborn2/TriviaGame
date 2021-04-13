using System.Threading.Tasks;
using TriviaGame.Data.Models;
using TriviaGame.Data.Services.Impl;

namespace TriviaGame.Data.Services.Interfaces
{
    public interface IGameBoardHub
    {
        Task ConfirmPlayerAdded(Player player);

        Task ConfirmPlayerRemoved(Player player);

        Task PlayerTeamUpdated(Player player);

        Task ReceiveGameInformation(GameSessionInfo gameSessionInfo);

        Task HostChanged(Player hostId);

        Task TriviaAnswersRevealed(params TriviaAnswer[] answers);
    }
}
