using System;
using MongoDB.Bson;

namespace WebapiforAngular.Models
{
    public class DepartmentMongo
    {
        public ObjectId id { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
    }
}
