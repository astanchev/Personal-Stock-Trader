namespace PersonalStockTrader.Web.Areas.AccountManagement.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using PersonalStockTrader.Common;
    using PersonalStockTrader.Web.Controllers;

    [Authorize(Roles = GlobalConstants.AccountManagerRoleName)]
    [Area("AccountManagement")]
    public class AccountManagementController : BaseController
    {
    }
}
