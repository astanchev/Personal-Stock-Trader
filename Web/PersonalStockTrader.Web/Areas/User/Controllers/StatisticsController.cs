namespace PersonalStockTrader.Web.Areas.User.Controllers
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using PersonalStockTrader.Services.Data;
    using PersonalStockTrader.Web.ViewModels.User.Statistics;

    public class StatisticsController : UserController
    {

        private readonly IUserService userService;
        private readonly string start;
        private readonly string end;

        public StatisticsController(IUserService userService)
        {
            this.userService = userService;
            this.start = DateTime.Now.AddMonths(-3).ToShortDateString();
            this.end = DateTime.Now.ToShortDateString();
        }

        public async Task<IActionResult> Index(string startDate, string endDate)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var startPeriod = startDate ?? this.start;
            var endPeriod = endDate ?? this.end;

            var result = new StatisticsViewModel
            {
                PaidTradeFees = await this.userService.GetUserPaidTradeFees(userId, startPeriod, endPeriod),
                PaidMonthlyFees = await this.userService.GetUserPaidMonthlyFees(userId, startPeriod, endPeriod),
                ProfitLoss = await this.userService.GetUserProfitLoss(userId, startPeriod, endPeriod),
            };

            return this.View(result);
        }

        [HttpPost]
        public async Task<IActionResult> GetUpdate(string startDate, string endDate)
        {
            return this.RedirectToAction(nameof(Index), new { startDate, endDate });
        }
    }
}