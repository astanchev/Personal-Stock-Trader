namespace PersonalStockTrader.Services.Data
{
    using System.Threading.Tasks;

    using PersonalStockTrader.Common;
    using PersonalStockTrader.Data.Common.Repositories;
    using PersonalStockTrader.Data.Models;
    using PersonalStockTrader.Web.ViewModels.Contact;

    public class ContactFormService : IContactFormService
    {
        private readonly IRepository<ContactFormEntry> contactRepository;

        public ContactFormService(IRepository<ContactFormEntry> contactRepository)
        {
            this.contactRepository = contactRepository;
        }

        public async Task AddAsync(ContactFormViewModel input)
        {
            var contactFormEntry = new ContactFormEntry
            {
                Name = input.Name,
                Email = input.Email,
                Subject = input.Subject ?? GlobalConstants.ConstSubject,
                Content = input.Content,
            };

            await this.contactRepository.AddAsync(contactFormEntry);
            await this.contactRepository.SaveChangesAsync();
        }
    }
}