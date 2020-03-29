namespace PersonalStockTrader.Web.Areas.User.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using PersonalStockTrader.Common;
    using PersonalStockTrader.Data.Models;

    public class TradeController : UserController
    {
        private readonly SignInManager<ApplicationUser> signInManager;

        public TradeController(SignInManager<ApplicationUser> signInManager)
        {
            this.signInManager = signInManager;
        }

        [Authorize(Roles = GlobalConstants.NotConfirmedUserRoleName + "," + GlobalConstants.ConfirmedUserRoleName)]
        public IActionResult Index()
        {
            return this.View();
        }

        [Authorize(Roles = GlobalConstants.NotConfirmedUserRoleName  + "," + GlobalConstants.ConfirmedUserRoleName)]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await this.signInManager.SignOutAsync();

            return this.Redirect("/Home/Index");
        }
    }
}
