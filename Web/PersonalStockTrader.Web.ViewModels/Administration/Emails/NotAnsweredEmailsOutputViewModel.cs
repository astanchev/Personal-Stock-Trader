namespace PersonalStockTrader.Web.ViewModels.Administration.Emails
{
    using System;

    using PersonalStockTrader.Data.Models;
    using PersonalStockTrader.Services.Mapping;

    public class NotAnsweredEmailsOutputViewModel : IMapFrom<ContactFormEntry>, IMapTo<ContactFormEntry>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Subject { get; set; }

        public string Content { get; set; }

        public bool Answered { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
