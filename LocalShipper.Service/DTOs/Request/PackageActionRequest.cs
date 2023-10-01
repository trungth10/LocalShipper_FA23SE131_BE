using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Request
{
    public class PackageActionRequest
    {
      
        public string ActionType { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool? Deleted { get; set; }
    }
}
