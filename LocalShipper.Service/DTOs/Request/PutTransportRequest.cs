using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Request
{
    public class PutTransportRequest
    {
        public int TypeId { get; set; }
        public string LicencePlate { get; set; }
        public string TransportColor { get; set; }
        public string TransportImage { get; set; }
        public string TransportRegistration { get; set; }

    }
}
