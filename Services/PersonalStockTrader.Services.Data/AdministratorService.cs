namespace PersonalStockTrader.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using PersonalStockTrader.Common;
    using PersonalStockTrader.Data.Common.Repositories;
    using PersonalStockTrader.Data.Models;
    using PersonalStockTrader.Services.Mapping;
    using PersonalStockTrader.Web.ViewModels.Administration.Dashboard;

    public class AdministratorService : IAdministratorService
    {
        private const string InvalidUserIdErrorMessage = "User with ID: {0} does not exist.";

        private readonly IConfiguration configuration;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IDeletableEntityRepository<ApplicationUser> userRepository;

        public AdministratorService(IConfiguration configuration, UserManager<ApplicationUser> userManager, IDeletableEntityRepository<ApplicationUser> userRepository)
        {
            this.configuration = configuration;
            this.userManager = userManager;
            this.userRepository = userRepository;
        }

        public async Task<IList<AccountManagerOutputViewModel>> GetAllAccountManagersAsync()
        {
            return (await this.userManager
                .GetUsersInRoleAsync(GlobalConstants.AccountManagerRoleName))
                .Where(x => x.IsDeleted == false)
                .Select(x => new AccountManagerOutputViewModel
                {
                    UserId = x.Id,
                    Username = x.UserName,
                    Email = x.Email,
                })
                .ToList();
        }

        public async Task<AccountManagerOutputViewModel> GetAccountManagersByIdAsync(string userId)
        {
            var accountManager = await this.userRepository
                .GetByIdWithDeletedAsync(userId);

            return new AccountManagerOutputViewModel
            {
                UserId = accountManager.Id,
                Username = accountManager.UserName,
                Email = accountManager.Email,
            };
        }

        public async Task<bool> RemoveAccountManagerAsync(string userId)
        {
            var accountManager = await this.userRepository.GetByIdWithDeletedAsync(userId);

            if (accountManager == null)
            {
                throw new ArgumentNullException(
                    string.Format(InvalidUserIdErrorMessage, userId));
            }

            var result = await this.userManager.RemoveFromRoleAsync(
                accountManager,
                GlobalConstants.AccountManagerRoleName);

            if (result.Succeeded)
            {
                accountManager.IsDeleted = true;
                accountManager.DeletedOn = DateTime.Now;
            }

            await this.userRepository.SaveChangesAsync();

            return result.Succeeded;
        }

        public async Task<bool> CreateAccountManagerAsync(AccountManagerInputViewModel accountManager)
        {
            var user = new ApplicationUser
            {
                Email = accountManager.Email,
                EmailConfirmed = true,
                UserName = accountManager.Username,
            };

            var result = await this.userManager.CreateAsync(
                user,
                accountManager.Password);

            if (result.Succeeded)
            {
                result = await this.userManager.AddToRoleAsync(
                    user,
                    GlobalConstants.AccountManagerRoleName);
            }

            await this.userRepository.SaveChangesAsync();

            return result.Succeeded;
        }
    }
}