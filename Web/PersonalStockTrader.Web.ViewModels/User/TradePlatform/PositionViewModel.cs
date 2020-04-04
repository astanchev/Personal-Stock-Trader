namespace PersonalStockTrader.Web.ViewModels.User.TradePlatform
{
    public class PositionViewModel
    {
        public int PositionId { get; set; }

        public int Quantity { get; set; }

        public bool Direction { get; set; }

        public decimal OpenPrice { get; set; }
    }
}