namespace PersonalStockTrader.Web.ViewModels.User.TradePlatform
{
    using System.Collections.Generic;

    public class DisplayViewModel
    {
        public IEnumerable<PriceTimeViewModel> PricesAndTimes { get; set; }

        public string LastPrice { get; set; }

        public string LastDateTime { get; set; }
    }
}