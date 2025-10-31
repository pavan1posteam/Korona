using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korona.Model
{
    class TaxResponseModel
    {
        public class EconomicZone
        {
            public string id { get; set; }
            public string name { get; set; }
            public string number { get; set; }
        }

        public class Links
        {
            public string self { get; set; }
        }

        public class Rate
        {
            public double rate { get; set; }
            public DateTime validFrom { get; set; }
        }

        public class Result
        {
            public bool active { get; set; }
            public string id { get; set; }
            public int revision { get; set; }
            public string number { get; set; }
            public EconomicZone economicZone { get; set; }
            public bool included { get; set; }
            public string name { get; set; }
            public List<Rate> rates { get; set; }
        }

        public class Root
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
}
