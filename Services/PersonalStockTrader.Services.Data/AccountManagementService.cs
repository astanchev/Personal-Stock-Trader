namespace PersonalStockTrader.Services.Data
{
    using System;
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
        private readonly IRepository<FeePayment> feePaymentsRepository;

        public AccountManagementService(UserManager<ApplicationUser> userManager, IDeletableEntityRepository<ApplicationUser> userRepository, IDeletableEntityRepository<Account> accountRepository, IRepository<FeePayment> feePaymentsRepository)
        {
            this.userManager = userManager;
            this.userRepository = userRepository;
            this.accountRepository = accountRepository;
            this.feePaymentsRepository = feePaymentsRepository;
        }

        public async Task<IEnumerable<ConfirmedClientsViewModel>> GetAllConfirmedClientsAsync()
        {
            return (await this.userManager
                    .GetUsersInRoleAsync(GlobalConstants.ConfirmedUserRoleName))
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
                    .GetUsersInRoleAsync(GlobalConstants.NotConfirmedUserRoleName))
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

            var result = await this.userManager.RemoveFromRoleAsync(userToBeConfirmed, GlobalConstants.NotConfirmedUserRoleName);

            if (result.Succeeded)
            {
                await this.userManager.AddToRoleAsync(userToBeConfirmed, GlobalConstants.ConfirmedUserRoleName);
            }

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

        public IDictionary<string, decimal> GetPaidTradeFeesLast7Days()
        {
            var result = this.CreateEmptyResultNDays(7);

            var tradeFees = this.feePaymentsRepository
                .All()
                .Where(f => f.TypeFee == TypeFee.TradeFee && f.CreatedOn > DateTime.Now.Date.AddDays(-7))
                .GroupBy(c => c.CreatedOn.Date)
                .Select(c => new
                {
                    Day = c.Key.ToShortDateString(),
                    Value = c.Sum(x => x.Amount),
                });

            foreach (var tradeFee in tradeFees)
            {
                if (result.ContainsKey(tradeFee.Day))
                {
                    result[tradeFee.Day] = tradeFee.Value;
                }
            }

            return result;
        }

        public IDictionary<string, decimal> GetPaidMonthlyFeesLast6Months()
        {
            var result = this.CreateEmptyResultNMonths(6);

            var tradeFees = this.feePaymentsRepository
                .All()
                .Where(f => f.TypeFee == TypeFee.MonthlyCommission && f.CreatedOn > DateTime.Now.Date.AddMonths(-6))
                .GroupBy(c => new
                {
                    Month = c.CreatedOn.Month,
                    Year = c.CreatedOn.Year,
                })
                .Select(c => new
                {
                    Month = c.Key.Month.ToString() + " " + c.Key.Year.ToString(),
                    Value = c.Sum(x => x.Amount),
                });

            foreach (var tradeFee in tradeFees)
            {
                if (result.ContainsKey(tradeFee.Month))
                {
                    result[tradeFee.Month] = tradeFee.Value;
                }
            }

            return result;
        }

        public IDictionary<string, decimal> GetAllPaidFeesLast90Days()
        {
            var result = this.CreateEmptyResultNDays(90);

            var tradeFees = this.feePaymentsRepository
                .All()
                .Where(f => f.CreatedOn > DateTime.Now.Date.AddDays(-89))
                .GroupBy(c => c.CreatedOn.Date)
                .Select(c => new
                {
                    Day = c.Key.ToShortDateString(),
                    Value = c.Sum(x => x.Amount),
                });

            foreach (var tradeFee in tradeFees)
            {
                if (result.ContainsKey(tradeFee.Day))
                {
                    result[tradeFee.Day] = tradeFee.Value;
                }
            }

            return result;
        }

        private IDictionary<string, decimal> CreateEmptyResultNDays(int days)
        {
            var result = new Dictionary<string, decimal>();

            var startDate = DateTime.UtcNow.Date.AddDays(-(days - 1));

            for (DateTime i = startDate; i <= DateTime.UtcNow.Date; i = i.AddDays(1))
            {
                string key = i.ToShortDateString();
                result.Add(key, 0M);
            }

            return result;
        }

        private IDictionary<string, decimal> CreateEmptyResultNMonths(int months)
        {
            var result = new Dictionary<string, decimal>();

            var startDate = DateTime.UtcNow.Date.AddMonths(-(months - 1));

            for (DateTime i = startDate; i <= DateTime.UtcNow.Date; i = i.AddMonths(1))
            {
                string key = i.Month.ToString() + " " + i.Year.ToString();
                result.Add(key, 0M);
            }

            return result;
        }
    }
}
