using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using WebapiforAngular.Models;
using MongoDB.Driver;
using Npgsql;
// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebapiforAngular.Controllers
{
    [Route("api/[controller]")]
    public class DepartmentController : Controller
    {
        // GET: /<controller>/
        private readonly IConfiguration _configuration;
        public DepartmentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [Route("GetDepartments")]
        [HttpGet]
        public JsonResult GetDepartments()
        {
            List<Department> deparments = new List<Department>();
            try
            {
                string query = @"select * from Department";
                string connection = _configuration.GetConnectionString("EmployeeConn");
                SqlDataAdapter da = new SqlDataAdapter(query, connection);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        var item = new Department();
                        int id = 0;
                        int.TryParse(dr["DepartmentId"].ToString(), out id);
                        item.Id = id;
                        item.DepartmentName = dr["DepartName"].ToString();

                        deparments.Add(item);
                    }
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return new JsonResult(deparments);
        }
        
        [Route("SaveDepartment")]
        [HttpPost]
        public JsonResult SaveDepartment(string department)
        {
            string result = "NOK";
            try
            {
                if (!string.IsNullOrEmpty(department))
                {
                    string connectionString = _configuration.GetConnectionString("MongoDB");
                    string q = "Insert into Department values((select  count(*)+1 from Department),@DepartmentName)";
                    SqlConnection conn = new SqlConnection(connectionString);
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = q;
                    cmd.Parameters.AddWithValue("@DepartmentName",  department);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    result = "OK";
                }else
                {
                    result += ". Department Name is  required";
                }
            }
            catch(Exception ex)
            {
                result += ". " + ex.Message; 
            }

            return new JsonResult(result);
        }
       

        [Route("UpdateDepartment")]
        [HttpPut]
        public JsonResult UpdateDepartment(Department department)
        {
            string result = "NOK";
            try
            {
                string connectionString = _configuration.GetConnectionString("EmployeeConn");
                string q = @"update Department set DepartName = @DepartName where DepartmentId = @Id";
                SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = q;
                cmd.Parameters.AddWithValue("@Id", department.Id);
                cmd.Parameters.AddWithValue("@DepartName", department.DepartmentName);
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
      
        [Route("DeleteDepartment")]
        [HttpDelete]
        public JsonResult DeleteDepartment(int departmentId)
        {
            string result = "NOK";
            try
            {
                string deleteQuery = "Delete  from Department where DepartmentId = @Id";
                string connectionString = _configuration.GetConnectionString("EmployeeConn");
                SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = deleteQuery;
                cmd.Parameters.AddWithValue("@Id", departmentId);
                cmd.ExecuteNonQuery();
                conn.Close();
                result = "OK";
            }catch(Exception ex)
            {
                result += ". " + ex.Message;
            }


            return new JsonResult(result);
        }
        [Route("GetDepartmentsMongo")]
        [HttpGet]
        public JsonResult GetDepartmentsMongo()
        {
            
            try
            {
                var settings = MongoClientSettings.FromConnectionString("mongodb+srv://alper:Metallica1@mytestdb.qx6yz.mongodb.net/mytestdb?retryWrites=true&w=majority");
                var client = new MongoClient(settings);
                var database = client.GetDatabase("mytestdb");
                var dbList = database.GetCollection<DepartmentMongo>("Department").AsQueryable();
                Console.WriteLine(dbList.Count());
                return new JsonResult(dbList);
                // string connection = _configuration.GetConnectionString("MongoDB");
                //  MongoClient dbClient = new MongoClient(connection);
                // var dbList = dbClient.GetDatabase("mytestdb").GetCollection<Department>("Department").AsQueryable();
                //return new JsonResult(dbList);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new JsonResult(ex.Message);
            }


        }
        [Route("SaveDepartmentMongo")]
        [HttpPost]
        public JsonResult SaveDepartmentMongo(DepartmentMongo department)
        {
            string result = "NOK";
            try
            {
                if (!string.IsNullOrEmpty(department.DepartmentName))
                {
                    var settings = MongoClientSettings.FromConnectionString("mongodb+srv://alper:Metallica1@mytestdb.qx6yz.mongodb.net/mytestdb?retryWrites=true&w=majority");
                    var client = new MongoClient(settings);
                    var database = client.GetDatabase("mytestdb");
                    int lastDepartmentId = database.GetCollection<DepartmentMongo>("Department").AsQueryable().Count();
                    department.DepartmentId = lastDepartmentId + 1;
                    database.GetCollection<DepartmentMongo>("Department").InsertOne(department);
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
        [Route("UpdateDepartmentMongo")]
        [HttpPut]
        public JsonResult UpdateDepartmentMongo(DepartmentMongo department)
        {
            string result = "NOK";
            try
            {
                var filters = Builders<DepartmentMongo>.Filter.Eq("DepartmentId", department.DepartmentId);
                var settings = MongoClientSettings.FromConnectionString("mongodb+srv://alper:Metallica1@mytestdb.qx6yz.mongodb.net/mytestdb?retryWrites=true&w=majority");
                var client = new MongoClient(settings);
                var database = client.GetDatabase("mytestdb");
                var update = Builders<DepartmentMongo>.Update.Set("DepartmentName", department.DepartmentName);
                database.GetCollection<DepartmentMongo>("Department").UpdateOne(filters, update);
                result = "OK";
            }
            catch (Exception ex)
            {
                result += ". " + ex.Message;
            }


            return new JsonResult(result);
        }
        [Route("DeleteDepartmentMongo")]
        [HttpDelete]
        public JsonResult DeleteDepartmentMongo(int departmentId)
        {
            string result = "NOK";
            try
            {
                
                var settings = MongoClientSettings.FromConnectionString("mongodb+srv://alper:Metallica1@mytestdb.qx6yz.mongodb.net/mytestdb?retryWrites=true&w=majority");
                var client = new MongoClient(settings);
                var database = client.GetDatabase("mytestdb");
                var filters = Builders<DepartmentMongo>.Filter.Eq("DepartmentId", departmentId);                
                database.GetCollection<DepartmentMongo>("Department").DeleteOne(filters);
                result = "OK";
            }
            catch (Exception ex)
            {
                result += ". " + ex.Message;
            }


            return new JsonResult(result);
        }

        [Route("GetDepartmentsPos")]
        [HttpGet]
        public JsonResult GetDepartmentsPos()
        {
            List<Department> deparments = new List<Department>();
            try
            {
                DataTable dt = new DataTable(); ;
                string query = @"select * from Department";
                string connection = _configuration.GetConnectionString("PostgreSql");
                using (NpgsqlConnection conn = new NpgsqlConnection(connection))
                {
                    conn.Open();
                    NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                    dt.Load(cmd.ExecuteReader());
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            var item = new Department();
                            int id = 0;
                            int.TryParse(dr["DepartmentId"].ToString(), out id);
                            item.Id = id;
                            item.DepartmentName = dr["DepartmentName"].ToString();
                            deparments.Add(item);
                        }
                    }
                    conn.Close();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return new JsonResult(deparments);
        }

        [Route("SaveDepartmentPos")]
        [HttpPost]
        public JsonResult SaveDepartmentPos(string department)
        {
            string result = "NOK";
            try
            {
                if (!string.IsNullOrEmpty(department))
                {
                    string connectionString = _configuration.GetConnectionString("PostgreSql");
                    string q = "Insert into Department(DepartmentName) values(@DepartmentName)";
                    using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
                    {
                        conn.Open();
                        NpgsqlCommand cmd = new NpgsqlCommand(q, conn);
                        cmd.Parameters.AddWithValue("@DepartmentName", department);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                        result = "OK";
                    }

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


        [Route("UpdateDepartmentPos")]
        [HttpPut]
        public JsonResult UpdateDepartmentPos(Department department)
        {
            string result = "NOK";
            try
            {
                string connectionString = _configuration.GetConnectionString("PostgreSql");
                string q = @"update Department set departmentname = @DepartName where departmentid = @Id";
                using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    NpgsqlCommand cmd = new NpgsqlCommand(q, conn);
                    cmd.Parameters.AddWithValue("@DepartName", department.DepartmentName);
                    cmd.Parameters.AddWithValue("@Id", department.Id);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    result = "OK";
                } 
            }
            catch (Exception ex)
            {
                result += ". " + ex.Message;
            }


            return new JsonResult(result);
        }

        [Route("DeleteDepartmentPos")]
        [HttpDelete]
        public JsonResult DeleteDepartmentPos(int departmentId)
        {
            string result = "NOK";
            try
            {
                string deleteQuery = "Delete  from Department where DepartmentId = @Id";
                string connectionString = _configuration.GetConnectionString("PostgreSql");
                using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    NpgsqlCommand cmd = new NpgsqlCommand(deleteQuery, conn);                    
                    cmd.Parameters.AddWithValue("@Id",departmentId);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    result = "OK";
                }
            }
            catch (Exception ex)
            {
                result += ". " + ex.Message;
            }


            return new JsonResult(result);
        }


        [Route("PostgreTest")]
        [HttpGet]
        public string GetConnection()
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetConnectionString("PostgreSql")))
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        return "Connected";
                    }
                    else
                    {
                        return "Not connected";
                    }
                }
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
