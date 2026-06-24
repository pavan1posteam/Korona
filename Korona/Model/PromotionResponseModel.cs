using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Korona.Model
{
    public class PromotionResponseModel
    {
        public int currentPage { get; set; }
        public int pagesTotal { get; set; }
        public List<PromotionResult> results { get; set; }
    }

    public class PromotionResult
    {
        public bool active { get; set; }
        public string id { get; set; }
        public int revision { get; set; }
        public string number { get; set; }
        public Benefit benefit { get; set; }
        public bool deactivated { get; set; }
        public string name { get; set; }
        public string type { get; set; }
    }

    public class Benefit
    {
        public Common common { get; set; }
        public Layer layer { get; set; }
        public string type { get; set; }
    }

    public class Common
    {
        public string applianceTarget { get; set; }
        public string applianceType { get; set; }
        public TargetTag targetTag { get; set; }
        public string type { get; set; }
        public string unitType { get; set; }
        public decimal value { get; set; }
    }

    public class TargetTag
    {
        public string id { get; set; }
        public string name { get; set; }
        public string number { get; set; }
    }

    public class Layer
    {
        public bool exclusive { get; set; }
    }
}
