using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTO.Request
{
    public class SignInUsermamePass
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FCMToken { get; set; }
    }
}