using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Request
{
    public class AccountRequest
    {
        
        public string? FullName { get; set; } = null;

        public string? Email { get; set; } = null;
        public string? Phone { get; set; } = null;
        public string? Password { get; set; } = null;
        public int? RoleId { get; set; }
        public string? ImageUrl { get; set; } = null;
        public string? Fcm_token { get; set; } = null;
        public bool? Active { get; set; } = null;
    }
}
