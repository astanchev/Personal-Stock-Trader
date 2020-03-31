namespace PersonalStockTrader.Web.Areas.User.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class TradeHistoryController : UserController
    {
        public IActionResult Index()
        {
            return this.View();
        }
    }
}