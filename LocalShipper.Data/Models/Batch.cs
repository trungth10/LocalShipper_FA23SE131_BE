﻿using System;
using System.Collections.Generic;

namespace LocalShipper.Data.Models
{
    public partial class Batch
    {
        public Batch()
        {
            Orders = new HashSet<Order>();
            Packages = new HashSet<Package>();
        }

        public int Id { get; set; }
        public int StoreId { get; set; }
        public string BatchName { get; set; } = null!;
        public string? BatchDescription { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }

        public virtual Store Store { get; set; } = null!;
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Package> Packages { get; set; }
    }
}
