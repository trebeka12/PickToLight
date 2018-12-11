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
    public class ProductController : Controller{
        public IDbConnection Connection { get; }
        private readonly AppOptions options;
        private string connectionString;

        public ProductController(IOptions<AppOptions> options)
        {           
            this.options = options.Value;
            connectionString = this.options.DefaultConnection;
        }

        [HttpPost]       
        public Product packSN(string s)
        {
            try{
                if(s != null && s.Length>0){
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@SerialNumber", s);
                        var sql = "SELECT * FROM Product WHERE SerialNumber = @SerialNumber";
                        Product myproduct = connection.QueryFirst<Product>(sql, parameters);
                        connection.Update(new Product() { ID = myproduct.ID, SerialNumber=myproduct.SerialNumber, PartID=myproduct.PartID, CreationTime=myproduct.CreationTime, IsComplete = true, StationID= 4 });   
                        return myproduct;
                    }
                }else{
                    return null;
                }
            }catch(Exception) {
                return null;
            }
        }

        [HttpPost]       
        public Product getProductBySN(string serialnum)
        {
            try{
                if(serialnum != null && serialnum.Length>0){
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@SerialNumber", serialnum);
                        var sql = "SELECT * FROM Product WHERE SerialNumber = @SerialNumber";
                        Product myproduct = connection.QueryFirst<Product>(sql, parameters);                       
                        return myproduct;   
                    }
                }else{
                    return null;
                }
            }catch(Exception){
                return null;
            }
        }
        
        [HttpPost]       
        public Part getImageUrlBySN( Product p)
        {
            try{
                if(p != null && p.SerialNumber.Length>0){
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        DynamicParameters parameters = new DynamicParameters();
                        string serialnumber = p.SerialNumber;
                        parameters.Add("@SerialNumber", serialnumber);
                        var sql = "SELECT PartName FROM Product pr INNER JOIN Part p ON pr.PartID = p.ID WHERE Serialnumber = @SerialNumber";
                        Part mypart = connection.QueryFirst<Part>(sql, parameters);                       
                        return mypart;   
                    }
                }else{
                    return null;
                }
            }catch(Exception){
                return null;
            }
        }
        
        
    }
}