using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korona.Model
{
    public class Amount
    {
        public double actual { get; set; }
        public double lent { get; set; }
        public double maxLevel { get; set; }
        public double ordered { get; set; }
        public double reorderLevel { get; set; }
    }

    public class Links
    {
        public string next { get; set; }
        public string self { get; set; }
    }

    public class Product
    {
        public string id { get; set; }
        public string name { get; set; }
        public string number { get; set; }
    }

    public class Result
    {
        public Amount amount { get; set; }
        public double averagePurchasePrice { get; set; }
        public Product product { get; set; }
        public int revision { get; set; }
    }

    public class StockResponseModel
    {
        public int currentPage { get; set; }
        public Links links { get; set; }
        public int pagesTotal { get; set; }
        public List<Result> results { get; set; }
        public int resultsOfPage { get; set; }
        public int resultsTotal { get; set; }
        public int maxRevision { get; set; }
    }


}
