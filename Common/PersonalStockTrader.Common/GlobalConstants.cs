namespace PersonalStockTrader.Common
{
    using System;

    public static class GlobalConstants
    {
        public const string SystemName = "PersonalStockTrader";

        public const string SystemDisplayName = "Personal Stock Trader";

        public const string AdministratorRoleName = "Administrator";

        public const string AccountManagerRoleName = "AccountManager";

        public const string ConfirmedUserRoleName = "ConfirmedUser";

        public const string NotConfirmedUserRoleName = "NotConfirmedUser";

        public const string SystemEmail = "personal.stock.trader@gmail.com";

        public const string ConstSubject = "From footer";

        public const string StockName = "International Business Machines Corporation";

        public const string StockTicker = "IBM";

        public const string StockFunction = "TIME_SERIES_INTRADAY";

        public const string StockInterval = "1min";

        public const decimal MinAccountBalance = 500.00M;
    }
}
