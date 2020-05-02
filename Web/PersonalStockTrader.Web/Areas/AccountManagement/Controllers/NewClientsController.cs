namespace PersonalStockTrader.Web.Areas.AccountManagement.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using PersonalStockTrader.Common;
    using PersonalStockTrader.Data.Models;
    using PersonalStockTrader.Services.Data;
    using PersonalStockTrader.Services.Messaging;
    using PersonalStockTrader.Web.ViewModels.AccountManagement.NewClients;

    public class NewClientsController : AccountManagementController
    {
        private readonly IAccountManagementService accountManagement;
        private readonly IEmailSender emailSender;
        private readonly SignInManager<ApplicationUser> signInManager;

        public NewClientsController(IAccountManagementService accountManagement, IEmailSender emailSender, SignInManager<ApplicationUser> signInManager)
        {
            this.accountManagement = accountManagement;
            this.emailSender = emailSender;
            this.signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new NewClientsViewModel()
            {
                NewClients = await this.accountManagement.GetAllNotConfirmedClientsAsync(),
            };

            return this.View(viewModel);
        }

        public async Task<IActionResult> ConfirmAccount(string userId)
        {
            var user = await this.accountManagement.GetClientToBeConfirmedByIdAsync(userId);

            return this.View(user);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmAccount(ClientToBeConfirmedViewModel input)
        {
            if (!this.ModelState.IsValid)
            {
                return await this.ConfirmAccount(input.UserId);
            }

            await this.accountManagement.ConfirmUserAccountAsync(input);

            await this.emailSender.SendEmailAsync(
                GlobalConstants.SystemEmail,
                this.User.Identity.Name,
                input.Email,
                GlobalConstants.ConfirmSubject,
                GlobalConstants.ConfirmMessage);

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
