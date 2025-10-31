using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Configuration;

namespace Korona.Model
{
    class GenerateCSVFile
    {
        string discountable = ConfigurationManager.AppSettings["dicountable"];
        
        public static string GenerateCSVFiles<T>(IList<T> list, string Name, int StoreId, string BaseUrl)
        {
            string differentfile = ConfigurationManager.AppSettings["differentfile"];

            if (list == null || list.Count == 0) return "Quantity and Price are 0 or less than 0";
            if (!Directory.Exists(BaseUrl + "\\" + StoreId + "\\Upload\\"))
            {
                Directory.CreateDirectory(BaseUrl + "\\" + StoreId + "\\Upload\\");
            }
            string filename = Name + StoreId + DateTime.Now.ToString("yyyyMMddhhmmss") + ".csv";
            string fcname = BaseUrl + "\\" + StoreId + "\\Upload\\" + filename;
            // Console.WriteLine("Generating " + filename + " ........");
            //File.WriteAllText(BaseUrl + "\\" + StoreId + "\\Upload\\" + filename, csvData.ToString());
            // return filename;

            //get type from 0th member
            Type t = list[0].GetType();
            string newLine = Environment.NewLine;

            using (var sw = new StreamWriter(fcname))
            {
                //make a new instance of the class name we figured out to get its props
                object o = Activator.CreateInstance(t);
                //gets all properties
                PropertyInfo[] props = o.GetType().GetProperties();

                //foreach of the properties in class above, write out properties
                //this is the header row
                foreach (PropertyInfo pi in props)
                {
                    if (pi.Name != "Productid" && pi.Name != "cat" && differentfile.Contains(StoreId.ToString()))
                    {
                        sw.Write(pi.Name + ",");
                    }
                    else if (pi.Name != "Productid" && pi.Name != "cat" && pi.Name != "Discountable")
                    {
                        sw.Write(pi.Name + ",");
                    }

                }
                sw.Write(newLine);

                //this acts as datarow
                foreach (T item in list)
                {
                    //this acts as datacolumn
                    foreach (PropertyInfo pi in props)
                    {
                        if (pi.Name != "Productid" && pi.Name != "cat" && differentfile.Contains(StoreId.ToString()))
                        {
                            //this is the row+col intersection (the value)
                            string whatToWrite =
                                Convert.ToString(item.GetType()
                                                     .GetProperty(pi.Name)
                                                     .GetValue(item, null))
                                    .Replace(',', ' ') + ',';

                            sw.Write(whatToWrite.Trim());
                        }
                        else if (pi.Name != "Productid" && pi.Name != "cat" && pi.Name != "Discountable")
                        {
                            //this is the row+col intersection (the value)
                            string whatToWrite =
                                Convert.ToString(item.GetType()
                                                     .GetProperty(pi.Name)
                                                     .GetValue(item, null))
                                    .Replace(',', ' ') + ',';

                            sw.Write(whatToWrite.Trim());
                        }
                    }
                    sw.Write(newLine);
                }
                return filename;
            }
        }
    }
}
