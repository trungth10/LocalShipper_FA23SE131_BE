using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Helpers
{
    public enum RouteEdgeStatusEnum
    {
        IDLE = 1,
        INPROCESS = 2,
        COMPLETE =3,
        DELETED = 4,
    }
}
