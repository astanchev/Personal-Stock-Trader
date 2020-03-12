namespace PersonalStockTrader.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using PersonalStockTrader.Data.Common.Models;

    public class Stock : BaseDeletableModel<int>
    {
        public Stock()
        {
            this.Intervals = new HashSet<Interval>();
        }

        [Required]
        public string Name { get; set; }

        [Required]
        [StringLength(5, MinimumLength = 1)]
        public string Ticker { get; set; }

        public virtual ICollection<Interval> Intervals { get; set; }
    }
}