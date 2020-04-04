namespace PersonalStockTrader.Web.ViewModels.User.TradePlatform
{
    public class TradeSharesInputViewModel
    {
        public int AccountId { get; set; }

        public int PositionId { get; set; }

        public int Quantity { get; set; }

        public bool IsBuy { get; set; }
    }
}