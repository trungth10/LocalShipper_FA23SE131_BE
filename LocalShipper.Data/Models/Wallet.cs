using System;
using System.Collections.Generic;

#nullable disable

namespace LocalShipper.Data.Models
{
    public partial class Wallet
    {
        public Wallet()
        {          
            Stores = new HashSet<Store>();
            WalletTransactionFromWallets = new HashSet<WalletTransaction>();
            WalletTransactionToWallets = new HashSet<WalletTransaction>();
        }

        public int Id { get; set; }
        public decimal Balance { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? Type { get; set; }
        public int? ShipperId { get; set; }

        public virtual Shipper Shipper { get; set; }
        
        public virtual ICollection<Store> Stores { get; set; }
        public virtual ICollection<WalletTransaction> WalletTransactionFromWallets { get; set; }
        public virtual ICollection<WalletTransaction> WalletTransactionToWallets { get; set; }
    }
}
