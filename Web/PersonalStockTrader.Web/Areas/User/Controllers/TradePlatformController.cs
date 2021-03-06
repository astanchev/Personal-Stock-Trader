﻿namespace PersonalStockTrader.Web.Areas.User.Controllers
{
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using PersonalStockTrader.Common;
    using PersonalStockTrader.Services.Data;
    using PersonalStockTrader.Web.ViewModels.User.TradePlatform;

    public class TradePlatformController : UserController
    {
        private readonly IStockService stockService;
        private readonly IUserService userService;
        private readonly IAccountService accountService;

        public TradePlatformController(IStockService stockService, IUserService userService, IAccountService accountService)
        {
            this.stockService = stockService;
            this.userService = userService;
            this.accountService = accountService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await this.stockService.GetLastPriceAndTime(GlobalConstants.StockTicker);
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pricesAndTimes = await this.stockService.GetPricesLast300Minutes(GlobalConstants.StockTicker);
            var balance = await this.userService.GetUserBalanceAsync(userId);
            var accountId = await this.userService.GetUserAccountIdAsync(userId);
            var position = await this.accountService.GetCurrentPositionAsync(accountId);
            var currentProfit = position.Direction
                ? (decimal.Parse(result.Price) - position.OpenPrice) * position.Quantity
                : (position.OpenPrice - decimal.Parse(result.Price)) * position.Quantity;

            var output = new DisplayViewModel()
            {
                Ticker = GlobalConstants.StockTicker,
                PricesAndTimes = pricesAndTimes,
                LastPrice = result.Price,
                LastDateTime = result.DateTime,
                Balance = balance,
                CurrentProfit = currentProfit,
                AccountId = accountId,
                Position = position,
            };

            return this.View(output);
        }
    }
}
