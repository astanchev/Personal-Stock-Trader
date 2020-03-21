namespace PersonalStockTrader.Services.Data
{
    using System.Threading.Tasks;

    using PersonalStockTrader.Web.ViewModels.Contact;

    public interface IContactFormService
    {
        Task AddAsync(ContactFormViewModel input);
    }
}