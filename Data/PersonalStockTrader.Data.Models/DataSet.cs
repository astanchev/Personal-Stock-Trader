namespace PersonalStockTrader.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using PersonalStockTrader.Data.Common.Models;

    public class DataSet : BaseDeletableModel<int>
    {
        public DateTime DateAndTime { get; set; }

        [Range(typeof(decimal), "0.01", "10000.00")]
        public decimal OpenPrice { get; set; }

        [Range(typeof(decimal), "0.01", "10000.00")]
        public decimal HighPrice { get; set; }

        [Range(typeof(decimal), "0.01", "10000.00")]
        public decimal LowPrice { get; set; }

        [Range(typeof(decimal), "0.01", "10000.00")]
        public decimal ClosePrice { get; set; }

        [Range(typeof(decimal), "0.01", "10000.00")]
        public long Volume { get; set; }

        public int IntervalId { get; set; }

        public virtual Interval Interval { get; set; }
    }
}