namespace PersonalStockTrader.Web.ViewModels.AccountManagement.ClientsStatistic
{
    using System;
    using System.Collections.Generic;

    public class StatisticsViewModel
    {
        public IDictionary<DateTime, decimal> Statistic { get; set; }
    }
}