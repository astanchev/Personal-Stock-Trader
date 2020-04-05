namespace PersonalStockTrader.Web.ViewModels.User.TradePlatform
{
    using System.Collections.Generic;

    public class DisplayViewModel
    {
        public string Ticker { get; set; }

        public IList<PriceTimeViewModel> PricesAndTimes { get; set; }

        public string LastPrice { get; set; }

        public string LastDateTime { get; set; }

        public decimal Balance { get; set; }

        public decimal CurrentProfit { get; set; }

        public int AccountId { get; set; }

        public PositionViewModel Position { get; set; }
    }
}