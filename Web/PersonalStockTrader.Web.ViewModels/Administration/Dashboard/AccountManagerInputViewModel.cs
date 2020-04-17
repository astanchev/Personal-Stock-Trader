namespace PersonalStockTrader.Web.ViewModels.Administration.Dashboard
{
    using System.ComponentModel.DataAnnotations;

    using PersonalStockTrader.Data.Models;
    using PersonalStockTrader.Services.Mapping;

    public class AccountManagerInputViewModel : IMapTo<ApplicationUser>, IMapFrom<ApplicationUser>
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "The {0} must have only letters and numbers.")]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}


