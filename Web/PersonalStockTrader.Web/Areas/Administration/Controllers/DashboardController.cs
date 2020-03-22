namespace PersonalStockTrader.Web.Areas.Administration.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using PersonalStockTrader.Data.Models;
    using PersonalStockTrader.Services.Data;
    using PersonalStockTrader.Web.ViewModels.Administration.Dashboard;

    public class DashboardController : AdministrationController
    {
        private readonly IAdministratorService administratorService;
        private readonly SignInManager<ApplicationUser> signInManager;

        public DashboardController(IAdministratorService administratorService, SignInManager<ApplicationUser> signInManager)
        {
            this.administratorService = administratorService;
            this.signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new IndexViewModel
            {
                AccountManagers = await this.administratorService
                    .GetAllAccountManagersAsync(),
            };

            return this.View(viewModel);
        }

        public IActionResult CreateAccountManager()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccountManager(AccountManagerInputViewModel accountManager)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(accountManager);
            }

            await this.administratorService.CreateAccountManagerAsync(accountManager);

            return this.RedirectToAction(nameof(this.Index));
        }

        public async Task<IActionResult> UpdateAccountManager(string userId)
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToAction(nameof(this.Index));
            }

            var user = await this.administratorService.GetAccountManagersByIdAsync(userId);

            return this.View(user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAccountManager(AccountManagerOutputViewModel accountManager)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(accountManager);
            }

            await this.administratorService.UpdateAccountManagerAsync(accountManager);

            return this.RedirectToAction(nameof(this.Index));
        }

        public async Task<IActionResult> DeleteAccountManager(string userId)
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToAction(nameof(this.Index));
            }

            await this.administratorService.RemoveAccountManagerAsync(userId);

            return this.RedirectToAction(nameof(this.Index));
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await this.signInManager.SignOutAsync();

            return this.Redirect("/Home/Index");
        }
    }
}
