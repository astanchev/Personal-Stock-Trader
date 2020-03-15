namespace PersonalStockTrader.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class ContactController : BaseController
    {
        public IActionResult Index()
        {
            return this.View();
        }
    }
}