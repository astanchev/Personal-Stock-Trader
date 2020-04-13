namespace PersonalStockTrader.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IUserService
    {
        Task<decimal> GetUserBalanceAsync(string userId);

        Task<int> GetUserAccountIdAsync(string userId);

        Task<IDictionary<DateTime, decimal>> GetUserPaidTradeFees(string userId, string startDate, string endDate);

        Task<IDictionary<DateTime, decimal>> GetUserPaidMonthlyFees(string userId, string startDate, string endDate);

        Task<IDictionary<DateTime, decimal>> GetUserProfitLoss(string userId, string startDate, string endDate);
    }
}