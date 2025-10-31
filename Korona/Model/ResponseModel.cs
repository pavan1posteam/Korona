// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
using System;
using System.Collections.Generic;

public class AlternativeSector
{
    public string id { get; set; }
    public string name { get; set; }
    public string number { get; set; }
}
public class Tag
{
    public string id { get; set; }
    public string name { get; set; }
    public string number { get; set; }
}

public class Assortment
{
    public string id { get; set; }
    public string name { get; set; }
    public string number { get; set; }
}

public class Code
{
    public string productCode { get; set; }
    public double containerSize { get; set; }
    public string description { get; set; }
}

public class CommodityGroup
{
    public string id { get; set; }
    public string name { get; set; }
    public string number { get; set; }
}

public class Links
{
    public string next { get; set; }
    public string self { get; set; }
}

public class Price
{
    public double value { get; set; }
    public DateTime validFrom { get; set; }
    public PriceGroup priceGroup { get; set; }
}

public class PriceGroup
{
    public string id { get; set; }
    public string name { get; set; }
    public string number { get; set; }
}

public class Product
{
    public string id { get; set; }
    public string name { get; set; }
    public string number { get; set; }
}

public class Result
{
    public bool active { get; set; }
    public string id { get; set; }
    public int revision { get; set; }
    public string number { get; set; }
    public Assortment assortment { get; set; }
    public List<Code> codes { get; set; }
    public CommodityGroup commodityGroup { get; set; }
    public bool conversion { get; set; }
    public double costs { get; set; }
    public bool deactivated { get; set; }
    public bool deposit { get; set; }
    public bool discountable { get; set; }
    public double lastPurchasePrice { get; set; }
    public bool listed { get; set; }
    public DateTime listedSince { get; set; }
    public double maxPrice { get; set; }
    public double minPrice { get; set; }
    public string name { get; set; }
    public bool packagingRequired { get; set; }
    public bool personalizationRequired { get; set; }
    public bool priceChangable { get; set; }
    public List<Price> prices { get; set; }
    public bool printTicketsSeparately { get; set; }
    public Sector sector { get; set; }
    public bool serialNumberRequired { get; set; }
    public string subproductPresentation { get; set; }
    public List<SupplierPrice> supplierPrices { get; set; }
    public bool trackInventory { get; set; }
    public bool salesLock { get; set; }
    public bool independentSubarticleDiscounts { get; set; }
    public bool stockReturnUnsellable { get; set; }
    public List<Subproduct> subproducts { get; set; }
    public List<Tag> tag { get; set; }
    public AlternativeSector alternativeSector { get; set; }
    public List<Container> containers { get; set; }
}
public class Container
{
    public List<Price> prices { get; set; }
    public Product product { get; set; }
    public bool defaultContainer { get; set; }
}
public class ResponseModel
{
    public int currentPage { get; set; }
    public Links links { get; set; }
    public int pagesTotal { get; set; }
    public List<Result> results { get; set; }
    public int resultsOfPage { get; set; }
    public int resultsTotal { get; set; }
    public int maxRevision { get; set; }
}

public class Sector
{
    public string id { get; set; }
    public string name { get; set; }
    public string number { get; set; }
}

public class Subproduct
{
    public List<Price> prices { get; set; }
    public Product product { get; set; }
    public double quantity { get; set; }
    public string type { get; set; }
}

public class Supplier
{
    public string id { get; set; }
    public string name { get; set; }
    public string number { get; set; }
}

public class SupplierPrice
{
    public Supplier supplier { get; set; }
    public string orderCode { get; set; }
    public double value { get; set; }
    public double containerSize { get; set; }
    public string description { get; set; }
}

