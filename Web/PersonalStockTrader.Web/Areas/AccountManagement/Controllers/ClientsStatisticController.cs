namespace PersonalStockTrader.Web.Areas.AccountManagement.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using PersonalStockTrader.Services.Data;
    using PersonalStockTrader.Web.ViewModels.AccountManagement.ClientsStatistic;

    public class ClientsStatisticController : AccountManagementController
    {
        private readonly IAccountManagementService accountManagement;

        public ClientsStatisticController(IAccountManagementService accountManagement)
        {
            this.accountManagement = accountManagement;
        }

        public IActionResult Index()
        {
            var viewModel = new ClientsStatisticsViewModel
            {
                TradeFeesLast7Days = this.accountManagement.GetPaidTradeFeesLast7Days(),
                MonthlyFeesLast6Months = this.accountManagement.GetPaidMonthlyFeesLast6Months(),
                FeesLast90Days = this.accountManagement.GetAllPaidFeesLast90Days(),
            };

            return this.View(viewModel);
        }
    }
}