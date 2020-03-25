namespace PersonalStockTrader.Web.ViewModels.AccountManagement.ClientsStatistic
{
    using System.Collections.Generic;

    public class ClientsStatisticsViewModel
    {
        public IDictionary<string, decimal> TradeFeesLast7Days { get; set; }

        public IDictionary<string, decimal> MonthlyFeesLast6Months { get; set; }

        public IDictionary<string, decimal> FeesLast90Days { get; set; }
    }
}