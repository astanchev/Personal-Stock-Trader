namespace PersonalStockTrader.Web.ViewModels.AccountManagement.ManageClients
{
    public class ClientToBeManagedViewModel
    {
        public string UserId { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public int AccountId { get; set; }

        public decimal Balance { get; set; }

        public decimal TradeFee { get; set; }

        public decimal MonthlyFee { get; set; }
    }
}