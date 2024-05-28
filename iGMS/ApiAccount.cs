using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WMS.Models;

namespace WMS
{
    public class ApiAccount
    {
        public string UserName {  get; set; }
        public string FullName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public List<ApiRole> ApiRoles { get; set; }
    }
}