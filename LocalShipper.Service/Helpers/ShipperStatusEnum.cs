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
        [Display(Name = "Vô hiệu hóa")]
        Deactive = 1,
        [Display(Name = "Ngoại tuyến")]
        Offline =  2,
        [Display(Name = "Trực tuyến")]
        Online = 3,
        [Display(Name = "Đang giao hàng")]
        Delivering = 4,
    }
}
