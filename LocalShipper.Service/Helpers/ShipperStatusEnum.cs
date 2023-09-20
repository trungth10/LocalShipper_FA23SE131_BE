using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LocalShipper.Service.Helpers
{
    public enum ShipperStatusEnum
    {
        [Display(Name = "Ngoại tuyến")]
        Offline =  0,
        [Display(Name = "Trực tuyến")]
        Online = 1,
        [Display(Name = "Đang giao hàng")]
        Delivering = 2,
    }
}
