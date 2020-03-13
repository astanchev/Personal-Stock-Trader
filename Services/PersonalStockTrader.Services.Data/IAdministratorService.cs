namespace PersonalStockTrader.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using PersonalStockTrader.Web.ViewModels.Administration.Dashboard;

    public interface IAdministratorService
    {
        Task<IEnumerable<AccountManagerOutputViewModel>> GetAllAccountManagersAsync();

        Task<bool> RemoveAccountManagerAsync(string userId);

        Task<bool> CreateAccountManagerAsync(AccountManagerInputViewModel accountManager);
    }
}