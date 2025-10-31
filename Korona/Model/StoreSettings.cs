using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korona.Model
{
    public class StoreSettings
    {
        public List<POSSetting> PosDetails { get; set; }
        public void IntializeStoreSettings()
        {
            DataSet dsResult = new DataSet();
            List<POSSetting> posdetails = new List<POSSetting>();
            try
            {
                List<SqlParameter> sparams = new List<SqlParameter>();
                sparams.Add(new SqlParameter("@PosId", 8));
                string constr = ConfigurationManager.AppSettings.Get("LiquorAppsConnectionString");
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "usp_ts_GetStorePosSetting";
                       // cmd.Parameters.Add(sparams[0]);
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter())
                        {
                            da.SelectCommand = cmd;
                            da.Fill(dsResult);
                        }
                    }
                }
                if (dsResult != null || dsResult.Tables.Count > 0)
                {
                    foreach (DataRow dr in dsResult.Tables[0].Rows)
                    {
                        if (dr["PosName"].ToString().ToUpper() == "KORONA")
                        {
                            POSSetting pobj = new POSSetting();
                            pobj.Setting = dr["Settings"].ToString();
                            StoreSetting obj = new StoreSetting();
                            obj.StoreId = Convert.ToInt16(dr["StoreId"] == DBNull.Value ? 0 : dr["StoreId"]);
                            obj.POSSettings = JsonConvert.DeserializeObject<Setting>(pobj.Setting);
                            if(string.IsNullOrEmpty(pobj.Setting))
                            {
                                continue;
                            }
                            pobj.PosName = dr["PosName"].ToString();
                            pobj.PosId = Convert.ToInt32(dr["PosId"]);
                            pobj.StoreSettings = obj;
                            posdetails.Add(pobj);
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                PosDetails = posdetails;                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.Read();
            }
        }
    public class POSSetting   
    {
        public int PosId { get; set; }
        public string PosName { get; set; }
        public StoreSetting StoreSettings { get; set; }
        public string Setting { get; set; }
    }
    public class Setting {
        public string ClientId { get; set; }
        public string merchanId { get; set; }
        public string Code { get; set; }
        public string tokenid { get; set; }
        public string instock { get; set; }
        public string categories { get; set; }
        public string tax { get; set; }
        public string PosFileName { get; set; }
        public string APIKey { get; set; }
        public int StoreMapId { get; set; }
        public decimal liquortax { get; set; }
        public decimal liquortaxrateperlitre { get; set; }
        public string StorePriceGroupId { get; set; }
        public string OrganisationalId { get; set; }
        public string BaseUrl { get; set; }
        public decimal deposit { get; set; }
    }
    public class StoreSetting {
        public int StoreId { get; set; }
        public Setting POSSettings { get; set; }          
    }
    public class storecat
    {
        public string catid { get; set; }
        public string catname { get; set; }
    }
    }
}
