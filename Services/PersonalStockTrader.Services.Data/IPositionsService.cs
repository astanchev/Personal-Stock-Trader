namespace PersonalStockTrader.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using PersonalStockTrader.Web.ViewModels.User.TradeHistory;
    using PersonalStockTrader.Web.ViewModels.User.TradePlatform;
    using PersonalStockTrader.Web.ViewModels.User.TradeShares;

    public interface IPositionsService
    {
        Task<TradeSharesResultModel> OpenPosition(int accountId, int numberShares, decimal currentPrice, bool isBuy);

        Task<TradeSharesResultModel> UpdatePosition(int accountId, int positionId, int numberShares, decimal currentPrice, bool isBuy);

        Task ClosePosition(int accountId);

        Task<PositionViewModel> GetOpenPosition(int accountId);

        Task<IEnumerable<HistoryPositionViewModel>> GetAccountClosedPositions(int accountId, string startDate, string endDate);
    }
}