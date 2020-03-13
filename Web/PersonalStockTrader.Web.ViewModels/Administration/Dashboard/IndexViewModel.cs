namespace PersonalStockTrader.Web.ViewModels.Administration.Dashboard
{
    using System.Collections.Generic;

    public class IndexViewModel
    {
        public IEnumerable<AccountManagerOutputViewModel> AccountManagers { get; set; }
    }
}
