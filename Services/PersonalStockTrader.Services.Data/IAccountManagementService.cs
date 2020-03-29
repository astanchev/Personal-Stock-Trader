namespace PersonalStockTrader.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using PersonalStockTrader.Web.ViewModels.AccountManagement.ManageClients;
    using PersonalStockTrader.Web.ViewModels.AccountManagement.NewClients;

    public interface IAccountManagementService
    {
        IEnumerable<ConfirmedAccountsViewModel> GetAllConfirmedAccounts();

        Task<IEnumerable<NotConfirmedClientsViewModel>> GetAllNotConfirmedClientsAsync();

        Task<ClientToBeConfirmedViewModel> GetClientToBeConfirmedByIdAsync(string userId);

        Task<ClientToBeManagedViewModel> GetClientToBeManagedByAccountIdAsync(int accountId);

        Task ConfirmUserAccountAsync(ClientToBeConfirmedViewModel user);

        Task DeleteUserAccountAsync(int accountId);

        Task RestoreUserAccountAsync(int accountId);

        Task DeleteUserAsync(string userId);

        Task UpdateUserAccountAsync(string userId, decimal balance, decimal tradeFee, decimal monthlyFee);

        IDictionary<DateTime, decimal> GetPaidTradeFeesLast7Days();

        IDictionary<string, decimal> GetPaidMonthlyFeesLast6Months();

        IDictionary<DateTime, decimal> GetAllPaidFeesLast90Days();

        IDictionary<DateTime, int> GetAllNewUsersLast90Days();
    }
}
