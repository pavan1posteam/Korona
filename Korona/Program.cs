using Korona.Model;
using System;
using System.Configuration;

namespace Korona
{
    // pushing to git on date 31/10/2025 and from github ui too
    class Program
    {
        static void Main(string[] args)//Testing by jacob
        {
            string KoronaApiUrl = ConfigurationManager.AppSettings["KoronaApiUrl"];
            string KoronaBaseUrl = ConfigurationManager.AppSettings["KoronaBaseUrl"];
            string KoronaApiUrl_185 = ConfigurationManager.AppSettings["KoronaBaseUrl185"];
            string diiffer_Url = ConfigurationManager.AppSettings["diiffer_Url"];
            string DeveloperId = ConfigurationManager.AppSettings["DeveloperId"];
            StoreSettings storeSetting = new StoreSettings();
            storeSetting.IntializeStoreSettings();
            string merchantId = "";
            string accessToken = "";
            try
            {
                foreach (var itm in storeSetting.PosDetails)
                {
                    try
                    {
                        if (itm.StoreSettings.StoreId == 12383)
                        {

                        }
                        else
                        {
                            continue;
                        }
                        if (string.IsNullOrEmpty(itm.StoreSettings.POSSettings.BaseUrl))
                        {
                            if (diiffer_Url.Contains(itm.StoreSettings.StoreId.ToString()))
                            {
                                if (itm.StoreSettings.StoreId != 11950)
                                {
                                    merchantId = itm.StoreSettings.POSSettings.merchanId;
                                    accessToken = itm.StoreSettings.POSSettings.tokenid;
                                    clsKorona obj = new clsKorona();
                                    obj.DeleteProductPath();
                                    obj.CreateKoronaProductResponseFile("web/api/v3/" + KoronaApiUrl + itm.StoreSettings.POSSettings.merchanId + "products?includeDeleted=false", itm.StoreSettings.POSSettings.tokenid, KoronaApiUrl_185);
                                    obj.setupenv();
                                    obj.CreateKoronaStockResponseFile("web/api/v3/" + KoronaApiUrl + itm.StoreSettings.POSSettings.merchanId + itm.StoreSettings.POSSettings.OrganisationalId, itm.StoreSettings.POSSettings.tokenid, KoronaApiUrl_185);
                                    obj.DeleteTaxPath();
                                    obj.CreateKoronaTaxResponseFile("web/api/v3/" + KoronaApiUrl + itm.StoreSettings.POSSettings.merchanId + "/salesTaxes", itm.StoreSettings.POSSettings.tokenid, KoronaApiUrl_185);
                                    obj.KoronaProductDetails(itm.StoreSettings.POSSettings.tax, itm.StoreSettings.POSSettings.StorePriceGroupId, itm.StoreSettings.StoreId);
                                }
                            }
                            else
                            {
                                merchantId = itm.StoreSettings.POSSettings.merchanId;
                                accessToken = itm.StoreSettings.POSSettings.tokenid;
                                clsKorona obj = new clsKorona();
                                obj.DeleteProductPath();
                                obj.CreateKoronaProductResponseFile("web/api/v3/" + KoronaApiUrl + itm.StoreSettings.POSSettings.merchanId + "products?includeDeleted=false", itm.StoreSettings.POSSettings.tokenid, KoronaBaseUrl);
                                obj.setupenv();
                                obj.CreateKoronaStockResponseFile("web/api/v3/" + KoronaApiUrl + itm.StoreSettings.POSSettings.merchanId + itm.StoreSettings.POSSettings.OrganisationalId, itm.StoreSettings.POSSettings.tokenid, KoronaBaseUrl);
                                obj.KoronaProductDetails(itm.StoreSettings.POSSettings.tax, itm.StoreSettings.POSSettings.StorePriceGroupId, itm.StoreSettings.StoreId);
                            }
                        }
                        else
                        {
                            merchantId = itm.StoreSettings.POSSettings.merchanId;
                            accessToken = itm.StoreSettings.POSSettings.tokenid;
                            clsKorona2 obj = new clsKorona2(itm.StoreSettings.StoreId, itm.StoreSettings.POSSettings.BaseUrl, merchantId, accessToken, itm.StoreSettings.POSSettings.StorePriceGroupId, itm.StoreSettings.POSSettings.OrganisationalId, itm.StoreSettings.POSSettings.tax, itm.StoreSettings.POSSettings.deposit);
                            Console.WriteLine();
                        }
                    }
                    catch (Exception ex)
                    {
                       Console.WriteLine(ex.Message);
                    }
                    finally 
                    { 
                    }
                }
                
            }
            catch (Exception ex)
            {
                new clsEmail().sendEmail(DeveloperId, "", "", "Error in Korona Program.cs @" + DateTime.UtcNow + " GMT", ex.Message + "<br/>" + ex.StackTrace);
                Console.WriteLine(ex.Message);
            }
            finally { }
        }
    }
}
