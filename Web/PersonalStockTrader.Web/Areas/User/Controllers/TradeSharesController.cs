namespace PersonalStockTrader.Web.Areas.User.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using PersonalStockTrader.Common;
    using PersonalStockTrader.Services.Data;
    using PersonalStockTrader.Web.ViewModels.User.TradePlatform;
    using PersonalStockTrader.Web.ViewModels.User.TradeShares;

    [ApiController]
    [Authorize(Roles = GlobalConstants.ConfirmedUserRoleName)]
    [Route("api/[controller]")]
    public class TradeSharesController : ControllerBase
    {
        private readonly IAccountService accountService;

        public TradeSharesController(IAccountService accountService)
        {
            this.accountService = accountService;
        }

        [HttpPost]
        public async Task<ActionResult<TradeSharesResultModel>> Post(TradeSharesInputViewModel input)
        {
            if (decimal.Parse(input.Balance) - GlobalConstants.MinAccountBalance <= decimal.Parse(input.CurrentPrice) * int.Parse(input.Quantity))
            {
                return new TradeSharesResultModel()
                {
                    PositionId = -1,
                };
            }

            var result = await this.accountService.ManagePositionsAsync(input);

            if (result != null)
            {
                return result;
            }
            else
            {
                return new TradeSharesResultModel()
                {
                    PositionId = -1,
                };
            }
        }
    }
}
