namespace PersonalStockTrader.Services.CronJobs
{
    using System.Threading.Tasks;

    using PersonalStockTrader.Common;

    public class SeedDBFromApi
    {
        private readonly IApiConnection alphaVantageConnection;

        public SeedDBFromApi(IApiConnection alphaVantageConnection)
        {
            this.alphaVantageConnection = alphaVantageConnection;
        }

        public async Task Work()
        {
            await this.alphaVantageConnection.GetCurrentData(GlobalConstants.StockFunction, GlobalConstants.StockTicker, GlobalConstants.StockInterval);
        }
    }
}