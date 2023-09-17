using System;
using System.Collections.Generic;

#nullable disable

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
        public string OwnerType { get; set; }
        public decimal Balance { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual Shipper Owner { get; set; }
        public virtual Store OwnerNavigation { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual ICollection<WalletTransaction> WalletTransactionFromWallets { get; set; }
        public virtual ICollection<WalletTransaction> WalletTransactionToWallets { get; set; }
    }
}
