namespace PersonalStockTrader.Web.Areas.User.Controllers
{
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Data.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using PersonalStockTrader.Common;
    using PersonalStockTrader.Services.Data;
    using PersonalStockTrader.Web.ViewModels.User.TradePlatform;

    public class TradePlatformController : UserController
    {
        private readonly IStockService stockService;
        private readonly IUserService userService;
        private readonly UserManager<ApplicationUser> userManager;

        public TradePlatformController(IStockService stockService, IUserService userService, UserManager<ApplicationUser> userManager)
        {
            this.stockService = stockService;
            this.userService = userService;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var result = await this.stockService.GetLastPriceAndTime(GlobalConstants.StockTicker);

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var output = new DisplayViewModel()
            {
                PricesAndTimes = await this.stockService.GetPricesLast300Minutes(GlobalConstants.StockTicker),
                LastPrice = result.Price,
                LastDateTime = result.DateTime,
                Balance = await this.userService.GetUserBalanceAsync(userId),
            };

            return this.View(output);
        }
    }
}