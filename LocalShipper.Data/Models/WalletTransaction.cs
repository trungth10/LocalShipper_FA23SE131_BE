using System;
using System.Collections.Generic;

#nullable disable

namespace LocalShipper.Data.Models
{
    public partial class WalletTransaction
    {
        public int Id { get; set; }
        public string TransactionType { get; set; }
        public int? FromWalletId { get; set; }
        public int? ToWalletId { get; set; }
        public decimal Amount { get; set; }
        public DateTime? TransactionTime { get; set; }
        public string Description { get; set; }
        public int? OrderId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? Active { get; set; }


        public virtual Wallet FromWallet { get; set; }
        public virtual Order Order { get; set; }
        public virtual Wallet ToWallet { get; set; }
    }
}
