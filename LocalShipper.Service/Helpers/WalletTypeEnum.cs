using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Helpers
{
    public enum WalletTypeEnum
    {
        [Display(Name = "Ví Chính")]
        VICHINH = 1,
        [Display(Name = "Ví thu hộ")]
        VITHUHO = 2,
        [Display(Name = "Ví kích hoạt")]
        VIKICHHOAT = 3,
    }
}
