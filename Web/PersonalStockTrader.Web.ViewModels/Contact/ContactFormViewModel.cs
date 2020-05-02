namespace PersonalStockTrader.Web.ViewModels.Contact
{
    using System.ComponentModel.DataAnnotations;

    using PersonalStockTrader.Common;
    using PersonalStockTrader.Data.Models;
    using PersonalStockTrader.Services.Mapping;

    public class ContactFormViewModel : IMapFrom<ContactFormEntry>, IMapTo<ContactFormEntry>
    {
        [Required]
        [StringLength(20, ErrorMessage = GlobalConstants.TextError, MinimumLength = 3)]
        [Display(Name = "Full name")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [StringLength(20, ErrorMessage = GlobalConstants.ShortTextError)]
        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Required]
        [StringLength(10000, ErrorMessage = GlobalConstants.TextError, MinimumLength = 20)]
        [Display(Name = "Content")]
        public string Content { get; set; }
    }
}
