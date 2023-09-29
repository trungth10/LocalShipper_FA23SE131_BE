using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Request
{
    public class RegisterTransactionRequest
    {
        public string TransactionMethod { get; set; }
        public int OrderId { get; set; }
        public int WalletId { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionTime { get; set; }
        public string TransactionDescription { get; set; }
        public DateTime? CreatedAt { get; set; }

    }
}
