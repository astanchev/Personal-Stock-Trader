﻿namespace PersonalStockTrader.Web.Areas.User.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using PersonalStockTrader.Common;
    using PersonalStockTrader.Data.Models;
    using PersonalStockTrader.Web.Controllers;

    [Area("User")]
    public class TradeController : BaseController
    {
        private readonly SignInManager<ApplicationUser> signInManager;

        public TradeController(SignInManager<ApplicationUser> signInManager)
        {
            this.signInManager = signInManager;
        }

        [Authorize(Roles = GlobalConstants.NotConfirmedUserRoleName + "," + GlobalConstants.ConfirmedUserRoleName)]
        public IActionResult Index()
        {
            if (this.User.IsInRole(GlobalConstants.ConfirmedUserRoleName))
            {
                return this.Redirect("/User/TradePlatform/Index");
            }

            return this.View();
        }

        [Authorize(Roles = GlobalConstants.NotConfirmedUserRoleName + "," + GlobalConstants.ConfirmedUserRoleName)]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await this.signInManager.SignOutAsync();

            return this.Redirect("/Home/Index");
        }
    }
}
