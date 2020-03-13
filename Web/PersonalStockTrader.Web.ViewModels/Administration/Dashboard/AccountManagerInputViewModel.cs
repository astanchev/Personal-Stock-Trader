namespace PersonalStockTrader.Web.ViewModels.Administration.Dashboard
{
    using PersonalStockTrader.Data.Models;
    using PersonalStockTrader.Services.Mapping;

    public class AccountManagerInputViewModel : IMapTo<ApplicationUser>, IMapFrom<ApplicationUser>
    {
        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }
}