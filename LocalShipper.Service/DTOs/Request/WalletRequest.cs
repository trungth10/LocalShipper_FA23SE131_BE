using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Request
{
    public class WalletRequest
    {
        public decimal Balance { get; set; }
        public int Type { get; set; }
        public int ShipperId { get; set; }
    }
}
