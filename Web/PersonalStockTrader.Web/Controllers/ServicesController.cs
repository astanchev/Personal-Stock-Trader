namespace PersonalStockTrader.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class ServicesController : BaseController
    {
        public IActionResult Index()
        {
            return this.View();
        }
    }
}