using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTO.Request
{
    public class PostAccountRequest
    {
        public string Fullname { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public int Role { get; set; }
        public string ImageUrl { get; set; }
        public int BrandId { get; set; } = -1;
        public string Password { get; set; }
    }
}
