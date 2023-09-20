using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Helpers
{
    public enum BatchStatusEnum
    {
        New = 0,
        Failed = 1,
        Closed = 3,
        NotEnoughDriver = 4
    }
}
