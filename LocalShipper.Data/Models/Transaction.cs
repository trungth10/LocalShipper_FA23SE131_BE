using System;
using System.Collections.Generic;

namespace LocalShipper.Data.Models
{
    public partial class Transaction
    {
        public int Id { get; set; }
        public string TransactionMethod { get; set; } = null!;
        public int OrderId { get; set; }
        public int WalletId { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionTime { get; set; }
        public string? TransactionDescription { get; set; }
        public DateTime? CreatedAt { get; set; }

        public virtual Order Order { get; set; } = null!;
        public virtual Wallet Wallet { get; set; } = null!;
    }
}
