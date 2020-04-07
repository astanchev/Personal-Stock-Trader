﻿namespace PersonalStockTrader.Web.ViewModels
{
    public class ErrorViewModel
    {
        public int StatusCode { get; set; }

        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(this.RequestId);
    }
}
