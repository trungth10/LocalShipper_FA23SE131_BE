using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Request
{
    public class TransportRequest
    {
        public int typeId { get; set; }
        public string licence_plate { get; set; }
        public string color { get; set; }
        public string transport_image { get; set; }
        public string transport_registration { get; set; }
    }
}
