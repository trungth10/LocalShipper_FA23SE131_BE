using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LocalShipper.Service.Helpers
{
    public enum ShipperTypeEnum
    {
        [Display(Name = "Private")]
        PRIVATE =  1,
        [Display(Name = "Public")]
        PUBLIC = 2

    }
}
