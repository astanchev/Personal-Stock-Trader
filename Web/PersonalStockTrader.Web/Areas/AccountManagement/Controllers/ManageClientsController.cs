namespace PersonalStockTrader.Web.Areas.AccountManagement.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
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
                Clients = this.accountManagement.GetAllConfirmedClients(),
            };

            return this.View(viewModel);
        }

        public async Task<IActionResult> ManageClient(string userId)
        {
            var user = await this.accountManagement.GetClientToBeManagedByIdAsync(userId);

            return this.View(user);
        }

        [HttpPost]
        public async Task<IActionResult> ManageClient(ClientToBeManagedViewModel input)
        {
            if (!this.ModelState.IsValid)
            {
                return await this.ManageClient(input.UserId);
            }

            return this.RedirectToAction(nameof(this.Index));
        }
    }
}
