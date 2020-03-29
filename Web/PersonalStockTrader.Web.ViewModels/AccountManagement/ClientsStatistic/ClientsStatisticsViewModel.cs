namespace PersonalStockTrader.Web.ViewModels.AccountManagement.ClientsStatistic
{
    using System;
    using System.Collections.Generic;

    public class ClientsStatisticsViewModel
    {
        public IDictionary<DateTime, decimal> TradeFeesLast7Days { get; set; }

        public IDictionary<string, decimal> MonthlyFeesLast6Months { get; set; }

        public IDictionary<DateTime, decimal> FeesLast90Days { get; set; }

        public IDictionary<DateTime, int> NewUsersLast90Days { get; set; }
    }
}
