namespace PersonalStockTrader.Services
{
    using System.Threading.Tasks;

    public interface IApiConnection
    {
        Task GetCurrentData(string function, string ticker, string interval);
    }
}