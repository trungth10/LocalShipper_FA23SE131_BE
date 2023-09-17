﻿using LocalShipper.Data.Models;
using System.Collections.Generic;
using System;

namespace LSAPI.Models
{
    public partial class Account
    {
        public Account()
        {
            Brands = new HashSet<Brand>();
            Shippers = new HashSet<Shipper>();
            Stores = new HashSet<Store>();
        }

        public int Id { get; set; }
        public string Fullname { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
        public bool? Active { get; set; }
        public string FcmToken { get; set; }
        public DateTime CreateDate { get; set; }
        public int? BrandId { get; set; }
        public string ImageUrl { get; set; }
        public string Password { get; set; }

        public virtual Role Role { get; set; }
        public virtual ICollection<Brand> Brands { get; set; }
        public virtual ICollection<Shipper> Shippers { get; set; }
        public virtual ICollection<Store> Stores { get; set; }
    }
}
