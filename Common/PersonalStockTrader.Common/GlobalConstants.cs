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

        public const string ConfirmMessage = "Your account has been approved!";

        public const string ConfirmSubject = "Account confirmation";

        public const string EmailSent = "Email Sent!";

        public const string LockOut = "User account locked out.";

        public const string InvalidLogin = "Invalid login attempt.";

        public const string TextError = "The {0} must be at least {2} and at max {1} characters long.";

        public const string UsernameErrorRegex = "The {0} must have only letters and numbers.";

        public const string PassConfirmError = "The password and confirmation password do not match.";

        public const string BalanceError = "Value for {0} must be at least {1} USD.";

        public const string UserExists = "User with this name exists!";

        public const string CreatedNewPass = "User created a new account with password.";

        public const string ShortTextError = "The {0} must be max {1} characters long.";
    }
}
