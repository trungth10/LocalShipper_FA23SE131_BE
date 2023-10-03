using System;
using System.Collections.Generic;

#nullable disable

namespace LocalShipper.Data.Models
{
    public partial class Store
    {
        public Store()
        {
            Histories = new HashSet<History>();
            Orders = new HashSet<Order>();
            Packages = new HashSet<Package>();
            Prices = new HashSet<Price>();
            Ratings = new HashSet<Rating>();
        }

        public int Id { get; set; }
        public string StoreName { get; set; }
        public string StoreAddress { get; set; }
        public string StorePhone { get; set; }
        public string StoreEmail { get; set; }
        public TimeSpan? OpenTime { get; set; }
        public TimeSpan? CloseTime { get; set; }
        public string StoreDescription { get; set; }
        public int? Status { get; set; }
        public int? TemplateId { get; set; }
        public int? ZoneId { get; set; }
        public int WalletId { get; set; }
        public int AccountId { get; set; }

        public virtual Account Account { get; set; }
        public virtual Template Template { get; set; }
        public virtual Wallet Wallet { get; set; }
        public virtual Zone Zone { get; set; }
        public virtual ICollection<History> Histories { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Package> Packages { get; set; }
        public virtual ICollection<Price> Prices { get; set; }
        public virtual ICollection<Rating> Ratings { get; set; }
    }
}
