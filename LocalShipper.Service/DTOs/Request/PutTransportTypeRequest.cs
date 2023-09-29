using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Request
{
    public class PutTransportTypeRequest
    {
        public string TransportType { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
