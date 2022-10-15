using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebapiForAngular2.Models;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebapiForAngular2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DepartmentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public DepartmentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        // GET: /<controller>/
        public JsonResult Index()
        {
            List<Department> departments = new List<Department>();
            try
            {
                string connectionString = _configuration.GetConnectionString("EmployeeConn");
                string q = "select  * from Department";
                SqlDataAdapter da = new SqlDataAdapter(q, connectionString);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        var item = new Department();
                        int id = 0;
                        int.TryParse(dr["Id"].ToString(), out id);

                        departments.Add(item);
                    }
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException);
            }
            return new JsonResult(departments);
        }
    }
}
