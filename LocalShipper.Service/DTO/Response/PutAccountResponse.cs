﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTO.Response
{
    public class PutAccountResponse
    {
        public int Id { get; set; }
        public string FireBaseUid { get; set; }
        public string Fullname { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public int? Role { get; set; }
        public bool? Active { get; set; }
        public string FcmToken { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? BrandId { get; set; }
        public string ImageUrl { get; set; }
        public string BrandName { get; set; }
        public bool? IsApproved { get; set; }
        public string Jwt { get; set; }
    }
}