using System;
using MongoDB.Bson;

namespace WebapiforAngular.Models
{
    public class EmployeeMongo
    {
        public ObjectId id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string Department { get; set; }
        public string DateofJoining { get; set; }
        public string PhotoFileName { get; set; }
    }
}
