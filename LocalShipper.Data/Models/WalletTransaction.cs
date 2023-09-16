using System;
using System.Collections.Generic;

namespace LocalShipper.Data.Models
{
    public partial class WalletTransaction
    {
        public int Id { get; set; }
        public string TransactionType { get; set; } = null!;
        public int FromWalletId { get; set; }
        public int ToWalletId { get; set; }
        public decimal Amount { get; set; }
        public DateTime? TransactionTime { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; }

        public virtual Wallet FromWallet { get; set; } = null!;
        public virtual Wallet ToWallet { get; set; } = null!;
    }
}
