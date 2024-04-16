using Korona.Model;
using System;
using System.Configuration;

namespace Korona
{
    class Program
    {
        static void Main(string[] args)
        {
            string KoronaApiUrl = ConfigurationManager.AppSettings["KoronaApiUrl"];
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
                        if (itm.StoreSettings.StoreId == 11962)
                        {
                            merchantId = itm.StoreSettings.POSSettings.merchanId;
                            accessToken = itm.StoreSettings.POSSettings.tokenid;
                            clsKorona obj = new clsKorona();                       
                            obj.DeleteProductPath();
                            obj.CreateKoronaProductResponseFile("web/api/v3/" + KoronaApiUrl + itm.StoreSettings.POSSettings.merchanId + "products?includeDeleted=false", itm.StoreSettings.POSSettings.tokenid);
                            obj.setupenv();
                            obj.CreateKoronaStockResponseFile("web/api/v3/" + KoronaApiUrl + itm.StoreSettings.POSSettings.merchanId + itm.StoreSettings.POSSettings.OrganisationalId, itm.StoreSettings.POSSettings.tokenid);
                            obj.KoronaProductDetails(itm.StoreSettings.POSSettings.tax, itm.StoreSettings.POSSettings.StorePriceGroupId, itm.StoreSettings.StoreId);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    finally { }
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