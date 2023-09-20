using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LocalShipper.Service.Helpers
{
    public enum OrderStatusEnum
    {
        [Display(Name = "Chọn Shipper")]
        IDLE = 0,
        [Display(Name = "Public Order")]
        ASSIGNING = 1,
        ACCEPTED = 2,
        CANCELLED = 3,
        INPROCESS = 4,
        FAILED = 5,
        COMPLETED = 6,
        RETURNED = 7,
    }
}
