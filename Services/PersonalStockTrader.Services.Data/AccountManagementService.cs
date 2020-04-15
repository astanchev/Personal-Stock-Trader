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
    using PersonalStockTrader.Services.Mapping;
    using PersonalStockTrader.Web.ViewModels.AccountManagement.ManageClients;
    using PersonalStockTrader.Web.ViewModels.AccountManagement.NewClients;

    public class AccountManagementService : IAccountManagementService
    {
        private const decimal MonthlyFee = 50.0M;
        private const decimal TradeFee = 10.0M;

        private readonly UserManager<ApplicationUser> userManager;
        private readonly IDeletableEntityRepository<ApplicationUser> userRepository;
        private readonly IDeletableEntityRepository<Account> accountRepository;
        private readonly IPositionsService positionsService;
        private readonly IRepository<FeePayment> feePaymentsRepository;

        public AccountManagementService(UserManager<ApplicationUser> userManager, IDeletableEntityRepository<ApplicationUser> userRepository, IDeletableEntityRepository<Account> accountRepository, IPositionsService positionsService, IRepository<FeePayment> feePaymentsRepository)
        {
            this.userManager = userManager;
            this.userRepository = userRepository;
            this.accountRepository = accountRepository;
            this.positionsService = positionsService;
            this.feePaymentsRepository = feePaymentsRepository;
        }

        public IEnumerable<ConfirmedAccountsViewModel> GetAllConfirmedAccounts()
        {
            var accounts = this.accountRepository
                .AllWithDeleted()
                .Where(a => !a.User.IsDeleted)
                .Select(a => new ConfirmedAccountsViewModel
                {
                    UserId = a.UserId,
                    Username = a.User.UserName,
                    Email = a.User.Email,
                    AccountId = a.Id,
                    Balance = a.Balance,
                    Condition = a.IsDeleted,
                })
                .ToList();

            return accounts;
        }

        public async Task<IEnumerable<NotConfirmedClientsViewModel>> GetAllNotConfirmedClientsAsync()
        {
            var usersNotConfirmed = await this.userManager
                .GetUsersInRoleAsync(GlobalConstants.NotConfirmedUserRoleName);

            return usersNotConfirmed
                .Where(u => u.IsDeleted == false && u.Account == null)
                .Select(u => new NotConfirmedClientsViewModel
                {
                    UserId = u.Id,
                    Username = u.UserName,
                    Email = u.Email,
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
                    Balance = u.StartBalance,
                    TradeFee = TradeFee,
                    MonthlyFee = MonthlyFee,
                    Notes = string.Empty,
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ClientToBeManagedViewModel> GetClientToBeManagedByAccountIdAsync(int accountId)
        {
            return await this.accountRepository
                .AllWithDeleted()
                .Where(a => a.Id == accountId)
                .Select(a => new ClientToBeManagedViewModel
                {
                    UserId = a.UserId,
                    Username = a.User.UserName,
                    Email = a.User.Email,
                    AccountId = a.Id,
                    AccountIsDeleted = a.IsDeleted,
                    Balance = a.Balance,
                    TradeFee = a.TradeFee,
                    MonthlyFee = a.MonthlyFee,
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
                Positions = new List<Position>(),
            };

            var result = await this.userManager.RemoveFromRoleAsync(userToBeConfirmed, GlobalConstants.NotConfirmedUserRoleName);

            if (result.Succeeded)
            {
                await this.userManager.AddToRoleAsync(userToBeConfirmed, GlobalConstants.ConfirmedUserRoleName);
            }

            this.userRepository.Update(userToBeConfirmed);

            await this.userRepository.SaveChangesAsync();
        }

        public async Task DeleteUserAccountAsync(string userId, int accountId)
        {
            var account = await this.accountRepository.GetByIdWithDeletedAsync(accountId);

            await this.positionsService.ClosePosition(accountId);

            this.accountRepository.Delete(account);

            await this.accountRepository.SaveChangesAsync();

            var user = await this.userRepository.GetByIdWithDeletedAsync(userId);

            await this.userManager.RemoveFromRoleAsync(user, GlobalConstants.ConfirmedUserRoleName);

            await this.userManager.AddToRoleAsync(user, GlobalConstants.NotConfirmedUserRoleName);
        }

        public async Task RestoreUserAccountAsync(string userId, int accountId)
        {
            var account = await this.accountRepository.GetByIdWithDeletedAsync(accountId);

            this.accountRepository.Undelete(account);

            await this.accountRepository.SaveChangesAsync();

            var user = await this.userRepository.GetByIdWithDeletedAsync(userId);

            await this.userManager.RemoveFromRoleAsync(user, GlobalConstants.NotConfirmedUserRoleName);

            await this.userManager.AddToRoleAsync(user, GlobalConstants.ConfirmedUserRoleName);
        }

        public async Task UpdateUserAccountAsync(string userId, decimal balance, decimal tradeFee, decimal monthlyFee)
        {
            var accountToBeUpdated = await this.accountRepository
                .AllWithDeleted()
                .FirstOrDefaultAsync(a => a.UserId == userId);

            accountToBeUpdated.Balance = balance;
            accountToBeUpdated.TradeFee = tradeFee;
            accountToBeUpdated.MonthlyFee = monthlyFee;

            this.accountRepository.Update(accountToBeUpdated);

            await this.accountRepository.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(string userId, int accountId)
        {
            var user = await this.userRepository.GetByIdWithDeletedAsync(userId);

            this.userRepository.Delete(user);

            await this.userRepository.SaveChangesAsync();

            var account = await this.accountRepository.GetByIdWithDeletedAsync(accountId);

            this.accountRepository.Delete(account);

            await this.accountRepository.SaveChangesAsync();
        }

        public IDictionary<DateTime, decimal> GetPaidTradeFeesLast7Days()
        {
            var result = this.CreateEmptyResultNDays(7);

            var tradeFees = this.feePaymentsRepository
                .All()
                .Where(f => f.TypeFee == TypeFee.TradeFee && f.CreatedOn > DateTime.Now.Date.AddDays(-7))
                .GroupBy(c => c.CreatedOn.Date)
                .Select(c => new
                {
                    Day = (DateTime)c.Key,
                    Value = c.Sum(x => x.Amount),
                });

            foreach (var tradeFee in tradeFees)
            {
                string date = tradeFee.Day.Day.ToString() + "-" + tradeFee.Day.Month.ToString() + "-" + tradeFee.Day.Year.ToString();

                if (result.ContainsKey(tradeFee.Day.Date))
                {
                    result[tradeFee.Day.Date] = tradeFee.Value;
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

        public IDictionary<DateTime, decimal> GetAllPaidFeesLast90Days()
        {
            var result = this.CreateEmptyResultNDays(90);

            var tradeFees = this.feePaymentsRepository
                .All()
                .Where(f => f.CreatedOn > DateTime.Now.Date.AddDays(-89))
                .GroupBy(c => c.CreatedOn.Date)
                .Select(c => new
                {
                    Day = (DateTime)c.Key,
                    Value = c.Sum(x => x.Amount),
                });

            foreach (var tradeFee in tradeFees)
            {
                if (result.ContainsKey(tradeFee.Day.Date))
                {
                    result[tradeFee.Day.Date] = tradeFee.Value;
                }
            }

            return result;
        }

        public IDictionary<DateTime, int> GetAllNewUsersLast90Days()
        {
            var result = this.CreateEmptyIntResultNDays(90);

            var usersCountByDays = this.userManager
                .Users
                .Where(u => u.CreatedOn > DateTime.Now.Date.AddDays(-89))
                            .GroupBy(c => c.CreatedOn.Date)
                            .Select(u => new
                            {
                                Day = (DateTime)u.Key,
                                Value = u.Count(),
                            })
                .ToList();

            foreach (var day in usersCountByDays)
            {
                if (result.ContainsKey(day.Day.Date))
                {
                    result[day.Day.Date] = day.Value;
                }
            }

            return result;
        }

        private IDictionary<DateTime, decimal> CreateEmptyResultNDays(int days)
        {
            var result = new Dictionary<DateTime, decimal>();

            var startDate = DateTime.UtcNow.Date.AddDays(-(days - 1));

            for (DateTime i = startDate; i <= DateTime.UtcNow.Date; i = i.AddDays(1))
            {
                result.Add(i.Date, 0M);
            }

            return result;
        }

        private IDictionary<DateTime, int> CreateEmptyIntResultNDays(int days)
        {
            var result = new Dictionary<DateTime, int>();

            var startDate = DateTime.UtcNow.Date.AddDays(-(days - 1));

            for (DateTime i = startDate; i <= DateTime.UtcNow.Date; i = i.AddDays(1))
            {
                result.Add(i.Date, 0);
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
