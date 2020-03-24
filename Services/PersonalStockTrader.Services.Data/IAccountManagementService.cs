namespace PersonalStockTrader.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using PersonalStockTrader.Web.ViewModels.AccountManagement.ManageClients;
    using PersonalStockTrader.Web.ViewModels.AccountManagement.NewClients;

    public interface IAccountManagementService
    {
        Task<IEnumerable<ConfirmedClientsViewModel>> GetAllConfirmedClientsAsync();

        Task<IEnumerable<NotConfirmedClientsViewModel>> GetAllNotConfirmedClientsAsync();

        Task<ClientToBeConfirmedViewModel> GetClientToBeConfirmedByIdAsync(string userId);

        Task<ClientToBeManagedViewModel> GetClientToBeManagedByIdAsync(string userId);

        Task<bool> ConfirmUserAccountAsync(string userId);

        Task<bool> DeleteUserAccountAsync(string userId);

        Task<bool> RestoreUserAccountAsync(string userId);

        Task<bool> DeleteUserAsync(string userId);

        Task<bool> UpdateUserAccountAsync(ClientToBeManagedViewModel user);
    }
}