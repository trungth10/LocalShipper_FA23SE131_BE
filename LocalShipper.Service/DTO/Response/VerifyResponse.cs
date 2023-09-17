using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTO.Response
{
    public class VerifyResponse
    {
        public string Jwt { get; set; }
        public string RefreshToken { get; set; }
    }
}