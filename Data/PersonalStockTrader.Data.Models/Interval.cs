namespace PersonalStockTrader.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using PersonalStockTrader.Data.Common.Models;

    public class Interval : BaseDeletableModel<int>
    {
        public Interval()
        {
            this.MetaDatas = new HashSet<MetaData>();
            this.DataSets = new HashSet<DataSet>();
        }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<MetaData> MetaDatas { get; set; }

        public virtual ICollection<DataSet> DataSets { get; set; }
    }
}