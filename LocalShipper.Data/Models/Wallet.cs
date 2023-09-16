using System;
using System.Collections.Generic;

namespace LocalShipper.Data.Models
{
    public partial class Wallet
    {
        public Wallet()
        {
            Transactions = new HashSet<Transaction>();
            WalletTransactionFromWallets = new HashSet<WalletTransaction>();
            WalletTransactionToWallets = new HashSet<WalletTransaction>();
        }

        public int Id { get; set; }
        public int OwnerId { get; set; }
        public string OwnerType { get; set; } = null!;
        public decimal Balance { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual Shipper Owner { get; set; } = null!;
        public virtual Store OwnerNavigation { get; set; } = null!;
        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual ICollection<WalletTransaction> WalletTransactionFromWallets { get; set; }
        public virtual ICollection<WalletTransaction> WalletTransactionToWallets { get; set; }
    }
}
