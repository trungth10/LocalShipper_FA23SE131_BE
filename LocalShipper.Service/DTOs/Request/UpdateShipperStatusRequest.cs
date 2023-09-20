using LocalShipper.Service.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Request
{
    public class UpdateShipperStatusRequest
    {
        public ShipperStatusEnum? status { get; set; }
    }
}
