namespace PersonalStockTrader.Services.CronJobs
{
    using System.Threading.Tasks;

    public class SeedDBFromApi
    {
        private readonly IApiConnection alphaVantageConnection;
        private readonly string function = "TIME_SERIES_INTRADAY";
        private readonly string ticker = "MSFT";
        private readonly string interval = "1min";

        public SeedDBFromApi(IApiConnection alphaVantageConnection)
        {
            this.alphaVantageConnection = alphaVantageConnection;
        }

        public async Task Work()
        {
            await alphaVantageConnection.GetCurrentData(function, ticker, interval);
        }


        //public async Task Work()
        //{
        //    if (!context.DataSets.Any())
        //    {
        //        return;
        //    }

        //    var dataFromDb = context
        //        .DataSets
        //        .OrderBy(x => x.DateAndTime)
        //        .First();

        //    var priceData = new TestTable
        //    {
        //        DateTime = dataFromDb.DateAndTime,
        //        Price = dataFromDb.ClosePrice
        //    };

        //    context.TestTables.Add(priceData);
        //    context.DataSets.Remove(dataFromDb);
        //    await context.SaveChangesAsync();

        //}
    }
}