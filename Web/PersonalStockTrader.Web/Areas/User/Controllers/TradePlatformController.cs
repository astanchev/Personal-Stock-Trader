namespace PersonalStockTrader.Web.Areas.User.Controllers
{
    using System.Threading.Tasks;
    using Common;
    using Microsoft.AspNetCore.Mvc;
    using Services.Data;

    public class TradePlatformController : UserController
    {
        private readonly IStockService stockService;

        public TradePlatformController(IStockService stockService)
        {
            this.stockService = stockService;
        }

        public async Task<IActionResult> Index()
        {
            var output = await this.stockService.GetLastPriceAndTime(GlobalConstants.StockTicker);

            return View("Index", output);
        }
    }
}