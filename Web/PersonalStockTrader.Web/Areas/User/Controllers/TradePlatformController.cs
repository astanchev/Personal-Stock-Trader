namespace PersonalStockTrader.Web.Areas.User.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using PersonalStockTrader.Common;
    using PersonalStockTrader.Services.Data;
    using PersonalStockTrader.Web.ViewModels.User.TradePlatform;

    public class TradePlatformController : UserController
    {
        private readonly IStockService stockService;

        public TradePlatformController(IStockService stockService)
        {
            this.stockService = stockService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await this.stockService.GetLastPriceAndTime(GlobalConstants.StockTicker);

            var output = new DisplayViewModel()
            {
                PricesAndTimes = await this.stockService.GetPricesLast300Minutes(GlobalConstants.StockTicker),
                LastPrice = result.Price,
                LastDateTime = result.DateTime,
            };

            return this.View(output);
        }
    }
}