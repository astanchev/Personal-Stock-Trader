namespace PersonalStockTrader.Data.Seeding
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore.Internal;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using PersonalStockTrader.Common;
    using PersonalStockTrader.Services.Data;

    public class StockSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Stocks.Any())
            {
                return;
            }

            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var stockService = serviceProvider.GetRequiredService<IStockService>();

            await stockService.CreateStockAsync(GlobalConstants.StockTicker, GlobalConstants.StockName, GlobalConstants.StockInterval);
        }
    }
}