namespace PersonalStockTrader.Web.ViewModels.Administration.Emails
{
    using System.ComponentModel.DataAnnotations;

    using PersonalStockTrader.Common;

    public class SendReplayViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Subject { get; set; }

        public string Content { get; set; }

        public bool Answered { get; set; }

        [Required]
        [StringLength(10000, ErrorMessage = GlobalConstants.TextError, MinimumLength = 20)]
        [Display(Name = "Answer")]
        public string Answer { get; set; }
    }
}
