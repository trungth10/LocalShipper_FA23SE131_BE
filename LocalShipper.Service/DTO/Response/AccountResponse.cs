using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTO.Response
{
    public class AccountResponse
    {
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
        public string BrandName { get; set; }
    }
}
