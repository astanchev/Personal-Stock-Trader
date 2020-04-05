namespace PersonalStockTrader.Web.ViewModels.User.TradeShares
{
    public class TradeSharesResultModel
    {
        public int PositionId { get; set; }

        public int Quantity { get; set; }

        public decimal OpenPrice { get; set; }

        public decimal Balance { get; set; }

        public bool IsBuy { get; set; }
    }
}