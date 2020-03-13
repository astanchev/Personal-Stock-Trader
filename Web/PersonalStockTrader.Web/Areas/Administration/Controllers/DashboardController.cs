namespace PersonalStockTrader.Web.Areas.Administration.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using PersonalStockTrader.Services.Data;

    using ViewModels.Administration.Dashboard;

    public class DashboardController : AdministrationController
    {
        private readonly IAdministratorService administratorService;

        public DashboardController(ISettingsService settingsService, IAdministratorService administratorService)
        {
            this.administratorService = administratorService;
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
            await this.administratorService.CreateAccountManagerAsync(accountManager);

            return this.RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteAccountManager(string userId)
        {
            await this.administratorService.RemoveAccountManagerAsync(userId);

            return this.RedirectToAction("Index");
        }
    }
}
