using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Response
{
    public class PackageTypeResponse
    {
        public int Id { get; set; }
        public string PackageType1 { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
