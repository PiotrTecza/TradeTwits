using TradeTwits.Models;

namespace TradeTwits.Service
{
    public interface IUserFacade
    {
        void UpdateRanking(Vote vote);
    }
}
