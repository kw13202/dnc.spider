using System;
using System.Collections.Generic;
using System.Text;

namespace dnc.model
{
    public class HisPrice
    {
        public int Id { get; set; }
        public string GoodsCode { get; set; }
        public decimal CurPrice { get; set; }
        public decimal PlusPrice { get; set; }
        public decimal DiscountPrice { get; set; }
        public string DiscountDesc { get; set; }
        public DateTime SpiderTime { get; set; }

        //public virtual Goods Good { get; set; }
    }
}
