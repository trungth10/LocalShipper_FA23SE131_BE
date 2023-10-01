using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LocalShipper.Service.Helpers
{
    public enum StatusOfBatch
    {
        [Display(Name = "TU DONG")]
        IDLE = 1,

        [Display(Name = "Đang giao hàng")]
        Delivering = 2,

        [Display(Name = "Hoan thanh")]
        COMPLETE = 3,

        [Display(Name = "xoa")]
        DELETE = 4,
    }
}
