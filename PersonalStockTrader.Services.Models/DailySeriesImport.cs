namespace PersonalStockTrader.Services.Models
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class DailySeriesImport
    {
        [JsonProperty("Meta Data")]
        public MetaDataImport MetaDataImport { get; set; }

        [JsonProperty("Time Series (1min)")]
        public Dictionary<DateTime, TimeSeries1minImport> TimeSeries1min { get; set; }
    }
}