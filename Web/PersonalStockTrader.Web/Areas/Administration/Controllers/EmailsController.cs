namespace PersonalStockTrader.Web.Areas.Administration.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using PersonalStockTrader.Common;
    using PersonalStockTrader.Services.Data;
    using PersonalStockTrader.Services.Messaging;
    using PersonalStockTrader.Web.ViewModels.Administration.Emails;

    public class EmailsController : AdministrationController
    {
        private readonly IContactFormService contactFormService;
        private readonly IEmailSender emailSender;

        public EmailsController(IContactFormService contactFormService, IEmailSender emailSender)
        {
            this.contactFormService = contactFormService;
            this.emailSender = emailSender;
        }

        public async Task<IActionResult> Index()
        {
            var countAll = await this.contactFormService.GetAllCountAsync();
            var allNotAnswered = this.contactFormService.GetAllNotAnswered();
            var notAnsweredLast10Days = this.contactFormService.GetNotAnsweredLast10Days();


            var viewModel = new EmailsIndexPageViewModel
            {
                NotAnsweredEmails = allNotAnswered,
                CountAnsweredEmails = countAll.CountAnswered,
                CountNotAnsweredEmails = countAll.CountNotAnswered,
                NotAnsweredLast10Days = notAnsweredLast10Days,
            };

            return this.View(viewModel);
        }

        public async Task<IActionResult> AnswerEmail(int emailId)
        {
            var notAnsweredEmail = await this.contactFormService.GetByIdAsync(emailId);

            if (notAnsweredEmail == null)
            {
                return this.RedirectToAction(nameof(this.Index));
            }

            var viewModel = new SendReplayViewModel
            {
                Id = notAnsweredEmail.Id,
                Name = notAnsweredEmail.Name,
                Email = notAnsweredEmail.Email,
                Subject = notAnsweredEmail.Subject,
                Content = notAnsweredEmail.Content,
                Answered = notAnsweredEmail.Answered,
                Answer = string.Empty,
            };

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AnswerEmail(SendReplayViewModel input)
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToAction(nameof(this.Index));
            }

            await this.contactFormService.MarkAsAnsweredAsync(input.Id);

            await this.emailSender.SendEmailAsync(
                GlobalConstants.SystemEmail,
                GlobalConstants.AdministratorRoleName,
                input.Email,
                input.Subject,
                input.Answer);

            return this.RedirectToAction(nameof(this.Index));
        }
    }
}