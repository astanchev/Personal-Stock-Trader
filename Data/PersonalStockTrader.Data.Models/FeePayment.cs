namespace PersonalStockTrader.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using PersonalStockTrader.Data.Common.Models;

    public class FeePayment : BaseDeletableModel<int>
    {
        [Column(TypeName = "decimal(18,4)")]
        [Range(typeof(decimal), "0.01", "100.00")]
        public decimal Amount { get; set; }

        public TypeFee TypeFee { get; set; }

        public int AccountId { get; set; }

        public virtual Account Account { get; set; }
    }
}
