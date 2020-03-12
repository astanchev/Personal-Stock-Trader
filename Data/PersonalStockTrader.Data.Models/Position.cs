namespace PersonalStockTrader.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using PersonalStockTrader.Data.Common.Models;

    public class Position : BaseDeletableModel<int>
    {
        public int StockId { get; set; }

        public Stock Stock { get; set; }

        [Range(1, int.MaxValue)]
        public int CountStocks { get; set; }

        public DateTime OpenTime { get; set; }

        public DateTime? CloseTime { get; set; }

        public TypeOfTrade TypeOfTrade { get; set; }

        public OpenClose OpenClose { get; set; }
    }
}