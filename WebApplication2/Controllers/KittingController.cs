using Microsoft.AspNetCore.Mvc;
using PickToLight.Models;
using System.Data;
using System.Data.SqlClient;
using System;
using System.Linq;
using Dapper;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO.Ports;


namespace PickToLight.Controllers
{
    public class KittingController : Controller
    {
        public IDbConnection Connection { get; }
        private readonly AppOptions options;
        private string connectionString;
        private SerialPort _serialPort = null;         
        private const int BaudRate = 9600;
        private const string SerialLine = "COM1";
        public double partWeight;
       

        public KittingController(IOptions<AppOptions> options)
        {            
            this.options = options.Value;
            connectionString = this.options.DefaultConnection;
        }

        public Part getWeightBySN(string sn)
        {
            try
            {
                if (sn != null && sn.Length > 0)
                {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@SerialNumber", sn);
                    var sql = "  SELECT p.* FROM Part p JOIN Product pr ON p.ID=pr.PartID WHERE SerialNumber=@SerialNumber";
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

        public double readWeight()
        {
            _serialPort = new SerialPort(SerialLine, BaudRate);

            try
            {
                _serialPort.Open();
                var weight = _serialPort.ReadLine();
                _serialPort.Close();
                char[] delimiterchars = { '+', 'g' };
                string[] weightList = weight.Split(delimiterchars);
                double partWeight = double.Parse(weightList[1]);
                return partWeight;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }

        }

        public bool setWeight(string sn, double w)
        {
            try
            {
                if (sn != null && sn.Length > 0)
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@SerialNumber", sn);
                        parameters.Add("@Weight", w);
                        var sql = "UPDATE Product SET Weight=@Weight, StationID=2 WHERE SerialNumber=@SerialNumber";
                        connection.Execute(sql, parameters);
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
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
                    List<Part> required = connection.Query<Part>("SELECT p.PartName, SUM(1) AS Qty FROM Product pr INNER JOIN BOM b ON pr.PartID = b.ParentID INNER JOIN Part p ON p.ID = b.PartID WHERE pr.SerialNumber = @SerialNumber group by p.PartName", parameters).ToList();
                    List<Part> available = connection.Query<Part>("SELECT PartName, Qty FROM Part").ToList();

                    foreach (var item in required)
                    {
                        foreach (var a in available)
                        {
                            if (item.PartName == a.PartName && item.Qty > a.Qty) { return null; }
                        }

                    }


                    foreach (var item in required)
                    {
                        DynamicParameters parameters2 = new DynamicParameters();
                        parameters2.Add("@Qty", item.Qty);
                        parameters2.Add("@PN", item.PartName);
                        var sql = "UPDATE Part SET Qty = Qty-@Qty WHERE PartName = @PN";
                        connection.Execute(sql, parameters2);

                    }
                
                    List<Part> result = connection.Query<Part>("SELECT p.* FROM Product pr INNER JOIN BOM b ON pr.PartID = b.ParentID INNER JOIN Part p ON p.ID = b.PartID WHERE pr.SerialNumber = @SerialNumber order by b.AssemblyOrder", parameters).ToList();

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