namespace PersonalStockTrader.Web.ViewModels.User.TradeHistory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class TradeHistoryViewModel
    {
        public string Username { get; set; }

        public string Email { get; set; }

        public int AccountId { get; set; }

        public decimal StartBalance { get; set; }

        public decimal Balance { get; set; }

        public IEnumerable<HistoryPositionViewModel> Positions { get; set; }

        public DateTime StartDate { get; set; }
    }
}