namespace PersonalStockTrader.Web.ViewModels.AccountManagement.ManageClients
{
    public class ConfirmedClientsViewModel
    {
        public string UserId { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public int AccountId { get; set; }

        public decimal Balance { get; set; }

        // If account is deleted or still trading
        public bool Condition { get; set; }
    }
}