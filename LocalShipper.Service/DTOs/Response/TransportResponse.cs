using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Response
{
    public class TransportResponse
    {
        public int Id { get; set; }
        public int TypeId { get; set; }
        public string LicencePlate { get; set; }
        public string TransportColor { get; set; }
        public string TransportImage { get; set; }
        public string TransportRegistration { get; set; }
        public bool Active { get; set; }
        public TransportTypeResponse TransportType { get; set; }
    }
}
