namespace PersonalStockTrader.Services.Data
{
    using System.Threading.Tasks;

    using PersonalStockTrader.Web.ViewModels.User.TradePlatform;

    public interface IAccountService
    {
        Task ManagePositions(TradeSharesInputViewModel input);

        Task<PositionViewModel> GetCurrentPosition(int accountId);
    }
}