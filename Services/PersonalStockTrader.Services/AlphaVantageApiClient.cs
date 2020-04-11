namespace PersonalStockTrader.Services
{
    using System;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using PersonalStockTrader.Common;
    using PersonalStockTrader.Services.Data;

    public class AlphaVantageApiClient : IApiConnection
    {
        private const string OutputSizeFull = "full";
        private const string OutputSizeCompact = "compact";
        private const string ApiString = @"{0}query?function={1}&symbol={2}&interval={3}&outputsize={4}&apikey={5}";

        private readonly IStockService stockService;
        private readonly IConfiguration configuration;

        public AlphaVantageApiClient(IStockService stockService, IServiceProvider serviceProvider)
        {
            this.stockService = stockService;
            this.configuration = serviceProvider.GetRequiredService<IConfiguration>();
        }

        public async Task GetCurrentData(string function, string ticker, string interval)
        {
            var lastUpdatedTime = await this.stockService.GetLastUpdatedTime(GlobalConstants.StockTicker);
            var apiUrl = string.Empty;

            if (lastUpdatedTime.AddMinutes(100) < DateTime.UtcNow)
            {
                apiUrl = this.ComposeApiUrl(function, ticker, interval, OutputSizeFull);
            }
            else
            {
                apiUrl = this.ComposeApiUrl(function, ticker, interval, OutputSizeCompact);
            }

            var result = await GetData(apiUrl);

            await this.stockService.ImportData(result, ticker);
        }

        private async Task<string> GetData(string apiUrl)
        {
            WebRequest requestObj = WebRequest.Create(apiUrl);
            requestObj.Method = "GET";
            WebResponse responseObj = await requestObj.GetResponseAsync();

            string result = string.Empty;

            await using (Stream stream = responseObj.GetResponseStream())
            {
                if (stream != null)
                {
                    StreamReader sr = new StreamReader(stream);
                    result = await sr.ReadToEndAsync();
                }
            }

            return result;
        }

        private string ComposeApiUrl(string function, string ticker, string interval, string outputSize)
        {
            return string.Format(ApiString, this.configuration["API:URI"], function, ticker, interval, outputSize, this.configuration["API:AlphaVantageKEY"]);
        }
    }
}
