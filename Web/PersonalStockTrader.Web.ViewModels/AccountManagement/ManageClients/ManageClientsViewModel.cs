namespace PersonalStockTrader.Web.ViewModels.AccountManagement.ManageClients
{
    using System.Collections.Generic;

    public class ManageClientsViewModel
    {
        public IEnumerable<ConfirmedAccountsViewModel> Clients { get; set; }
    }
}

