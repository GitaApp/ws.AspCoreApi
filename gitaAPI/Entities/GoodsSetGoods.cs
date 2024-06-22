using System;
using System.ComponentModel.DataAnnotations;

namespace gitaAPI.Entities
{
	public class GoodsSetGoods

    {

        public List<GoodsLine> goodsData { get; set; }
    }

    public class GoodsLine
    {
        public int lineId { get; set; }
        public List<Good> goods { get; set; }
    }

    public class Good
    {
        public int id { get; set; }
        public int extId { get; set; }
        public string name { get; set; }
        public int count { get; set; }
        public int units { get; set; }
        public decimal weight { get; set; }
        public decimal length { get; set; }
        public decimal width { get; set; }
        public decimal height { get; set; }
        public decimal m3 { get; set; }
        public decimal lm { get; set; }
        public decimal price { get; set; }
        public int currency { get; set; }
        public string adr { get; set; }
        public string adrData { get; set; }
        public string description { get; set; }
        public bool delete { get; set; }
    }

}
