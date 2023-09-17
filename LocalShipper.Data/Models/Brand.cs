﻿using System;
using System.Collections.Generic;

#nullable disable

namespace LocalShipper.Data.Models
{
    public partial class Brand
    {
        public Brand()
        {
            Stores = new HashSet<Store>();
        }

        public int Id { get; set; }
        public string BrandName { get; set; }
        public string BrandDescription { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string IconUrl { get; set; }
        public string ImageUrl { get; set; }
        public bool? Active { get; set; }
        public int AccountId { get; set; }

        public virtual Account Account { get; set; }
        public virtual ICollection<Store> Stores { get; set; }
    }
}
