namespace PersonalStockTrader.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using PersonalStockTrader.Data.Common.Models;

    public class ContactFormEntry: BaseModel<int>
    {
        public ContactFormEntry()
        {
            this.Answered = false;
        }

        [Required]
        [StringLength(30, MinimumLength = 1)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Subject { get; set; }

        [Required]
        [StringLength(10000, MinimumLength = 20)]
        public string Content { get; set; }

        public bool Answered { get; set; }
    }
}