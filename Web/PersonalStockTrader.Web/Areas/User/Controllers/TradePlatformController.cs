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
        private readonly IAccountService accountService;

        public TradePlatformController(IStockService stockService, IUserService userService, UserManager<ApplicationUser> userManager, IAccountService accountService)
        {
            this.stockService = stockService;
            this.userService = userService;
            this.userManager = userManager;
            this.accountService = accountService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await this.stockService.GetLastPriceAndTime(GlobalConstants.StockTicker);
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pricesAndTimes = await this.stockService.GetPricesLast300Minutes(GlobalConstants.StockTicker);
            var balance = await this.userService.GetUserBalanceAsync(userId);
            var accountId = await this.userService.GetUserAccountIdAsync(userId);
            var position = await this.accountService.GetCurrentPosition(accountId);

            var output = new DisplayViewModel()
            {
                PricesAndTimes = pricesAndTimes,
                LastPrice = result.Price,
                LastDateTime = result.DateTime,
                Balance = balance,
                AccountId = accountId,
                Position = position,
            };

            return this.View(output);
        }

        [HttpPost]
        public async Task<IActionResult> TradeShares(TradeSharesInputViewModel input)
        {
            await this.accountService.ManagePositions(input);

            return this.RedirectToAction(nameof(this.Index));
        }
    }
}