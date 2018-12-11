using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using System.Data;
using System.Data.SqlClient;
using System;
using System.Linq;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;


namespace WebApplication2.Controllers
{
    public class KittingController : Controller
    {
        public IDbConnection Connection { get; }
        private readonly AppOptions options;
        private string connectionString;

        public KittingController(IOptions<AppOptions> options)
        {            
            //Connection.Open();
            this.options = options.Value;
            connectionString = this.options.DefaultConnection;
        }      

        [HttpPost]      
        public List<Part> GetBoms( Product p)
        {
            try{
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DynamicParameters parameters = new DynamicParameters();       
                    parameters.Add("@SerialNumber", p.SerialNumber);
                    List<Part> result = connection.Query<Part>("SELECT p.* FROM Product pr INNER JOIN BOM b ON pr.PartID = b.ParentID INNER JOIN Part p ON p.ID = b.PartID WHERE pr.SerialNumber = @SerialNumber order by b.AssemblyOrder", parameters).ToList();

                    p.StationID = 2;
                    connection.Update(p);
                    for (var y = 0; y< result.Count; y++)
                         {
                                result[y].Qty = result[y].Qty - 1;
                                connection.Update(result[y]);
                          }
                            return result;
                     }

            }catch(Exception e){
              System.Console.WriteLine(e);
                return new List<Part>();

            }
        }
        
        [HttpPost]      
        public bool InsertToPTL( List<Part> p)
        {           
            try{
                string sql = "dbo.ptlAddRequest";
                string xml_param="<ptl><application><name>PTLBGE</name><version>1.0</version></application><signal>PICK</signal><signalref>N/A</signalref>";
                xml_param+="<request><rpos>1</rpos><line>Line1</line><zone>Zone1</zone><uniqid>1</uniqid><description>-</description>";
                
                Dictionary<string, int> mydictionary = new Dictionary<string, int>();

                foreach (Part mypart in p)
                {
                    if(!mydictionary.ContainsKey(mypart.Code)){
                        mydictionary.Add(mypart.Code,1);
                    }else{
                        int value = mydictionary[mypart.Code];
                        value++;
                        mydictionary[mypart.Code]=value;
                    }
                }
               
               foreach (var pair in mydictionary)
                {
                    xml_param+="<data><item>"+pair.Key+"</item><qty>"+pair.Value+"</qty></data>";
                }
                
                xml_param+="</request></ptl>";
                var param = new DynamicParameters();
                param.Add("@Request", xml_param);

               
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var affectedRows = connection.Execute(sql, param, commandType: CommandType.StoredProcedure);
                    return affectedRows>0;                    
                }
            }catch(Exception e){
                System.Console.WriteLine(e.Message);
                return false;
            }
        }
    }
}