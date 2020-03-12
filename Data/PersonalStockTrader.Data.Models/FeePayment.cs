namespace PersonalStockTrader.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using PersonalStockTrader.Data.Common.Models;

    public class FeePayment : BaseDeletableModel<int>
    {
        public DateTime PaymentDate { get; set; }

        [Range(typeof(decimal), "0.01", "100.00")]
        public decimal Amount { get; set; }

        public TypeFee TypeFee { get; set; }

        public int AccountId { get; set; }

        public Account Account { get; set; }
    }
}
