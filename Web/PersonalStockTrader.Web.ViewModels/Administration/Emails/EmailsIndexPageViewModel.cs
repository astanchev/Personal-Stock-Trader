namespace PersonalStockTrader.Web.ViewModels.Administration.Emails
{
    using System;
    using System.Collections.Generic;

    public class EmailsIndexPageViewModel
    {
        public IEnumerable<NotAnsweredEmailsOutputViewModel> NotAnsweredEmails { get; set; }

        public int CountAnsweredEmails { get; set; }

        public int CountNotAnsweredEmails { get; set; }

        public IDictionary<DateTime, int> NotAnsweredLast10Days { get; set; }
    }
}