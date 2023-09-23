using System;
using System.Collections.Generic;

#nullable disable

namespace LocalShipper.Data.Models
{
    public partial class Shipper
    {
        public Shipper()
        {
            Orders = new HashSet<Order>();
            Ratings = new HashSet<Rating>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailShipper { get; set; }
        public string PhoneShipper { get; set; }
        public string AddressShipper { get; set; }
        public int TransportId { get; set; }
        public int AccountId { get; set; }
        public int ZoneId { get; set; }
        public int? Status { get; set; }
        public string Fcmtoken { get; set; }
        public int WalletId { get; set; }

        public virtual Account Account { get; set; }
        public virtual Transport Transport { get; set; }
        public virtual Wallet Wallet { get; set; }
        public virtual Zone Zone { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Rating> Ratings { get; set; }
    }
}
