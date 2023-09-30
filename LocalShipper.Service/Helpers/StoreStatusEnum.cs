using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LocalShipper.Service.Helpers
{
    public enum StoreStatusEnum
    {
        [Display(Name = "chua xoa")]
        EXIST = 1,

        [Display(Name = "xoa")]
        DELETE = 2,
    }
}
