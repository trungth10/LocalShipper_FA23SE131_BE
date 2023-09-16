using System;
using System.Collections.Generic;

namespace LocalShipper.Data.Models
{
    public partial class Shipper
    {
        public Shipper()
        {
            Orders = new HashSet<Order>();
            Ratings = new HashSet<Rating>();
            Wallets = new HashSet<Wallet>();
        }

        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? EmailShipper { get; set; }
        public string? PhoneShipper { get; set; }
        public string AdressShipper { get; set; } = null!;
        public int TransportId { get; set; }
        public int AccountId { get; set; }
        public int ZoneId { get; set; }
        public int? Status { get; set; }
        public string Fcmtoken { get; set; } = null!;

        public virtual Account Account { get; set; } = null!;
        public virtual Transport Transport { get; set; } = null!;
        public virtual Zone Zone { get; set; } = null!;
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Rating> Ratings { get; set; }
        public virtual ICollection<Wallet> Wallets { get; set; }
    }
}
