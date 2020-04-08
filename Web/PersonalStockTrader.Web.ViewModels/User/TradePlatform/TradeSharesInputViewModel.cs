namespace PersonalStockTrader.Web.ViewModels.User.TradePlatform
{
    public class TradeSharesInputViewModel
    {
        public string AccountId { get; set; }

        public string PositionId { get; set; }

        public string Balance { get; set; }

        public string CurrentPrice { get; set; }

        public string Quantity { get; set; }

        public bool IsBuy { get; set; }
    }
}