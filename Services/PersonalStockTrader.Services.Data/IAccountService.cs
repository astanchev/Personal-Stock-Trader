namespace PersonalStockTrader.Services.Data
{
    using System.Threading.Tasks;

    using PersonalStockTrader.Web.ViewModels.User.TradeHistory;
    using PersonalStockTrader.Web.ViewModels.User.TradePlatform;
    using PersonalStockTrader.Web.ViewModels.User.TradeShares;

    public interface IAccountService
    {
        Task<TradeSharesResultModel> ManagePositionsAsync(TradeSharesInputViewModel input);

        Task<PositionViewModel> GetCurrentPositionAsync(int accountId);

        Task<TradeHistoryViewModel> GetAllClosedPositionsByUserIdAsync(string userId);

        Task<TradeHistoryViewModel> GetAllClosedPositionsIntervalByUserIdAsync(string userId, string startDate, string endDate);

        Task TakeAllAccountsMonthlyFeesAsync();
    }
}