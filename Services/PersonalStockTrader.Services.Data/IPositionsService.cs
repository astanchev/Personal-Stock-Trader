namespace PersonalStockTrader.Services.Data
{
    using System.Threading.Tasks;

    using PersonalStockTrader.Web.ViewModels.User.TradePlatform;

    public interface IPositionsService
    {
        Task OpenPosition(int accountId, int numberShares, bool isBuy);

        Task UpdatePosition(int accountId, int positionId, int numberShares, bool isBuy);

        Task ClosePosition(int accountId);

        Task<PositionViewModel> GetOpenPosition(int accountId);
    }
}