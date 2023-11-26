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
    public class WalletTransactionPayment
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }
}
