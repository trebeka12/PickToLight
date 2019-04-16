using Microsoft.AspNetCore.Mvc;
using PickToLight.Models;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PickToLight.Controllers
{
    public class PartController : Controller
    {
        public IDbConnection Connection { get; }
        private readonly AppOptions options;
        private string connectionString;

        public PartController(IOptions<AppOptions> options)
        {
            this.options = options.Value;
            connectionString = this.options.DefaultConnection;
        }

        [HttpPost]
        public List<Part> GetProducts()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                List<Part> result = connection.Query<Part>("SELECT * FROM Part WHERE PartFamilyID = (SELECT ID FROM PartFamily WHERE PartFamilyName = 'Product')").ToList();
                return result;
            }
        }

        [HttpPost]
        public Part getPartByPN(string partnum)
        {
            try
            {
                if (partnum != null && partnum.Length > 0)
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@Code", partnum);
                        var sql = "SELECT * FROM Part WHERE Code = @Code";
                        Part mypart = connection.QueryFirst<Part>(sql, parameters);
                        return mypart;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }


        [HttpPost]
        public Product createSN(int param)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                Product pr = new Product();
                pr.SerialNumber = System.DateTime.Now.ToString("yyyyMMddHHmmss");
                pr.PartID = param;
                pr.CreationTime = System.DateTime.Now;
                pr.StationID = 1;
                var id = connection.Insert(pr);
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@SerialNumber", pr.SerialNumber);
                parameters.Add("@partID", param);
                var sql = "INSERT INTO [Assembly] SELECT @SerialNumber, PartID, AssemblyOrder, null FROM [BOM] WHERE ParentID = @partID";
                var r = connection.Execute(sql, parameters);

                return connection.Get<Product>(id);
            }
        }

        [HttpPost]
        public bool fillPart(string p, int quantity)
        {
            if (p != null && quantity != 0)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@Qty", quantity);
                    parameters.Add("@PartNumber", p);
                    var sql = "UPDATE Part SET Qty= Qty+@Qty WHERE Code=@PartNumber";
                    connection.Execute(sql, parameters);

                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public bool InsertToPTL(Part p, int qty)
        {
            try
            {
                string sql = "dbo.ptlAddRequest";
                string xml_param = "<ptl><application><name>PTLBGE</name><version>1.0</version></application><signal>PUT</signal><signalref>N/A</signalref>";
                xml_param += "<request><rpos>1</rpos><line>Line1</line><zone>Zone1</zone><uniqid>1</uniqid><description>-</description>";

                Dictionary<string, int> mydictionary = new Dictionary<string, int>();

                xml_param += "<data><item>" + p.Code + "</item><qty>" + qty + "</qty></data>";
                xml_param += "</request></ptl>";
                var param = new DynamicParameters();
                param.Add("@Request", xml_param);


                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    connection.Execute(sql, param, commandType: CommandType.StoredProcedure);
                    return true;
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
                return false;
            }
        }
    }
}