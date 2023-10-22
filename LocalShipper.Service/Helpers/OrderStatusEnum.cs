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
        [Display(Name = "Đợi Shipper Accept")]
        WAITING = 3,
        ACCEPTED = 4,
        CANCELLED = 5,
        INPROCESS = 6,       
        COMPLETED = 7,
        DELETED = 8,
        RETURN = 9,
    }

    public enum RouteStatusEnum
    {
        ACTIVE =1,
        DEACTIVE = 2,
    }

    public enum SuggestEnum
    {
        DISTRICT =1,
        ACTION = 2,
        TYPE = 3,
        CAPACITY =4,
        COD =5,
    }
}
