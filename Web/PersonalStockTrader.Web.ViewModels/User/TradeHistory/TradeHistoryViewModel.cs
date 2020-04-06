namespace PersonalStockTrader.Web.ViewModels.User.TradeHistory
{
    using System.Collections.Generic;

    public class TradeHistoryViewModel
    {
        public string Username { get; set; }

        public string Email { get; set; }

        public int AccountId { get; set; }

        public decimal StartBalance { get; set; }

        public decimal Balance { get; set; }

        public IEnumerable<HistoryPositionViewModel> Positions { get; set; }
    }
}