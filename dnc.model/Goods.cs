using System;
using System.Collections.Generic;
using System.Text;

namespace dnc.model
{
    public class Goods
    {
        public string GoodsCode { get; set; }
        public string GoodsName { get; set; }
        public decimal? LowestPrice { get; set; }
        public DateTime? LowestPriceTime { get; set; }
        public decimal CurPrice { get; set; }
        public decimal? PlusPrice { get; set; }
        public decimal? DiscountPrice { get; set; }
        public string DiscountDesc { get; set; }
        public DateTime? SpiderTime { get; set; }
        public DateTime? CreateTime { get; set; }

        //public virtual IList<HisPrice> HisPrices { get; set; }
    }
}
