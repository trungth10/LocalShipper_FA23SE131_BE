using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Request
{
    public class TransportRequest
    {
        public int Id {  get; set; }
        public string LicencePlate { get; set; }
    }
}
