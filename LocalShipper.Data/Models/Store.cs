﻿using System;
using System.Collections.Generic;

#nullable disable

namespace LocalShipper.Data.Models
{
    public partial class Store
    {
        public Store()
        {
            Batches = new HashSet<Batch>();
            Histories = new HashSet<History>();
            Orders = new HashSet<Order>();
            Ratings = new HashSet<Rating>();
            Wallets = new HashSet<Wallet>();
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
        public int BrandId { get; set; }
        public int? TemplateId { get; set; }
        public int? ZoneId { get; set; }
        public int AccountId { get; set; }

        public virtual Account Account { get; set; }
        public virtual Brand Brand { get; set; }
        public virtual Template Template { get; set; }
        public virtual Zone Zone { get; set; }
        public virtual ICollection<Batch> Batches { get; set; }
        public virtual ICollection<History> Histories { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Rating> Ratings { get; set; }
        public virtual ICollection<Wallet> Wallets { get; set; }
    }
}
