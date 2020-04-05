namespace PersonalStockTrader.Services.Data
{
    using System.Threading.Tasks;

    using PersonalStockTrader.Web.ViewModels.User.TradePlatform;
    using PersonalStockTrader.Web.ViewModels.User.TradeShares;

    public interface IPositionsService
    {
        Task<TradeSharesResultModel> OpenPosition(int accountId, int numberShares, bool isBuy);

        Task<TradeSharesResultModel> UpdatePosition(int accountId, int positionId, int numberShares, bool isBuy);

        Task ClosePosition(int accountId);

        Task<PositionViewModel> GetOpenPosition(int accountId);
    }
}