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

        Task ConfirmUserAccountAsync(ClientToBeConfirmedViewModel user);

        Task DeleteUserAccountAsync(string userId);

        Task RestoreUserAccountAsync(string userId);

        Task DeleteUserAsync(string userId);

        Task UpdateUserAccountAsync(string userId, decimal balance, decimal tradeFee, decimal monthlyFee);
    }
}