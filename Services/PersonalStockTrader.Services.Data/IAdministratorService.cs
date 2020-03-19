namespace PersonalStockTrader.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using PersonalStockTrader.Web.ViewModels.Administration.Dashboard;

    public interface IAdministratorService
    {
        Task<IList<AccountManagerOutputViewModel>> GetAllAccountManagersAsync();

        Task<AccountManagerOutputViewModel> GetAccountManagersByIdAsync(string userId);

        Task<bool> RemoveAccountManagerAsync(string userId);

        Task<bool> CreateAccountManagerAsync(AccountManagerInputViewModel accountManager);

        Task<bool> UpdateAccountManagerAsync(AccountManagerOutputViewModel accountManager);
    }
}