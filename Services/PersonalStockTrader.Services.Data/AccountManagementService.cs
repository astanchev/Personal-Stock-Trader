namespace PersonalStockTrader.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using PersonalStockTrader.Web.ViewModels.AccountManagement.ManageClients;
    using PersonalStockTrader.Web.ViewModels.AccountManagement.NewClients;

    public class AccountManagementService : IAccountManagementService
    {
        public Task<IEnumerable<ConfirmedClientsViewModel>> GetAllConfirmedClientsAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<NotConfirmedClientsViewModel>> GetAllNotConfirmedClientsAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<ClientToBeConfirmedViewModel> GetClientToBeConfirmedByIdAsync(string userId)
        {
            throw new System.NotImplementedException();
        }

        public Task<ClientToBeManagedViewModel> GetClientToBeManagedByIdAsync(string userId)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> ConfirmUserAccountAsync(string userId)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> DeleteUserAccountAsync(string userId)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> RestoreUserAccountAsync(string userId)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> DeleteUserAsync(string userId)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> UpdateUserAccountAsync(ClientToBeManagedViewModel user)
        {
            throw new System.NotImplementedException();
        }
    }
}