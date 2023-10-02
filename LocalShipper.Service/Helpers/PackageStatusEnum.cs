using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LocalShipper.Service.Helpers
{
    public enum PackageStatusEnum
    {
        IDLE = 1,
        ACCEPTED = 2,
        CANCELLED = 3,
    }
}
