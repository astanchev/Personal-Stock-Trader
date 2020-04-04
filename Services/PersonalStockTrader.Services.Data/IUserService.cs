namespace PersonalStockTrader.Services.Data
{
    using System.Threading.Tasks;

    public interface IUserService
    {
        Task<decimal> GetUserBalanceAsync(string userId);

        Task<int> GetUserAccountIdAsync(string userId);
    }
}