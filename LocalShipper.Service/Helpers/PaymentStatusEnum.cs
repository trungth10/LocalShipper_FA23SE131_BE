using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Helpers
{
    public enum PaymentStatusEnum
    {
        [Display(Name = "Hoạt động")]
        ACTIVE =  1,
        [Display(Name = "Tạm khóa")]
        BAN = 2,
        [Display(Name = "Đã xóa")]
        DELETED = 3,
    }
}
