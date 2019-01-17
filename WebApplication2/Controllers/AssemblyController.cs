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
    public class AssemblyController : Controller
    {
        public IDbConnection Connection { get; }
        private readonly AppOptions options;
        private string connectionString;

        public AssemblyController(IOptions<AppOptions> options)
        {           
            this.options = options.Value;
            connectionString = this.options.DefaultConnection;
        }

        
        [HttpPost]       
        public bool assemblyPart( string sn)
        {
            if(sn != null ){
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DynamicParameters parameters = new DynamicParameters();                   
                    parameters.Add("@SerialNumber", sn);
                    parameters.Add("@now", System.DateTime.Now);
                    var sql = "UPDATE Assembly SET AssembledTime = @now WHERE ID = (SELECT TOP 1 ID FROM  Assembly WHERE AssembledTime IS NULL and SerialNumber = @SerialNumber order by AssemblyOrder)";
                    connection.Execute(sql, parameters); 
               

                    return true;   
                }
            }else{
                return false;
            }
        }
        
         [HttpPost]       
        public Part getNextBom( Product p)
        {
            try{
                if(p != null && p.SerialNumber.Length>0){
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        DynamicParameters parameters = new DynamicParameters();                  
                        parameters.Add("@SerialNumber",  p.SerialNumber);
                        var sql = "SELECT p.* FROM [Assembly] a INNER JOIN Part p ON p.ID = a.AssembledPartID WHERE a.SerialNumber = @SerialNumber and a.AssembledTime IS NULL order by a.AssemblyOrder";                        
                        Part mypart = connection.QueryFirst<Part>(sql, parameters);                       
                        return mypart;   
                    }
                }else{
                    return null;
                }
            }catch(System.Exception){
                return null;
            }
        }

         [HttpPost]       
        public int getProgress( Product p)
        {
            if(p != null && p.SerialNumber.Length>0){
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DynamicParameters parameters = new DynamicParameters();                  
                    parameters.Add("@SerialNumber", p.SerialNumber);
                    var sql = "SELECT CAST((SELECT Count(*) FROM [Assembly] WHERE [AssembledTime] IS NOT NULL and SerialNumber = @SerialNumber)AS float)/CAST((SELECT Count(*) FROM [Assembly] WHERE SerialNumber = @SerialNumber)AS float) * 100 as result";
                    int r = connection.QueryFirst<int>(sql, parameters);
                    if (r == 100)
                    {
                        p.StationID = 3;
                        connection.Update(p);
                    }
                    return r;   
                }
            }else{
                return -1;
            }
        }
    }
}