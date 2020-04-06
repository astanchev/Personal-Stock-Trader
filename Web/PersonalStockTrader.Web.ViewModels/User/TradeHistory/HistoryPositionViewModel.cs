namespace PersonalStockTrader.Web.ViewModels.User.TradeHistory
{
    using System;

    public class HistoryPositionViewModel
    {
        public string Ticker { get; set; }

        public DateTime OpenDate { get; set; }

        public int Quantity { get; set; }

        public string Direction { get; set; }

        public decimal OpenPrice { get; set; }

        public decimal ClosePrice { get; set; }

        public decimal Profit { get; set; }
    }
}