namespace PersonalStockTrader.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using PersonalStockTrader.Web.ViewModels.Administration.Emails;
    using PersonalStockTrader.Web.ViewModels.Contact;

    public interface IContactFormService
    {
        Task<bool> AddAsync(ContactFormViewModel input);

        Task<bool> MarkAsAnsweredAsync(int emailId);

        Task<NotAnsweredEmailsOutputViewModel> GetByIdAsync(int emailId);

        IEnumerable<NotAnsweredEmailsOutputViewModel> GetAllNotAnswered();

        Task<CountEmailsOutputViewModel> GetAllCountAsync();

        IDictionary<DateTime, int> GetNotAnsweredLast10Days();
    }
}
