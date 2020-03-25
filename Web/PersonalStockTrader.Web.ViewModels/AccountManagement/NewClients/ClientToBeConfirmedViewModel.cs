namespace PersonalStockTrader.Web.ViewModels.AccountManagement.NewClients
{
    using System.ComponentModel.DataAnnotations;

    public class ClientToBeConfirmedViewModel
    {
        public string UserId { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        [Range(typeof(decimal), "1000.00", "79228162514264337593543950335", ErrorMessage = "Value for {0} must be at least {1} USD.")]
        public decimal Balance { get; set; }

        [Range(typeof(decimal), "0.01", "100.00", ErrorMessage = "Value for {0} must be at least {1} USD.")]
        public decimal TradeFee { get; set; }

        [Range(typeof(decimal), "0.01", "100.00", ErrorMessage = "Value for {0} must be at least {1} USD.")]
        public decimal MonthlyFee { get; set; }

        [StringLength(2000)]
        public string Notes { get; set; }
    }
}
