using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace WebApplication2.Controllers
{
    public class PartController : Controller
    {
        public IDbConnection Connection { get; }
        private readonly AppOptions options;
        private string connectionString;

        public PartController(IOptions<AppOptions> options)
        {
            // Connection = new SqlConnection("data source=ZALNT254;initial catalog=PTLBGE;persist security info=True;user id=web;password=connect!;App=FlexPTLBGEWeb");
            //Connection.Open();
            this.options = options.Value;
            connectionString = this.options.DefaultConnection;
        }

        [HttpPost]
        // public Part GetProducts([FromBody] string partNumber)
        public List<Part> GetProducts()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                // DynamicParameters parameters = new DynamicParameters();       
                // parameters.Add("@PartNumber", partNumber);
                List<Part> result = connection.Query<Part>("SELECT * FROM Part WHERE PartFamilyID = (SELECT ID FROM PartFamily WHERE PartFamilyName = 'Product')").ToList();
                return result;
            }
        }

        [HttpPost]
        public Product createSN([FromBody] Part p){           
            using (SqlConnection connection = new SqlConnection(connectionString)) {
                Product pr = new Product();
                pr.SerialNumber=System.DateTime.Now.ToString("yyyyMMddHHmmss");
                pr.PartID=p.ID;
	            pr.CreationTime=System.DateTime.Now;    
                var id = connection.Insert(pr);
                
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@SerialNumber", pr.SerialNumber);
                parameters.Add("@partID", p.ID);
                var sql = "INSERT INTO [Assembly] SELECT @SerialNumber, PartID, AssemblyOrder, null FROM [BOM] WHERE ParentID = @partID";
                connection.Execute(sql, parameters);

                return connection.Get<Product>(id);
            }  
        }
    }
}