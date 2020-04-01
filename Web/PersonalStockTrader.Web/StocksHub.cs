namespace PersonalStockTrader.Web
{
    using System.Threading.Tasks;
    using Common;
    using Microsoft.AspNetCore.SignalR;
    using PersonalStockTrader.Services.Data;
    using PersonalStockTrader.Web.ViewModels.Hub;

    public class StocksHub : Hub
    {
        private readonly IStockService stockService;

        public StocksHub(IStockService stockService)
        {
            this.stockService = stockService;
        }

        public async Task GetUpdateForStockPrice(string lastData)
        {
            CheckResult result;
            ;
            do
            {
                result = await this.stockService.GetUpdate(lastData, GlobalConstants.StockTicker);

                if (result.New)
                {
                    await this.Clients.Caller.SendAsync("ReceiveStockPriceUpdate", result.NewPrice, result.NewTime);
                }
            }
            while (true);
        }
    }
}