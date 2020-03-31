namespace PersonalStockTrader.Web.Areas.User.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class TradePlatformController : UserController
    {
        public IActionResult Index()
        {
            return this.View();
        }
    }
}