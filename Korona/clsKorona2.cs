using Korona.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Korona
{
    class clsKorona2//testing by jacob on 06/11 again by PK again jk
    {
        string baseUrl = ConfigurationManager.AppSettings["KoronaBaseUrl"];
        string apiUrl = ConfigurationManager.AppSettings["KoronaApiUrl"];
        string folderPath = ConfigurationManager.AppSettings["BaseDirectory"];
        string codeupc = ConfigurationManager.AppSettings["different_upc"];
        string deposit = ConfigurationManager.AppSettings["deposit"];
        string qty = ConfigurationManager.AppSettings["qty"];
        string difqty = ConfigurationManager.AppSettings["difqty"];
        string catbased = ConfigurationManager.AppSettings["catbased"];
        string stocklist = ConfigurationManager.AppSettings["stocklist"];
        string catfilter = ConfigurationManager.AppSettings["catfilter"];
        string staticqty = ConfigurationManager.AppSettings["qtystatic"];
        string tax_rate = ConfigurationManager.AppSettings["tax_rate"];
        string Nonstocklistqty = ConfigurationManager.AppSettings["Nonstocklistqty"];
        string TagAsUom = ConfigurationManager.AppSettings["tagasuom"];
        string sprice = ConfigurationManager.AppSettings["sprice"];
        string discountable = ConfigurationManager.AppSettings["dicountable"];
        string tax_category = ConfigurationManager.AppSettings["tax_category"];
        string different_sku = ConfigurationManager.AppSettings["different_sku"];
        string upcnull = ConfigurationManager.AppSettings["upcnull"];
        string skunull = ConfigurationManager.AppSettings["skunull"];
        string diiffer_Url = ConfigurationManager.AppSettings["diiffer_Url"];
        string TaxFromResp = ConfigurationManager.AppSettings["TaxFromResp"];
        string AllowCBD = ConfigurationManager.AppSettings["AllowCBD"];
        string IncludeContainers = ConfigurationManager.AppSettings["IncludeContainers"];
        string Includedeposit = ConfigurationManager.AppSettings["Includedeposit"];
        string Beerdeposit = ConfigurationManager.AppSettings["Beerdeposit"];
        string AppendArticleUPCs = ConfigurationManager.AppSettings["AppendArticleUPCs"];

        public int StoreId;
        public string BaseUrl;
        public string MerchantId;
        public string ApiKey;
        public string StorePriceGroupId;
        public string OrganisationalId;
        public string Tax;
        public decimal Deposit;

        List<ResponseModel> resmdl = new List<ResponseModel>();
        List<StockResponseModel> stkModel = new List<StockResponseModel>();
        List<TaxResponseModel.Root> taxModel = new List<TaxResponseModel.Root>();

        List<FinalResult> finalResultList = new List<FinalResult>();
        List<Fullname> fullnameList = new List<Fullname>();
        List<QuantityPrice> PriceResult = new List<QuantityPrice>();
        List<codes> CodeResult = new List<codes>();

        string pathProduct = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"ProductDetails_Korona.json");
        string pathStock = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"KoronaProductStock.json");
        string PathProductTax = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"ProductTax.json");

        public clsKorona2(int _storeid, string _BaseUrl, string _MerchantId, string _ApiKey,string _StorePriceGroupId, string _OrganisationalId, string _tax, decimal _deposit)
        {
            StoreId = _storeid;
            BaseUrl = _BaseUrl;
            MerchantId = _MerchantId;
            ApiKey = _ApiKey;
            StorePriceGroupId = _StorePriceGroupId;
            OrganisationalId = _OrganisationalId;
            Tax = _tax;
            Deposit = _deposit;
            Start();
        }
        public void Start()
        {
            DeleteProductPath();
            CreateKoronaProductResponseFile(BaseUrl, ApiKey);
            setupenv();
            CreateKoronaStockResponseFile(BaseUrl, ApiKey);
            if (BaseUrl.Contains("185"))
            {
                DeleteTaxPath();
                CreateKoronaTaxResponseFile(BaseUrl, ApiKey);
            }
            KoronaProductDetails(Tax, StorePriceGroupId, StoreId);
        }

        public void DeleteProductPath()
        {
            if (File.Exists(pathProduct))
            {
                File.Delete(pathProduct);
            }
        }
        public string getProduct(string BaseUrl, string ApiKey, string value = "", bool first = false)
        {
            ResponseModel mdl = new ResponseModel();
            if (first)
                value = "/web/api/v3/accounts/" + MerchantId + "/products?includeDeleted=false";
            var client = new RestClient(BaseUrl + value);
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Basic " + ApiKey);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            IRestResponse response = client.Execute(request);
            string content = response.Content;
            mdl = JsonConvert.DeserializeObject<ResponseModel>(content);
            resmdl.Add(mdl);
            //File.AppendAllText("12383.json", content);
            return content;
        }
        public void CreateKoronaProductResponseFile(string BaseUrl, string ApiKey, string value = "")
        {
            string content;
            if (string.IsNullOrEmpty(value))
                content = getProduct(BaseUrl, ApiKey, value, true);
            else
                content = getProduct(BaseUrl, ApiKey, value);

            var jResult = JObject.Parse(content);

            using (StreamWriter sw = new StreamWriter(pathProduct, append: true))
            {
                sw.WriteLine(content);
            }
            var links = (dynamic)jResult["links"];
            if (links != null)
            {
                foreach (var item in links)
                {
                    if (item.Name.ToString().ToUpper() != "PREVIOUS" && item.Name.ToString().ToUpper() != "SELF")
                    {
                        CreateKoronaProductResponseFile(BaseUrl , ApiKey, item.Value.ToString());
                    }
                    break;
                }
            }
        }
        public void setupenv()
        {
            finalResultList = new List<FinalResult>();
            if (File.Exists(pathStock))
            {
                File.Delete(pathStock);
            }
        }
        public string getStock(string BaseUrl, string ApiKey, string value = "", bool first = false)
        {
            StockResponseModel stk = new StockResponseModel();
            if (first)
                value = "/web/api/v3/accounts/" + MerchantId + "/organizationalUnits/" + OrganisationalId + "/productStocks";
            var client = new RestClient(BaseUrl + value);
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Basic " + ApiKey);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            IRestResponse response = client.Execute(request);
            string content = "";
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                content = response.Content;
                stk = JsonConvert.DeserializeObject<StockResponseModel>(content);
                stkModel.Add(stk);
            }
            //File.AppendAllText("12383Stock.json", content);
            return content;

        }
        public void CreateKoronaStockResponseFile(string BaseUrl, string ApiKey, string value = "")
        {
            string content;
            if (string.IsNullOrEmpty(value))
                content = getStock(BaseUrl, ApiKey, value, true);
            else
                content = getStock(BaseUrl, ApiKey, value);
            var jResult = JObject.Parse(content);

            using (StreamWriter sw = new StreamWriter(pathStock, append: true))
            {
                sw.WriteLine(content);
            }
            var links = (dynamic)jResult["links"];
            if (links != null)
            {
                foreach (var item in links)
                {
                    if (item.Name.ToString().ToUpper() != "PREVIOUS" && item.Name.ToString().ToUpper() != "SELF")
                    {
                        CreateKoronaStockResponseFile(BaseUrl, ApiKey, item.Value.ToString());
                    }
                    break;
                }
            }
        }
        public void DeleteTaxPath()
        {
            if (File.Exists(PathProductTax))
            {
                File.Delete(PathProductTax);
            }
        }
        public string getTax(string BaseUrl, string ApiKey, string value = "", bool first = false)
        {
            TaxResponseModel.Root tax = new TaxResponseModel.Root();
            if (first)
                value = "/web/api/v3/accounts/" + MerchantId + "/salesTaxes";
            var client = new RestClient(BaseUrl + value);
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Basic " + ApiKey);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            IRestResponse response = client.Execute(request);
            string content = "";
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                content = response.Content;
                tax = JsonConvert.DeserializeObject<TaxResponseModel.Root>(content);
                taxModel.Add(tax);
            }
            return content;
        }
        public void CreateKoronaTaxResponseFile(string BaseUrl, string ApiKey, string value = "")
        {
            string content;
            if (string.IsNullOrEmpty(value))
                content = getTax(BaseUrl, ApiKey, value, true);
            else
                content = getTax(BaseUrl, ApiKey, value);
            var jResult = JObject.Parse(content);

            using (StreamWriter sw = new StreamWriter(PathProductTax, append: true))
            {
                sw.WriteLine(content);
            }
            var links = (dynamic)jResult["links"];
            if (links != null)
            {
                foreach (var item in links)
                {
                    if (item.Name.ToString().ToUpper() != "PREVIOUS" && item.Name.ToString().ToUpper() != "SELF")
                    {
                        CreateKoronaTaxResponseFile(BaseUrl, ApiKey, item.Value.ToString());
                    }
                    break;
                }
            }
        }
        public void KoronaProductDetails(string tax, string StorePriceGroupId, int StoreId)
        {
            try
            {
                finalResultList = new List<FinalResult>();
                fullnameList = new List<Fullname>();

                foreach (var item in resmdl)
                {
                    var data = item.results;
                    foreach (var dataitem in data)
                    {
                        var finalResult = new FinalResult();
                        var fullname = new Fullname();
                        var upc = "";
                        if (codeupc.Contains(StoreId.ToString()))
                        {
                            upc = dataitem.number;
                        }
                        else if(AppendArticleUPCs.Contains(StoreId.ToString()))
                        {
                            upc = dataitem.codes == null ? dataitem.number == null ? "": "99"+StoreId+dataitem.number : dataitem.codes.FirstOrDefault().productCode;
                        }
                        else
                        {
                            upc = dataitem.codes == null ? "" : dataitem.codes.FirstOrDefault().productCode;
                        }
                        finalResult.storeid = StoreId;
                        if (skunull.Contains(StoreId.ToString()) && !string.IsNullOrEmpty(upc))
                        {
                            finalResult.upc = "#" + upc.ToString().Trim();
                            fullname.upc = finalResult.upc;
                            finalResult.sku = finalResult.upc;
                            fullname.sku = finalResult.upc;
                        }
                        else if (AppendArticleUPCs.Contains(StoreId.ToString()) && !string.IsNullOrEmpty(upc))
                        {
                            finalResult.upc = "#" + upc.ToString().Trim();
                            fullname.upc = finalResult.upc;
                            if(!string.IsNullOrEmpty(dataitem.number))
                            {
                                finalResult.sku = "#" + dataitem.number.Trim();
                                fullname.sku = "#" + dataitem.number.Trim();
                            }
                            else
                            {
                                finalResult.sku = finalResult.upc;
                                fullname.sku = finalResult.upc;
                            }
                        }
                        else if (upcnull.Contains(StoreId.ToString()))
                        {
                            finalResult.upc = "#" + dataitem.number.Trim();
                            fullname.upc = finalResult.upc;
                            finalResult.sku = finalResult.upc;
                            fullname.sku = finalResult.upc;
                        }
                        else if (!string.IsNullOrEmpty(upc) && !string.IsNullOrEmpty(dataitem.number))
                        {
                            finalResult.upc = "#" + upc.ToString().Trim();
                            fullname.upc = "#" + upc.ToString().Trim();
                            finalResult.sku = "#" + dataitem.number.Trim();
                            fullname.sku = "#" + dataitem.number.Trim();
                        }
                        else
                        {
                            continue;
                        }

                        if (different_sku.Contains(StoreId.ToString()))
                        {
                            if (!string.IsNullOrEmpty(upc))
                            {
                                fullname.sku = "#" + upc.ToString().Trim();
                                finalResult.sku = "#" + upc.ToString().Trim();
                            }
                            else
                            {
                                continue;
                            }
                        }
                        //if (Regex.IsMatch(fullname.sku, @"[A-Z]|[a-z]"))
                        //    continue;
                        //if (Regex.IsMatch(upc,@"[A-Z]|[a-z]"))
                        //    continue;
                        finalResult.Productid = dataitem.id;
                        finalResult.StoreDescription = dataitem.name;
                        finalResult.StoreProductName = dataitem.name;
                        fullname.pdesc = dataitem.name;
                        fullname.pname = dataitem.name;
                        var cmgp = dataitem.commodityGroup;
                        if (cmgp != null)
                        {
                            if (!Regex.IsMatch(cmgp.name, @"\d+"))
                                fullname.pcat = cmgp.name;
                            else
                                fullname.pcat = "";
                        }
                        if (tax_rate.Contains(StoreId.ToString()))
                        {
                            var taxes = dataitem.sector;
                            double taxe;
                            var taxrate = "";
                            if (taxes != null)
                            {
                                taxe = Convert.ToDouble(taxes.number) * 0.01;
                                taxrate = taxe.ToString();
                            }
                            finalResult.tax = taxrate;
                        }
                        else
                        {
                            finalResult.tax = tax;
                        }
                        if (tax_category.Contains(StoreId.ToString()))
                        {
                            if (fullname.pcat.ToUpper() == "BEER")
                            {
                                finalResult.tax = Convert.ToString("0.0925");
                            }
                            else if (fullname.pcat.ToUpper() == "LIQUOR")
                            {
                                finalResult.tax = Convert.ToString("0.115");
                            }
                            else if (fullname.pcat.ToUpper() == "Mixers & More")
                            {
                                finalResult.tax = Convert.ToString("0.105");
                            }
                            else
                            {
                                finalResult.tax = Convert.ToString("0.0");
                            }
                        }

                        if (TaxFromResp.Contains(StoreId.ToString()))
                        {
                            foreach (var items in taxModel)
                            {
                                var datas = items;
                                foreach (var element in datas.results)
                                {
                                    for (int i = 0; i < datas.results.Count; i++)
                                    {
                                        if (dataitem.sector.number == items.results[i].number)
                                        {
                                            decimal a = (decimal)(items.results[i].rates[0].rate / 100);
                                            finalResult.tax = a.ToString();
                                            break;
                                        }
                                    }
                                    if (!string.IsNullOrEmpty(finalResult.tax))
                                    {
                                        break;
                                    }
                                }
                                if (!string.IsNullOrEmpty(finalResult.tax))
                                {
                                    break;
                                }
                            }
                        }
                        var Dep = "";
                        var Subproducts = dataitem.subproducts;

                        if (Subproducts != null && deposit.Contains(StoreId.ToString()))
                        {
                            var pr = new product();
                            var pcArrayCount = Subproducts.Count;
                            for (int k = 0; k < pcArrayCount; k++)
                            {
                                Dep = Subproducts[k].product.name;
                            }
                        }

                        
                        fullname.pcat1 = "";
                        fullname.pcat2 = "";
                        finalResult.pack = getpack(finalResult.StoreProductName);
                        fullname.pack = finalResult.pack;
                        finalResult.uom = getVolume(finalResult.StoreProductName);
                        fullname.uom = finalResult.uom;
                        if (discountable.Contains(StoreId.ToString())) //discountable column as per ticket 19362
                        {
                            var disctble = dataitem.discountable;
                            if (disctble == true)
                            {
                                finalResult.Discountable = 1.ToString();
                            }
                            else
                            {
                                finalResult.Discountable = 0.ToString();
                            }
                        }

                        if (TagAsUom.Contains(StoreId.ToString()))
                        {
                            var tags = dataitem.tag;
                            string xuom = "";
                            if (tags != null)
                            {

                                var pgx = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Tag>>(tags.ToString());
                                foreach (var tagitem in pgx)
                                {
                                    xuom = tagitem.name;
                                    if (Regex.IsMatch(xuom.ToUpper(), @"\dML$|\d\s+ML$|KEG$|\dL$|\d\s+L$|\dOZ$|\d\s+OZ$"))
                                    {
                                        xuom = Regex.Match(xuom.ToUpper(), @"\d+ML$|\d+\s+ML$|\d+[.]\d+ML$|\d+[.]\d+\s+ML$|KEG$|\d+L$|\d+\s+L$|\d+OZ$|\d+\s+OZ$|\d+[.]\d+L$|\d+[.]\d+\s+L$|\d+[.]\d+OZ$|\d+[.]\d+\s+OZ$").ToString();
                                        finalResult.uom = xuom;
                                        fullname.uom = xuom;
                                    }
                                }

                            }
                        }
                        fullname.region = "";
                        fullname.country = "";

                        if (deposit.Contains(StoreId.ToString()))
                        {
                            if (!string.IsNullOrEmpty(Dep.ToString()))
                            {
                                string dep = Dep.ToString().Trim();
                                dep = Regex.Replace(dep, "[^0-9.]", "");
                                finalResult.Deposit = Convert.ToDecimal(dep);
                            }
                            else
                            {
                                finalResult.Deposit = 0;
                            }
                        }
                        else
                        {
                            finalResult.Deposit = 0;
                        }
                        if (Includedeposit.Contains(StoreId.ToString()))
                        {
                            if (finalResult.StoreProductName.ToUpper().Contains("BOTTLE"))
                                finalResult.Deposit = finalResult.pack * Deposit;
                            else
                                finalResult.Deposit = 0;
                        }
                        if (Beerdeposit.Contains(StoreId.ToString()))
                        {
                            if (fullname.pcat.ToUpper().Contains("BEER"))
                                finalResult.Deposit = finalResult.pack * Deposit;
                            else
                                finalResult.Deposit = 0;
                        }
                        if (qty.Contains(StoreId.ToString()))
                        {
                            if (fullname.pcat.ToUpper() == "BEER" || fullname.pcat.ToUpper() == "CRAFT BEER")
                            {
                                finalResult.qty = 12;
                            }
                            else if (fullname.pcat.ToUpper().Contains("WINE") || fullname.pcat.ToUpper().Contains("SAKE") || fullname.pcat.ToUpper().Contains("SPARKLING"))
                            {
                                finalResult.qty = 50;
                            }
                            else
                            {
                                finalResult.qty = 30;
                            }
                        }
                        if (catbased.Contains(StoreId.ToString()))
                        {
                            if (fullname.pcat.ToUpper().Contains("CANDY") || fullname.pcat.ToUpper().Contains("SNACK") || fullname.pcat.ToUpper().Contains("DRINK") || fullname.pcat.ToUpper().Contains("SODA")
                                || fullname.pcat.ToUpper().Contains("MIXED DRINKS") || fullname.pcat.ToUpper().Contains("WATER") || fullname.pcat.ToUpper().Contains("ENERGY DRINK")
                                || fullname.pcat.ToUpper().Contains("MIXERS") || fullname.pcat.ToUpper().Contains("SPORTS DRINK") || fullname.pcat.ToUpper().Contains("BEER"))
                            {
                                finalResult.qty = 50;
                            }
                        }

                        PriceResult = new List<QuantityPrice>();

                        List<product> ProductResult = new List<product>();


                        if (IncludeContainers.Contains(StoreId.ToString()))
                        {
                            var containerrr = dataitem.containers != null ? dataitem.containers.FirstOrDefault(container => container.defaultContainer == true) : null;

                            if (dataitem.containers != null && dataitem.containers.Count > 0 && containerrr.defaultContainer)
                            {
                                foreach (var container in dataitem.containers)
                                {
                                    if (container.prices != null && container.prices.Count > 0 && container.defaultContainer)
                                    {
                                        var pArraycount = container.prices.Count;

                                        for (int j = 0; j < pArraycount; j++)
                                        {
                                            var val1 = container.prices[j];
                                            var pr = new QuantityPrice();
                                            var value = val1.value;
                                            var validFrom = val1.validFrom;

                                            var pgx = val1.priceGroup;

                                            if (pgx != null)
                                            {
                                                if (pgx.id == StorePriceGroupId && container.defaultContainer)
                                                {
                                                    pr.id = pgx.id;
                                                    pr.name = pgx.name;
                                                    pr.number = pgx.number;
                                                    pr.value = Convert.ToDecimal(value);
                                                    pr.validFrom = validFrom;
                                                    PriceResult.Add(pr);
                                                }
                                            }
                                        }
                                    }
                                    if (container.product != null && container.defaultContainer)
                                    {
                                        var prod = new product
                                        {
                                            id = container.product.id,
                                            name = container.product.name,
                                            number = container.product.number
                                        };

                                        var match = Regex.Match(prod.name, @"\d+");
                                        if (match.Success)
                                        {
                                            int packSize = int.Parse(match.Value);
                                            finalResult.pack = packSize;
                                            fullname.pack = packSize;
                                        }
                                        ProductResult.Add(prod);
                                    }
                                }
                            }
                            else
                            {
                                var p = dataitem.prices;
                                if (p != null)
                                {
                                    var pArraycount = p.Count;
                                    PriceResult = new List<QuantityPrice>();
                                    for (int j = 0; j < pArraycount; j++)
                                    {
                                        var val1 = p[j];
                                        var pr = new QuantityPrice();
                                        var value = val1.value;
                                        var validFrom = val1.validFrom;

                                        var pgx = val1.priceGroup;

                                        if (pgx != null)
                                        {

                                            if (pgx.id == StorePriceGroupId)
                                            {
                                                pr.id = pgx.id;
                                                pr.name = pgx.name;
                                                pr.number = pgx.number;
                                                pr.value = Convert.ToDecimal(value);
                                                pr.validFrom = validFrom;

                                                PriceResult.Add(pr);
                                            }
                                            if (sprice.Contains(StoreId.ToString())) //for sprice 11480
                                            {
                                                if (pgx.id == "27e64588-aee4-45b7-a9d6-146961cdd5ca")
                                                {
                                                    pr.id = pgx.id;
                                                    pr.name = pgx.name;
                                                    pr.number = pgx.number;
                                                    pr.value = Convert.ToDecimal(value);
                                                    pr.validFrom = validFrom;

                                                    PriceResult.Add(pr);

                                                    if (PriceResult.Count > 0)
                                                    {
                                                        var MaxvalidFrom = PriceResult.Max(a => a.validFrom);
                                                        var FinalPrice = PriceResult.Where(a => a.validFrom == MaxvalidFrom).ToList();
                                                        var Price = FinalPrice[0].value;
                                                        if (FinalPrice.Count == 2)
                                                        {
                                                            if (FinalPrice[1].name == "Member")
                                                            {
                                                                var sprice = FinalPrice[1].value;
                                                                finalResult.sprice = sprice.ToString();
                                                                if (sprice > 0)
                                                                {
                                                                    finalResult.start = DateTime.Now.ToString("MM/dd/yyyy") + " 0700";
                                                                    finalResult.end = "12/12/2229 2400";
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                        }
                        else
                        {
                            var p = dataitem.prices;
                            if (p != null)
                            {
                                var pArraycount = p.Count;
                                PriceResult = new List<QuantityPrice>();
                                for (int j = 0; j < pArraycount; j++)
                                {
                                    var val1 = p[j];
                                    var pr = new QuantityPrice();
                                    var value = val1.value;
                                    var validFrom = val1.validFrom;

                                    var pgx = val1.priceGroup;

                                    if (pgx != null)
                                    {

                                        if (pgx.id == StorePriceGroupId)
                                        {
                                            pr.id = pgx.id;
                                            pr.name = pgx.name;
                                            pr.number = pgx.number;
                                            pr.value = Convert.ToDecimal(value);
                                            pr.validFrom = validFrom;

                                            PriceResult.Add(pr);
                                        }
                                        if (sprice.Contains(StoreId.ToString())) //for sprice 11480
                                        {
                                            if (pgx.id == "27e64588-aee4-45b7-a9d6-146961cdd5ca")
                                            {
                                                pr.id = pgx.id;
                                                pr.name = pgx.name;
                                                pr.number = pgx.number;
                                                pr.value = Convert.ToDecimal(value);
                                                pr.validFrom = validFrom;

                                                PriceResult.Add(pr);

                                                if (PriceResult.Count > 0)
                                                {
                                                    var MaxvalidFrom = PriceResult.Max(a => a.validFrom);
                                                    var FinalPrice = PriceResult.Where(a => a.validFrom == MaxvalidFrom).ToList();
                                                    var Price = FinalPrice[0].value;
                                                    if (FinalPrice.Count == 2)
                                                    {
                                                        if (FinalPrice[1].name == "Member")
                                                        {
                                                            var sprice = FinalPrice[1].value;
                                                            finalResult.sprice = sprice.ToString();
                                                            if (sprice > 0)
                                                            {
                                                                finalResult.start = DateTime.Now.ToString("MM/dd/yyyy") + " 0700";
                                                                finalResult.end = "12/12/2229 2400";
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (PriceResult.Count > 0)
                        {
                            var MaxvalidFrom = PriceResult.Max(a => a.validFrom);
                            var FinalPrice = PriceResult.Where(a => a.validFrom == MaxvalidFrom).ToList();
                            var Price = FinalPrice[0].value;

                            finalResult.price = Price;
                            fullname.Price = Price;
                            finalResultList.Add(finalResult);
                            fullnameList.Add(fullname);
                        }

                    }
                }
                if (!stocklist.Contains(StoreId.ToString()))
                {
                    KoronaStockList(StoreId);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("Generating Korona " + StoreId + " Product CSV File......");
            Console.WriteLine("Generating Korona " + StoreId + " FullName CSV File......");
            //var fResult = finalResultList.Where(a => a.price > 0).ToList();
            if (AllowCBD.Contains(StoreId.ToString()))
            {
                var PResult = finalResultList.Where(a => a.price > 0).ToList();
                var FResult = fullnameList.Where(a => a.Price > 0).ToList();
                GenerateCSVFile.GenerateCSVFiles(PResult, "PRODUCT", StoreId, folderPath);
                GenerateCSVFile.GenerateCSVFiles(FResult, "FULLNAME", StoreId, folderPath);
            }
            else
            {
                var PResult = finalResultList.Where(a => a.price > 0).ToList(); //a.qty > 0 &&
                var FResult = fullnameList.Where(a => a.Price > 0 && a.pcat.ToUpper() != "CIGAR" && a.pcat.ToUpper() != "CIGARETTES" && a.pcat.ToUpper() != "Tobacco" && a.pcat.ToUpper() != "CIGARS").ToList();
                PResult = PResult.Where(w => FResult.Any(f => f.sku == w.sku)).ToList();
                if (catfilter.Contains(StoreId.ToString()))
                {
                    PResult = finalResultList.Where(a => a.price > 0).ToList();
                    FResult = fullnameList.Where(a => a.Price > 0 && a.pcat.ToUpper() != "CIGAR" && a.pcat.ToUpper() != "CIGARETTES" && a.pcat.ToUpper() != "Tobacco").ToList();
                    PResult = PResult.Where(w => FResult.Any(f => f.sku == w.sku)).ToList();
                }
                if (staticqty.Contains(StoreId.ToString()))
                {
                    PResult = finalResultList.Where(a => a.price > 0 && a.qty > 0).ToList();
                    FResult = fullnameList.Where(a => a.Price > 0 && a.pcat.ToUpper() != "CIGAR" && a.pcat.ToUpper() != "CIGARETTES" && a.pcat.ToUpper() != "Tobacco").ToList();
                    FResult = FResult.Where(x => PResult.Any(y => y.sku == x.sku)).ToList();
                    PResult = PResult.Where(w => FResult.Any(f => f.sku == w.sku)).ToList();
                }

                GenerateCSVFile.GenerateCSVFiles(PResult, "PRODUCT", StoreId, folderPath);
                GenerateCSVFile.GenerateCSVFiles(FResult, "FULLNAME", StoreId, folderPath);
            }
            Console.WriteLine("Product File Generated For " + StoreId);
            Console.WriteLine("FULL Name File Generated For " + StoreId);
        }
        private void KoronaStockList(int StoreId)
        {
            foreach (var item in stkModel)
            {
                foreach (var dataitem in item.results)
                {
                    var amount = dataitem.amount;
                    var product = dataitem.product;
                    if (amount.actual > 9999)
                    {
                        continue;
                    }
                    foreach (var Litem in finalResultList)
                    {
                        if (Nonstocklistqty.Contains(StoreId.ToString())) // For Store 11404 tktno:15856
                        {
                            Litem.qty = 999;
                        }
                        else if (Litem.Productid == product.id.ToString())
                        {
                            if (difqty.Contains(StoreId.ToString()) && Litem.cat != null && Litem.cat.ToUpper() == "BEER")
                            {
                                continue;
                            }
                            else if (staticqty.Contains(StoreId.ToString()))  //tckt8895
                            {
                                Litem.qty = Convert.ToInt32(amount.actual);
                            }
                            else
                            {
                                Litem.qty = Convert.ToInt32(amount.actual) > 0 ? Convert.ToInt32(amount.actual) : 0;
                            }
                        }

                    }
                }
            }
        }
        public int getpack(string prodName)
        {
            prodName = prodName.ToUpper();
            var regexMatch = Regex.Match(prodName, @"(?<Result>\d+)PK");
            var prodPack = regexMatch.Groups["Result"].Value;
            if (prodPack.Length > 0)
            {
                int outVal = 0;
                int.TryParse(prodPack.Replace("$", ""), out outVal);
                return outVal;
            }
            return 1;
        }
        public string getVolume(string prodName)
        {
            prodName = prodName.ToUpper();
            var regexMatch = Regex.Match(prodName, @"(?<Result>\d+)ML| (?<Result>\d+)LTR| (?<Result>\d+)OZ | (?<Result>\d+)L|(?<Result>\d+)\sOZ");
            var prodPack = regexMatch.Groups["Result"].Value;
            if (prodPack.Length > 0)
            {
                return regexMatch.ToString();
            }
            return "";
        }
    }
}
