namespace PersonalStockTrader.Services.Data
{
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using PersonalStockTrader.Data.Common.Repositories;
    using PersonalStockTrader.Data.Models;

    public class UserService : IUserService
    {
        private readonly IDeletableEntityRepository<Account> accountRepository;

        public UserService(IDeletableEntityRepository<Account> accountRepository)
        {
            this.accountRepository = accountRepository;
        }

        public async Task<decimal> GetUserBalanceAsync(string userId)
        {
            return await this.accountRepository
                .All()
                .Where(a => a.UserId == userId)
                .Select(a => a.Balance)
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetUserAccountIdAsync(string userId)
        {
            return await this.accountRepository
                .All()
                .Where(a => a.UserId == userId)
                .Select(a => a.Id)
                .FirstOrDefaultAsync();
        }
    }
}