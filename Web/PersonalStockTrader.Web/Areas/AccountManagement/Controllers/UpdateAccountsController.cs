namespace PersonalStockTrader.Web.Areas.AccountManagement.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using PersonalStockTrader.Common;
    using PersonalStockTrader.Services.Data;
    using PersonalStockTrader.Web.ViewModels.AccountManagement.ManageClients;

    [ApiController]
    [Authorize(Roles = GlobalConstants.AccountManagerRoleName)]
    [Route("api/[controller]")]
    public class UpdateAccountsController : ControllerBase
    {
        private readonly IAccountManagementService accountManagement;

        public UpdateAccountsController(IAccountManagementService accountManagement)
        {
            this.accountManagement = accountManagement;
        }

        [HttpPost]
        public async Task<ActionResult<UpdateAccountResponseModel>> Post(UpdateAccountViewModel input)
        {
            string userId = input.UserId;
            decimal balance = decimal.Parse(input.Balance);
            decimal tradeFee = decimal.Parse(input.TradeFee);
            decimal monthlyFee = decimal.Parse(input.MonthlyFee);

            await this.accountManagement.UpdateUserAccountAsync(userId, balance, tradeFee, monthlyFee);

            return new UpdateAccountResponseModel
            {
                Response = "Updated",
            };
        }
    }
}