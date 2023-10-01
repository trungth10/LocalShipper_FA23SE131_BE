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
        IDLE = 1,
        [Display(Name = "Public Order")]
        ASSIGNING = 2,
        ACCEPTED = 3,
        CANCELLED = 4,
        INPROCESS = 5,       
        COMPLETED = 6,
        DELETED = 7,
    }
}
