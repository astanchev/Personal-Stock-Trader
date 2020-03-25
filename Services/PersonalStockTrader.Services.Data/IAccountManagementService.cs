namespace PersonalStockTrader.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using PersonalStockTrader.Web.ViewModels.AccountManagement.ManageClients;
    using PersonalStockTrader.Web.ViewModels.AccountManagement.NewClients;

    public interface IAccountManagementService
    {
        IEnumerable<ConfirmedClientsViewModel> GetAllConfirmedClients();

        Task<IEnumerable<NotConfirmedClientsViewModel>> GetAllNotConfirmedClientsAsync();

        Task<ClientToBeConfirmedViewModel> GetClientToBeConfirmedByIdAsync(string userId);

        Task<ClientToBeManagedViewModel> GetClientToBeManagedByIdAsync(string userId);

        Task ConfirmUserAccountAsync(ClientToBeConfirmedViewModel user);

        Task DeleteUserAccountAsync(string userId);

        Task RestoreUserAccountAsync(string userId);

        Task DeleteUserAsync(string userId);

        Task UpdateUserAccountAsync(string userId, decimal balance, decimal tradeFee, decimal monthlyFee);

        IDictionary<string, decimal> GetPaidTradeFeesLast7Days();

        IDictionary<string, decimal> GetPaidMonthlyFeesLast6Months();

        IDictionary<string, decimal> GetAllPaidFeesLast90Days();
    }
}
