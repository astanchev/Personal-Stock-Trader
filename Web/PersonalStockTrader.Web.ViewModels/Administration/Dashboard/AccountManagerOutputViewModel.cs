namespace PersonalStockTrader.Web.ViewModels.Administration.Dashboard
{
    using System.ComponentModel.DataAnnotations;

    using PersonalStockTrader.Data.Models;
    using PersonalStockTrader.Services.Mapping;

    public class AccountManagerOutputViewModel : IMapTo<ApplicationUser>, IMapFrom<ApplicationUser>
    {
        public string UserId { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}