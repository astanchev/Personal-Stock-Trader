namespace PersonalStockTrader.Web.Areas.User.Controllers
{
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using PersonalStockTrader.Services.Data;

    public class TradeHistoryController : UserController
    {
        private readonly IAccountService accountService;

        public TradeHistoryController(IAccountService accountService)
        {
            this.accountService = accountService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await this.accountService.GetAllClosedPositionsByUserId(userId);

            return this.View(result);
        }

        [HttpPost]
        public async Task<IActionResult> Index(string startDate, string endDate)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await this.accountService.GetAllClosedPositionsIntervalByUserId(userId, startDate, endDate);

            return this.View(result);
        }
    }
}