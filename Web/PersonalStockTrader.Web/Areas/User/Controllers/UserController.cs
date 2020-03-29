namespace PersonalStockTrader.Web.Areas.User.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using PersonalStockTrader.Common;
    using PersonalStockTrader.Web.Controllers;

    [Authorize(Roles = GlobalConstants.ConfirmedUserRoleName)]
    [Area("User")]
    public class UserController : BaseController
    {
    }
}