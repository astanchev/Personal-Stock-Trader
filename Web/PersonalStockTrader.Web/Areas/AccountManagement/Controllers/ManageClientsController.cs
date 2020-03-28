namespace PersonalStockTrader.Web.Areas.AccountManagement.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.WebEncoders.Testing;
    using Newtonsoft.Json.Linq;
    using PersonalStockTrader.Services.Data;
    using PersonalStockTrader.Services.Messaging;
    using PersonalStockTrader.Web.ViewModels.AccountManagement.ManageClients;

    public class ManageClientsController : AccountManagementController
    {
        private readonly IAccountManagementService accountManagement;
        private readonly IEmailSender emailSender;

        public ManageClientsController(IAccountManagementService accountManagement, IEmailSender emailSender)
        {
            this.accountManagement = accountManagement;
            this.emailSender = emailSender;
        }

        public IActionResult Index()
        {
            var viewModel = new ManageClientsViewModel()
            {
                Clients = this.accountManagement.GetAllConfirmedAccounts(),
            };

            return this.View(viewModel);
        }

        public async Task<IActionResult> ManageClient(int accountId)
        {
            var user = await this.accountManagement.GetClientToBeManagedByAccountIdAsync(accountId);

            return this.View(user);
        }

        [HttpPost]
        public async Task<IActionResult> ManageClient(ClientToBeManagedViewModel input)
        {
            if (!this.ModelState.IsValid)
            {
                return await this.ManageClient(input.AccountId);
            }

            if (input.AccountIsDeleted)
            {
                await this.accountManagement.RestoreUserAccountAsync(input.AccountId);
            }
            else
            {
                await this.accountManagement.DeleteUserAccountAsync(input.AccountId);
            }

            return this.RedirectToAction(nameof(this.Index));
        }

        public async Task<IActionResult> DeleteClient(string userId)
        {
            await this.accountManagement.DeleteUserAsync(userId);

            return this.RedirectToAction(nameof(this.Index));
        }
    }
}
