﻿using System;
using System.Collections.Generic;

#nullable disable

namespace LocalShipper.Data.Models
{
    public partial class Account
    {
        public Account()
        {
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
        public string ImageUrl { get; set; }
        public string Password { get; set; }
        public virtual Role Role { get; set; }
        public virtual ICollection<Shipper> Shippers { get; set; }
        public virtual ICollection<Store> Stores { get; set; }
    }
}
