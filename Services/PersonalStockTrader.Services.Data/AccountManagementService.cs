namespace PersonalStockTrader.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using PersonalStockTrader.Common;
    using PersonalStockTrader.Data.Common.Repositories;
    using PersonalStockTrader.Data.Models;
    using PersonalStockTrader.Web.ViewModels.AccountManagement.ManageClients;
    using PersonalStockTrader.Web.ViewModels.AccountManagement.NewClients;

    public class AccountManagementService : IAccountManagementService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IDeletableEntityRepository<ApplicationUser> userRepository;
        private readonly IDeletableEntityRepository<Account> accountRepository;

        public AccountManagementService(UserManager<ApplicationUser> userManager, IDeletableEntityRepository<ApplicationUser> userRepository, IDeletableEntityRepository<Account> accountRepository)
        {
            this.userManager = userManager;
            this.userRepository = userRepository;
            this.accountRepository = accountRepository;
        }

        public async Task<IEnumerable<ConfirmedClientsViewModel>> GetAllConfirmedClientsAsync()
        {
            return (await this.userManager
                    .GetUsersInRoleAsync(GlobalConstants.UserRoleName))
                .Where(u => u.Account.Confirmed)
                .Select(u => new ConfirmedClientsViewModel
                {
                    UserId = u.Id,
                    Username = u.UserName,
                    Email = u.Email,
                    AccountId = u.Account.Id,
                    Balance = u.Account.Balance,
                    Condition = u.Account.IsDeleted,
                })
                .ToList();

        }

        public async Task<IEnumerable<NotConfirmedClientsViewModel>> GetAllNotConfirmedClientsAsync()
        {
            return (await this.userManager
                    .GetUsersInRoleAsync(GlobalConstants.UserRoleName))
                .Where(u => u.Account.Confirmed == false && u.IsDeleted == false)
                .Select(u => new NotConfirmedClientsViewModel
                {
                    UserId = u.Id,
                    Username = u.UserName,
                    Email = u.Email,
                    AccountId = u.Account.Id,
                    StartingBalance = u.StartBalance,
                })
                .ToList();
        }

        public async Task<ClientToBeConfirmedViewModel> GetClientToBeConfirmedByIdAsync(string userId)
        {
            return await this.userRepository
                .AllWithDeleted()
                .Where(u => u.Id == userId)
                .Select(u => new ClientToBeConfirmedViewModel
                {
                    UserId = u.Id,
                    Username = u.UserName,
                    Email = u.Email,
                    AccountId = u.Account.Id,
                    Balance = u.StartBalance,
                    TradeFee = u.Account.TradeFee,
                    MonthlyFee = u.Account.MonthlyFee,
                    Notes = u.Account.Notes,
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ClientToBeManagedViewModel> GetClientToBeManagedByIdAsync(string userId)
        {
            return await this.userRepository
                .AllWithDeleted()
                .Where(u => u.Id == userId)
                .Select(u => new ClientToBeManagedViewModel
                {
                    UserId = u.Id,
                    Username = u.UserName,
                    Email = u.Email,
                    AccountId = u.Account.Id,
                    Balance = u.Account.Balance,
                    TradeFee = u.Account.TradeFee,
                    MonthlyFee = u.Account.MonthlyFee,
                })
                .FirstOrDefaultAsync();
        }

        public async Task ConfirmUserAccountAsync(ClientToBeConfirmedViewModel user)
        {
            var userToBeConfirmed = await this.userRepository.GetByIdWithDeletedAsync(user.UserId);

            userToBeConfirmed.Account = new Account
            {
                Balance = user.Balance,
                MonthlyFee = user.MonthlyFee,
                TradeFee = user.TradeFee,
                Confirmed = true,
                Notes = user.Notes,
            };

            this.userRepository.Update(userToBeConfirmed);

            await this.userRepository.SaveChangesAsync();
        }

        public async Task DeleteUserAccountAsync(string userId)
        {
            var user = await this.userRepository.GetByIdWithDeletedAsync(userId);

            var account = await this.accountRepository.GetByIdWithDeletedAsync(user.Account.Id);

            this.accountRepository.Delete(account);

            this.userRepository.Update(user);

            await this.userRepository.SaveChangesAsync();
        }

        public async Task RestoreUserAccountAsync(string userId)
        {
            var user = await this.userRepository.GetByIdWithDeletedAsync(userId);

            var account = await this.accountRepository.GetByIdWithDeletedAsync(user.Account.Id);

            this.accountRepository.Undelete(account);

            this.userRepository.Update(user);

            await this.userRepository.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(string userId)
        {
            var user = await this.userRepository.GetByIdWithDeletedAsync(userId);

            this.userRepository.HardDelete(user);

            await this.userRepository.SaveChangesAsync();
        }

        public async Task UpdateUserAccountAsync(string userId, decimal balance, decimal tradeFee, decimal monthlyFee)
        {
            var userToBeUpdated = await this.userRepository.GetByIdWithDeletedAsync(userId);

            userToBeUpdated.Account.Balance = balance;
            userToBeUpdated.Account.TradeFee = tradeFee;
            userToBeUpdated.Account.MonthlyFee = monthlyFee;

            this.userRepository.Update(userToBeUpdated);

            await this.userRepository.SaveChangesAsync();
        }
    }
}