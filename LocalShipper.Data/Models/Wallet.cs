using System;
using System.Collections.Generic;

#nullable disable

namespace LocalShipper.Data.Models
{
    public partial class Wallet
    {
        public Wallet()
        {
            Shippers = new HashSet<Shipper>();
            Stores = new HashSet<Store>();
            WalletTransactionFromWallets = new HashSet<WalletTransaction>();
            WalletTransactionToWallets = new HashSet<WalletTransaction>();
        }

        public int Id { get; set; }
        public decimal Balance { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Shipper> Shippers { get; set; }
        public virtual ICollection<Store> Stores { get; set; }
        public virtual ICollection<WalletTransaction> WalletTransactionFromWallets { get; set; }
        public virtual ICollection<WalletTransaction> WalletTransactionToWallets { get; set; }
    }
}
