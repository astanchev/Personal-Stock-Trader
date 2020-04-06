namespace PersonalStockTrader.Services.CronJobs
{
    using System.Threading.Tasks;

    using PersonalStockTrader.Services.Data;

    public class TakeAllMonthlyFees
    {
        private readonly IAccountService accountService;

        public TakeAllMonthlyFees(IAccountService accountService)
        {
            this.accountService = accountService;
        }

        public async Task Work()
        {
            await this.accountService.TakeAllAccountsMonthlyFeesAsync();
        }
    }
}
