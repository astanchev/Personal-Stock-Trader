namespace PersonalStockTrader.Web.ViewModels.AccountManagement.NewClients
{
    using System.Collections.Generic;

    public class NewClientsViewModel
    {
        public IEnumerable<NotConfirmedClientsViewModel> NewClients { get; set; }
    }
}