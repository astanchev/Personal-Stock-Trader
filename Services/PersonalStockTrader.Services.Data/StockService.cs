namespace PersonalStockTrader.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using PersonalStockTrader.Common;
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

        public StockService(IDeletableEntityRepository<Stock> stockRepository, IDeletableEntityRepository<Interval> intervalRepository, IDeletableEntityRepository<DataSet> datasetRepository, IDeletableEntityRepository<MetaData> metadataRepository)
        {
            this.stockRepository = stockRepository;
            this.intervalRepository = intervalRepository;
            this.datasetRepository = datasetRepository;
            this.metadataRepository = metadataRepository;
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

        public async Task<DisplayViewModel> GetLastPriceAndTime(string ticker)
        {
            var stockId = await this.GetStockId(ticker);

            var intervalId = await this.GetIntervalId(stockId);

            return await this.datasetRepository
                .All()
                .Where(d => d.IntervalId == intervalId)
                .OrderByDescending(x => x.DateAndTime)
                .Select(x => new DisplayViewModel()
                {
                    Price = x.ClosePrice.ToString("F2"),
                    DateTime = x.DateAndTime.ToString("o"),
                })
                .FirstOrDefaultAsync();
        }

        public async Task<CheckResult> GetUpdate(string lastData, string ticker)
        {
            DateTime siteDate = new DateTime(1900, 1, 1);

            if (!string.IsNullOrEmpty(lastData))
            {
                siteDate = DateTime.ParseExact(lastData, "o", CultureInfo.InvariantCulture);
            }

            if (await this.GetLastUpdatedTime(ticker) > siteDate)
            {
                var result = new CheckResult
                {
                    New = true,
                    NewPrice = await this.GetLastPrice(ticker),
                    NewTime = await this.GetLastUpdatedTime(ticker),
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
    }
}