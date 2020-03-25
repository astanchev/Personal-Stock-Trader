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
    }
}
