namespace PersonalStockTrader.Services.CronJobs
{
    using System.Threading.Tasks;

    public class SendMonthlyReport
    {
        public Task Work()
        {
            return Task.CompletedTask;
        }
    }
}