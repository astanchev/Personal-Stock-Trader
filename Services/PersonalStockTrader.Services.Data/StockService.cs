namespace PersonalStockTrader.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Memory;
    using Newtonsoft.Json;
    using PersonalStockTrader.Data.Common.Repositories;
    using PersonalStockTrader.Data.Models;
    using PersonalStockTrader.Services.Models;
    using PersonalStockTrader.Web.ViewModels.Hub;
    using PersonalStockTrader.Web.ViewModels.User.TradePlatform;

    public class StockService : IStockService
    {
        private readonly IDeletableEntityRepository<Stock> stockRepository;
        private readonly IDeletableEntityRepository<Interval> intervalRepository;
        private readonly IDeletableEntityRepository<DataSet> datasetRepository;
        private readonly IDeletableEntityRepository<MetaData> metadataRepository;
        //private readonly IMemoryCache memoryCache;

        public StockService(IDeletableEntityRepository<Stock> stockRepository, IDeletableEntityRepository<Interval> intervalRepository, IDeletableEntityRepository<DataSet> datasetRepository, IDeletableEntityRepository<MetaData> metadataRepository/*,
            IMemoryCache memoryCache*/)
        {
            this.stockRepository = stockRepository;
            this.intervalRepository = intervalRepository;
            this.datasetRepository = datasetRepository;
            this.metadataRepository = metadataRepository;
            //this.memoryCache = memoryCache;
        }

        public async Task<string> GetLastPrice(string ticker)
        {
            var stockId = await this.GetStockId(ticker);

            var intervalId = await this.GetIntervalId(stockId);

            var price = await this.datasetRepository
                .All()
                .Where(d => d.IntervalId == intervalId)
                .OrderByDescending(d => d.DateAndTime)
                .Select(x => x.ClosePrice)
                .FirstOrDefaultAsync();

            return price.ToString("f2");
        }

        public async Task<DateTime> GetLastUpdatedTime(string ticker)
        {
            var stockId = await this.GetStockId(ticker);

            var intervalId = await this.GetIntervalId(stockId);

            var time = await this.datasetRepository
                .All()
                .Where(d => d.IntervalId == intervalId)
                .OrderByDescending(d => d.DateAndTime)
                .Select(x => x.DateAndTime)
                .FirstOrDefaultAsync();

            return time;
        }

        public async Task<PriceTimeViewModel> GetLastPriceAndTime(string ticker)
        {
            var stockId = await this.GetStockId(ticker);

            var intervalId = await this.GetIntervalId(stockId);

            return await this.datasetRepository
                .All()
                .Where(d => d.IntervalId == intervalId)
                .OrderByDescending(x => x.DateAndTime)
                .Select(x => new PriceTimeViewModel()
                {
                    Price = x.ClosePrice.ToString("F2"),
                    DateTime = x.DateAndTime.ToString("g", CultureInfo.InvariantCulture),
                })
                .FirstOrDefaultAsync();
        }

        public async Task<CheckResult> GetUpdate(string lastData, string ticker)
        {
            DateTime siteDate = new DateTime(1900, 1, 1);

            if (!string.IsNullOrEmpty(lastData))
            {
                siteDate = DateTime.ParseExact(lastData, "g", CultureInfo.InvariantCulture);
            }

            //TODO: Fix cache
            // Not clearing data after 1 min.:
            //if (!this.memoryCache.TryGetValue<TempData>("StockData", out var lastTempData))
            //{
            //    lastTempData = await this.GetLastUpdatedData(ticker);
            //    this.memoryCache.Set("StockData", lastTempData, TimeSpan.FromMinutes(1));
            //}

            var lastTempData = await this.GetLastUpdatedData(ticker);

            if (lastTempData.LastDateTime > siteDate)
            {
                var result = new CheckResult
                {
                    New = true,
                    NewPrice = lastTempData.LastPrice.ToString("f2"),
                    NewTime = lastTempData.LastDateTime.ToString("g", CultureInfo.InvariantCulture),
                };

                return result;
            }

            return new CheckResult { New = false };
        }

        public async Task ImportData(string jsonString, string ticker)
        {
            var dailySeriesImportDto = JsonConvert.DeserializeObject<DailySeriesImport>(jsonString);

            var metaDto = dailySeriesImportDto.MetaDataImport;

            var lastUpdatedTime = await this.GetLastUpdatedTime(ticker);

            var timeDto = dailySeriesImportDto.TimeSeries1min;

            var stockId = await this.GetStockId(ticker);

            var intervalId = await this.GetIntervalId(stockId);

            var metaData = new MetaData
            {
                Information = metaDto.The1Information,
                LastRefreshed = DateTime.Parse(metaDto.The3LastRefreshed, CultureInfo.InvariantCulture),
                IntervalId = intervalId,
                OutputSize = metaDto.The5OutputSize,
                TimeZone = metaDto.The5TimeZone,
            };

            if (metaData.LastRefreshed == lastUpdatedTime)
            {
                return;
            }

            await this.metadataRepository.AddAsync(metaData);
            await this.metadataRepository.SaveChangesAsync();

            foreach (var (minute, data) in timeDto)
            {
                if (minute <= lastUpdatedTime)
                {
                    continue;
                }

                var currentMinute = new DataSet
                {
                    DateAndTime = minute,
                    OpenPrice = decimal.Parse(data.The1Open),
                    HighPrice = decimal.Parse(data.The2High),
                    LowPrice = decimal.Parse(data.The3Low),
                    ClosePrice = decimal.Parse(data.The4Close),
                    Volume = data.The5Volume,
                    IntervalId = intervalId,
                };

                await this.datasetRepository.AddAsync(currentMinute);
            }

            await this.datasetRepository.SaveChangesAsync();
        }

        public async Task CreateStockAsync(string ticker, string name, string intervalName)
        {
            var interval = new Interval
            {
                Name = intervalName,
            };

            var stock = new Stock
            {
                Name = name,
                Ticker = ticker,
            };

            stock.Intervals.Add(interval);

            await this.stockRepository.AddAsync(stock);

            await this.stockRepository.SaveChangesAsync();
        }

        public async Task<IList<PriceTimeViewModel>> GetPricesLast300Minutes(string ticker)
        {
            var stockId = await this.GetStockId(ticker);

            var intervalId = await this.GetIntervalId(stockId);

            var result = this.datasetRepository
                .All()
                .Where(d => d.IntervalId == intervalId)
                .OrderByDescending(x => x.DateAndTime)
                .Select(x => new PriceTimeViewModel()
                {
                    Price = x.ClosePrice.ToString("F2"),
                    DateTime = x.DateAndTime.ToString("g", CultureInfo.InvariantCulture),
                })
                .Take(90)
                .ToList();

            return result.OrderBy(x => DateTime.Parse(x.DateTime)).ToList();
        }

        private async Task<int> GetIntervalId(int stockId)
        {
            return await this.intervalRepository
                .All()
                .Where(i => i.StockId == stockId)
                .Select(i => i.Id)
                .FirstOrDefaultAsync();
        }

        private async Task<int> GetStockId(string ticker)
        {
            return await this.stockRepository
                .All()
                .Where(s => s.Ticker == ticker)
                .Select(s => s.Id)
                .FirstOrDefaultAsync();
        }

        private async Task<TempData> GetLastUpdatedData(string ticker)
        {
            var data = await this.datasetRepository
                .All()
                .OrderByDescending(d => d.DateAndTime)
                .Select(x => new TempData()
                {
                    LastPrice = x.ClosePrice,
                    LastDateTime = x.DateAndTime,
                })
                .FirstOrDefaultAsync();

            return data;
        }
    }
}
