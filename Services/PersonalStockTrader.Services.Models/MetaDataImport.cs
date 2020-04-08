namespace PersonalStockTrader.Services.Models
{
    using Newtonsoft.Json;

    public class MetaDataImport
    {
        [JsonProperty("1. Information")]
        public string The1Information { get; set; }

        [JsonProperty("2. Symbol")]
        public string The2Symbol { get; set; }

        [JsonProperty("3. Last Refreshed")]
        public string The3LastRefreshed { get; set; }

        [JsonProperty("4. Interval")]
        public string The4Interval { get; set; }

        [JsonProperty("5. Output Size")]
        public string The5OutputSize { get; set; }

        [JsonProperty("6. Time Zone")]
        public string The5TimeZone { get; set; }
    }
}