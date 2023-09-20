using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Request
{
    public class CreateBatchRequest
    {
        public List<int> Orders { get; set; }
        public List<DriverRequest> Drivers { get; set; }
        public int BrandId { get; set; }
        public Guid? CreatorId { get; set; }
    }

    public class DriverRequest
    {
        public int Id { get; set; }
        public int Capacity { get; set; }
    }
}