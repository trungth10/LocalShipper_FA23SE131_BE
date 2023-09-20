﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Response
{
    public class ShipperResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailShipper { get; set; }
        public string PhoneShipper { get; set; }
        public string AdressShipper { get; set; }
        public int TransportId { get; set; }
        public int AccountId { get; set; }
        public int ZoneId { get; set; }
        public int? Status { get; set; }
        public string Fcmtoken { get; set; }
    }
    public class ShipperResponseJwt : ShipperResponse
    {
        public string? Jwt { get; set; }
    }
}
