namespace PersonalStockTrader.Web.ViewModels.Administration.Dashboard
{
    using PersonalStockTrader.Data.Models;
    using PersonalStockTrader.Services.Mapping;

    public class AccountManagerOutputViewModel : IMapTo<ApplicationUser>, IMapFrom<ApplicationUser>
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }
    }
}