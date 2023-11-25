using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Request
{
    public class RegisterShipperRequest
    {
        public RegisterRequest RegisterRequest { get; set; }
        public IFormFile Image { get; set; }
    }
}
