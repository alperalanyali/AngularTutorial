using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using WebapiforAngular.Models;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Net.Mail;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebapiforAngular.Controllers
{
    [Route("api/[controller]")]
    public class EmployeeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        public EmployeeController(IConfiguration configuration,IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }
        [Route("GetEmployees")]
        [HttpGet]
        public JsonResult GetEmployees()
        {
            List<Employee> employees = new List<Employee>();
            try
            {
                string query = "select EmployeeId,EmployeeName,Department,convert(varchar(11),DateOfJoining) DateOfJoining,PhotoFileName  from Employee";
                string connectionString = _configuration.GetConnectionString("EmployeeConn");
                SqlDataAdapter da = new SqlDataAdapter(query, connectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        var item = new Employee();
                        int id = 0;
                        int.TryParse(dr["EmployeeId"].ToString(), out id);
                        item.EmployeeId = id;
                        item.EmployeeName = dr["EmployeeName"].ToString();
                        item.DateOfJoining = dr["DateOfJoining"].ToString();
                        item.Department = dr["Department"].ToString();
                        item.PhotoFileName = dr["PhotoFileName"].ToString();

                        employees.Add(item);
                    }
                }
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return new JsonResult(employees);
        }
        [Route("SaveEmployee")]
        [HttpPost]
        public JsonResult SaveEmployee(Employee employee)
        {
            string result = "NOK";
            try
            {
                string connectionString = _configuration.GetConnectionString("EmployeeConn");
                string insert = "Insert into Employee values((select count(*)+1 from Employee),@EmployeeName,@Department,@DateOfJoining,@PhotoFileName)";
                SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = insert;
                cmd.Parameters.AddWithValue("@EmployeeName", employee.EmployeeName);
                cmd.Parameters.AddWithValue("@Department", employee.Department);
                cmd.Parameters.AddWithValue("@DateOfJoining", employee.DateOfJoining);
                cmd.Parameters.AddWithValue("@PhotoFileName", employee.PhotoFileName != null ? employee.PhotoFileName:"" );
                cmd.ExecuteNonQuery();
                conn.Close();
                result = "OK";
            }
            catch(Exception ex)
            {
                result += ". " + ex.Message;
            }
            return new JsonResult(result);
        }        
        [Route("UpdateEmployee")]
        [HttpPut]
        public JsonResult UpdateEmployee(Employee employee)
        {
                string result = "NOK";
            try
            {
                string connectionString = _configuration.GetConnectionString("EmployeeConn");
                string insert = "Update Employee set EmployeeName=@EmployeeName,Department=@Department,DateOfJoining = @DateOfJoining,PhotoFileName=@PhotoFileName  where EmployeeId=@EmployeeId";
                SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = insert;
                cmd.Parameters.AddWithValue("@EmployeeName", employee.EmployeeName);
                cmd.Parameters.AddWithValue("@Department", employee.Department);
                cmd.Parameters.AddWithValue("@DateOfJoining", employee.DateOfJoining);
                cmd.Parameters.AddWithValue("@PhotoFileName", employee.PhotoFileName);
                cmd.Parameters.AddWithValue("EmployeeId", employee.EmployeeId);
                cmd.ExecuteNonQuery();
                conn.Close();
                result = "OK";
            }
            catch (Exception ex)
            {
                result += ". " + ex.Message;
            }
            return new JsonResult(result);
        }
        [Route("DeleteEmployee")]
        [HttpDelete]
        public JsonResult DeleteEmployee(int employeeId)
        {
            string result = "NOK";
            try
            {
                string delete = "Delete from Employee where EmployeeId = @EmployeeId";
                string connectionString = _configuration.GetConnectionString("EmployeeConn");
                SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = delete;
                cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                cmd.ExecuteNonQuery();
                conn.Close();
                result = "OK";
            }
            catch(Exception ex)
            {
                result += ". " + ex.Message;
            }


            return new JsonResult(result);
        }
        [Route("GetEmployeesMongo")]
        [HttpGet]
        public JsonResult GetEmployeesMongo()
        {     
            try
            {
                var settings = MongoClientSettings.FromConnectionString("mongodb+srv://alper:Metallica1@mytestdb.qx6yz.mongodb.net/mytestdb?retryWrites=true&w=majority");
                var client = new MongoClient(settings);
                var database = client.GetDatabase("mytestdb");
                var dbList = database.GetCollection<EmployeeMongo>("Employee").AsQueryable();
                Console.WriteLine(dbList.Count());
                return new JsonResult(dbList);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new JsonResult(ex.Message);
            }            
        }
        [Route("SaveEmployeeMongo")]
        [HttpPost]
        public JsonResult SaveEmployeeMongo(EmployeeMongo employee)
        {
            string result = "NOK";
            try
            {
                if (!string.IsNullOrEmpty(employee.EmployeeName))
                {
                    var settings = MongoClientSettings.FromConnectionString("mongodb+srv://alper:Metallica1@mytestdb.qx6yz.mongodb.net/mytestdb?retryWrites=true&w=majority");
                    var client = new MongoClient(settings);
                    var database = client.GetDatabase("mytestdb");
                    int lastDepartmentId = database.GetCollection<DepartmentMongo>("Employee").AsQueryable().Count();
                    employee.EmployeeId = lastDepartmentId + 1;
                    database.GetCollection<EmployeeMongo>("Employee").InsertOne(employee);
                    result = "OK";
                }
                else
                {
                    result += ". Department Name is  required";
                }
            }
            catch (Exception ex)
            {
                result += ". " + ex.Message;
            }
            return new JsonResult(result);
        }        
        [Route("UpdateEmployeeMongo")]
        [HttpPut]
        public JsonResult UpdateEmployeeMongo(EmployeeMongo employee)
        {
            string result = "NOK";
            try
            {
                var filters = Builders<EmployeeMongo>.Filter.Eq("DepartmentId", employee.EmployeeId);
                var settings = MongoClientSettings.FromConnectionString("mongodb+srv://alper:Metallica1@mytestdb.qx6yz.mongodb.net/mytestdb?retryWrites=true&w=majority");
                var client = new MongoClient(settings);
                var database = client.GetDatabase("mytestdb");
                var update = Builders<EmployeeMongo>.Update.Set("EmployeeName", employee.EmployeeName)
                                                           .Set("Department", employee.Department)
                                                           .Set("DateofJoining", employee.DateofJoining)
                                                           .Set("PhotoFileName", employee.PhotoFileName);
                database.GetCollection<EmployeeMongo>("Department").UpdateOne(filters, update);
                result = "OK";
            }
            catch (Exception ex)
            {
                result += ". " + ex.Message;
            }
            return new JsonResult(result);
        }
        [Route("DeleteEmployeeMongo")]
        [HttpDelete]
        public JsonResult DeleteEmployeeMongo(int employeeId)
        {
            string result = "NOK";
            try
            {
                var settings = MongoClientSettings.FromConnectionString("mongodb+srv://alper:Metallica1@mytestdb.qx6yz.mongodb.net/mytestdb?retryWrites=true&w=majority");
                var client = new MongoClient(settings);
                var database = client.GetDatabase("mytestdb");
                var filters = Builders<DepartmentMongo>.Filter.Eq("EmployeeId", employeeId);
                database.GetCollection<DepartmentMongo>("Department").DeleteOne(filters);
                result = "OK";
            }
            catch (Exception ex)
            {
                result += ". " + ex.Message;
            }


            return new JsonResult(result);
        }
        [Route("SaveFile")]
        [HttpPost]
        public JsonResult SaveFile()
        {
            try
            {
                var httpRequest = Request.Form;
                var postedFile = httpRequest.Files[0];
                string fileName = postedFile.FileName;
                var physicalPath = _env.ContentRootPath + "/Photos/" + fileName;

                using(var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }
                return new JsonResult(fileName);
            }
            catch (Exception)
            {
                return new JsonResult("anonymous.png");
            }
        }
        [Route("SaveFile2")]
        [HttpPost]
        public JsonResult SaveFile2(IFormFile employeeFile)
        {
            try
            {
                var dd = employeeFile.FileName;
                var httpRequest = Request.Form;
                var postedFile = httpRequest.Files[0];
                string fileName = employeeFile.FileName;//postedFile.FileName;
                var physicalPath = _env.ContentRootPath + "/Photos/" + fileName;

                using (var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }
                return new JsonResult(fileName);
            }
            catch (Exception)
            {
                return new JsonResult("anonymous.png");
            }
        }
    }
}
