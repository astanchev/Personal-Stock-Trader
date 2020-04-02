namespace PersonalStockTrader.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using PersonalStockTrader.Data.Models;
    using PersonalStockTrader.Web.ViewModels.Hub;
    using PersonalStockTrader.Web.ViewModels.User.TradePlatform;

    public interface IStockService
    {
        Task<string> GetLastPrice(string ticker);

        Task<DateTime> GetLastUpdatedTime(string ticker);

        Task<PriceTimeViewModel> GetLastPriceAndTime(string ticker);

        Task<CheckResult> GetUpdate(string lastData, string ticker);

        Task ImportData(string jsonString, string ticker);

        Task CreateStockAsync(string ticker, string name, string intervalName);

        Task<IList<PriceTimeViewModel>> GetPricesLast300Minutes(string ticker);
    }
}
