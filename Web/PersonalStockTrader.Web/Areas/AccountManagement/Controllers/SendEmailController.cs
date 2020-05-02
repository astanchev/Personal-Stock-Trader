namespace PersonalStockTrader.Web.Areas.AccountManagement.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using PersonalStockTrader.Common;
    using PersonalStockTrader.Services.Messaging;
    using PersonalStockTrader.Web.ViewModels.AccountManagement.ManageClients;

    [ApiController]
    [Authorize(Roles = GlobalConstants.AccountManagerRoleName)]
    [Route("api/[controller]")]
    public class SendEmailController : ControllerBase
    {
        private readonly IEmailSender emailSender;

        public SendEmailController(IEmailSender emailSender)
        {
            this.emailSender = emailSender;
        }

        [HttpPost]
        public async Task<ActionResult<ResponseEmailViewModel>> Post(OutgoingEmailViewModel email)
        {
            await this.emailSender.SendEmailAsync(
                GlobalConstants.SystemEmail,
                this.User.Identity.Name,
                email.Email,
                email.Subject,
                email.Message);

            return new ResponseEmailViewModel()
            {
                Response = GlobalConstants.EmailSent,
            };
        }
    }
}
