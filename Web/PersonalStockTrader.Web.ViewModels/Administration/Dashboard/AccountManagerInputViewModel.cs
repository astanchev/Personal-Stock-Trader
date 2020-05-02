namespace PersonalStockTrader.Web.ViewModels.Administration.Dashboard
{
    using System.ComponentModel.DataAnnotations;
    using PersonalStockTrader.Common;
    using PersonalStockTrader.Data.Models;
    using PersonalStockTrader.Services.Mapping;

    public class AccountManagerInputViewModel : IMapTo<ApplicationUser>, IMapFrom<ApplicationUser>
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = GlobalConstants.UsernameErrorRegex)]
        [StringLength(20, ErrorMessage = GlobalConstants.TextError, MinimumLength = 3)]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = GlobalConstants.TextError, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}


