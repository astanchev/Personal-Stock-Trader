namespace PersonalStockTrader.Data.Models
{
    using System;

    using PersonalStockTrader.Data.Common.Models;

    public class MetaData : BaseDeletableModel<int>
    {
        public string Information { get; set; }

        public DateTime LastRefreshed { get; set; }

        public string OutputSize { get; set; }

        public string TimeZone { get; set; }

        public int IntervalId { get; set; }

        public virtual Interval Interval { get; set; }
    }
}