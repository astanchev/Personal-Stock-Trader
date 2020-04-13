namespace PersonalStockTrader.Web.ViewModels.User.Statistics
{
    using System;
    using System.Collections.Generic;

    public class StatisticsViewModel
    {
        public IDictionary<DateTime, decimal> PaidTradeFees { get; set; }

        public IDictionary<DateTime, decimal> PaidMonthlyFees { get; set; }

        public IDictionary<DateTime, decimal> ProfitLoss { get; set; }
    }
}