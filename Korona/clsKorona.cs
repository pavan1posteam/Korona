using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Korona.Model;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Reflection;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;

namespace Korona
{
    public class clsKorona
    {
        public string organizationalUnits { get; set; }

        List<FinalResult> finalResultList = new List<FinalResult>();
        List<Fullname> fullnameList = new List<Fullname>();
        List<QuantityPrice> PriceResult = new List<QuantityPrice>();
        List<codes> CodeResult = new List<codes>();

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

        List<ResponseModel> resmdl = new List<ResponseModel>();
        List<StockResponseModel> stkModel = new List<StockResponseModel>();


        string pathProduct = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"ProductDetails_Korona.json");
        string pathStock = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"KoronaProductStock.json");

        public clsKorona()
        {
            if (File.Exists(pathProduct))
            {
                File.Delete(pathProduct);
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
        public void DeleteProductPath()
        {
            if (File.Exists(pathProduct))
            {
                File.Delete(pathProduct);
            }
        }


        public string getProduct(string param, string accessToken)
        {
            ResponseModel mdl = new ResponseModel();
            var client = new RestClient(baseUrl + param);
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", accessToken);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            IRestResponse response = client.Execute(request);
            string content = response.Content;
            mdl = JsonConvert.DeserializeObject<ResponseModel>(content);
            resmdl.Add(mdl);
            return content;
        }
        public string getStock(string param, string accessToken)
        {
            StockResponseModel stk = new StockResponseModel();
            var client = new RestClient(baseUrl + param);
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", accessToken);
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

            return content;

        }
        public void CreateKoronaProductResponseFile(string url, string accessToken)
        {
            string content = getProduct(url, accessToken);
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
                        CreateKoronaProductResponseFile(item.Value.ToString(), accessToken);
                    }
                    break;
                }
            }
        }
        public void CreateKoronaStockResponseFile(string url, string accessToken)
        {
            string content = getStock(url, accessToken);
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
                        CreateKoronaStockResponseFile(item.Value.ToString(), accessToken);
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
                        else
                        {
                            upc = dataitem.codes == null ? "" : dataitem.codes.FirstOrDefault().productCode;

                        }
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
                        else { finalResult.tax = tax; }
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

                        finalResult.storeid = StoreId;
                        if (!string.IsNullOrEmpty(upc))
                        {
                            finalResult.upc = "#" + upc.ToString().Trim();
                            fullname.upc = "#" + upc.ToString().Trim();
                            finalResult.sku = "#" + dataitem.number.Trim();
                            fullname.sku = "#" + dataitem.number.Trim();

                        }
                        else if (upcnull.Contains(StoreId.ToString()))
                        {
                            finalResult.upc = "#" + dataitem.number.Trim();
                            fullname.upc = finalResult.upc;
                            finalResult.sku = finalResult.upc;
                            fullname.sku = finalResult.upc;

                        }
                        else
                        { continue; }

                        if (different_sku.Contains(StoreId.ToString()))
                        {
                            if (!string.IsNullOrEmpty(upc))
                            {
                                fullname.sku = "#" + upc.ToString().Trim();
                                finalResult.sku = "#" + upc.ToString().Trim();
                            }
                            else
                            { continue; }
                        }
                        //if (Regex.IsMatch(fullname.sku, @"[A-Z]|[a-z]"))
                        //    continue;
                        fullname.pcat1 = "";
                        fullname.pcat2 = "";
                        finalResult.pack = 1;
                        fullname.pack = 1;
                        finalResult.uom = "";
                        fullname.uom = "";
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
            var PResult = finalResultList.Where(a => a.price > 0).ToList(); //a.qty > 0 &&
            var FResult = fullnameList.Where(a => a.Price > 0 && a.pcat.ToUpper() != "CIGAR" && a.pcat.ToUpper() != "CIGARETTES" && a.pcat.ToUpper() != "Tobacco" && a.pcat.ToUpper() != "CIGARS").ToList();
            PResult = PResult.Where(w => FResult.Any(f => f.sku == w.sku)).ToList();
            if (catfilter.Contains(StoreId.ToString()))
            {
                PResult = finalResultList.Where(a => a.price > 0 ).ToList();
                FResult = fullnameList.Where(a => a.Price > 0 && a.pcat.ToUpper() != "CIGAR" && a.pcat.ToUpper() != "CIGARETTES" && a.pcat.ToUpper() != "Tobacco").ToList();
                PResult = PResult.Where(w => FResult.Any(f => f.sku == w.sku)).ToList();
            }
            if (staticqty.Contains(StoreId.ToString()))
            {
                PResult = finalResultList.Where(a => a.price > 0 && a.qty > 0).ToList();
                FResult = fullnameList.Where(a => a.Price > 0  && a.pcat.ToUpper() != "CIGAR" && a.pcat.ToUpper() != "CIGARETTES" && a.pcat.ToUpper() != "Tobacco").ToList();
                FResult = FResult.Where(x => PResult.Any(y => y.sku == x.sku)).ToList();
                PResult = PResult.Where(w => FResult.Any(f => f.sku == w.sku)).ToList();
            }

            GenerateCSVFile.GenerateCSVFiles(PResult, "PRODUCT", StoreId, folderPath);
            GenerateCSVFile.GenerateCSVFiles(FResult, "FULLNAME", StoreId, folderPath);
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
                    var product = dataitem.product; ;
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
                            if (difqty.Contains(StoreId.ToString()) && Litem.cat!=null && Litem.cat.ToUpper() == "BEER")
                            {
                                continue;
                            }
                            else if (staticqty.Contains(StoreId.ToString()))  ///////tckt8895
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
    }
}
