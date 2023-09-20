using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Helpers
{
    public static class ShipperStatusEnum
    {
        public enum Status
        {
            All = 0,
            Block = 1,
            OffDuty = 2,
            Deleted = 3,
            Available = 4,
            Busy = 5,
            InActive = 6
        }
    }
}
