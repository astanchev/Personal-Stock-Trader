namespace PersonalStockTrader.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Common.Models;

    public class Account : BaseDeletableModel<int>
    {
        public Account()
        {
            this.Confirmed = false;
            this.Positions = new HashSet<Position>();
            this.Fees = new HashSet<FeePayment>();
        }

        [Range(typeof(decimal), "1000.0", "79228162514264337593543950335")]
        public decimal Balance { get; set; }

        [Range(typeof(decimal), "0.01", "100.00")]
        public decimal MonthlyFee { get; set; }

        [Range(typeof(decimal), "0.01", "100.00")]
        public decimal TradeFee { get; set; }

        public bool Confirmed { get; set; }

        public string Notes { get; set; }

        [Required]
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<Position> Positions { get; set; }

        public virtual ICollection<FeePayment> Fees { get; set; }
    }
}