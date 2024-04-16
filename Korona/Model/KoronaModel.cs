using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korona.Model
{
    public class KoronaModel
    {
    }
    public class Fullname
    {
        public string pname { get; set; }
        public string pdesc { get; set; }
        public string upc { get; set; }
        public string sku { get; set; }
        public decimal? Price { get; set; }
        public string uom { get; set; }
        public int pack { get; set; }
        public string pcat { get; set; }
        public string pcat1 { get; set; }
        public string pcat2 { get; set; }
        public string country { get; set; }
        public string region { get; set; }

    }
    public class FinalResult
    {
        public string Productid { get; set; }
        public int storeid { get; set; }
        public string upc { get; set; }
        public Int32 qty { get; set; }
        public string sku { get; set; }
        public string uom { get; set; }
        public int pack { get; set; }
        public string StoreProductName { get; set; }
        public string StoreDescription { get; set; }
        public decimal? price { get; set; }
        public string sprice { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public string tax { get; set; }
        public string altupc1 { get; set; }
        public string altupc2 { get; set; }
        public string altupc3 { get; set; }
        public string altupc4 { get; set; }
        public string altupc5 { get; set; }
        public decimal Deposit { get; set; }
        public string cat { get; set; }
        public string Discountable { get; set; }
    }
    
    //public class PriceGroup
    //{
    //    public string id { get; set; }
    //    public string name { get; set; }
    //    public string number { get; set; }
    //   ///// public object product { get; set; }
    //}
    //public class Sector
    //{
    //    public string id { get; set; }
    //    public string name { get; set; }
    //    public string number { get; set; }
    //}
    public class codes
    {
        public string productCode { get; set; }
        public decimal containerSize { get; set; }
    }
    public class commodityGroup
    {
        public string id { get; set; }
        public string name { get; set; }
        public string number { get; set; }
    }
    //public class Tag
    //{
    //    public string id { get; set; }
    //    public string name { get; set; }
    //    public string number { get; set; }
    //}

    public class QuantityPrice
    {
        public decimal? value { get; set; }
        public DateTime? validFrom { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string number { get; set; }
    }
    // For Product  Stock   

    public class StockActual
    {
        public decimal? actual { get; set; }
    }
    public class product
    {
        public string id { get; set; }
        public string name { get; set; }
        public string number { get; set; }
    }

    public class StockProduct
    {
        public string id { get; set; }
        public string name { get; set; }
        public string number { get; set; }
    }
}
