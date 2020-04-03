namespace PersonalStockTrader.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class TempData
    {
        [Key]
        public int ID { get; set; }

        public DateTime LastDateTime { get; set; }

        public decimal LastPrice { get; set; }
    }
}
