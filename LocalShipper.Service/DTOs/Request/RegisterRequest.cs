using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Request
{
    public class RegisterRequest
    {
        [Required, MinLength(6, ErrorMessage = "Xin nhập lớn hơn 6 ký tự")]
        public string FullName { get; set; } = string.Empty;
        [Required] 
        public string Email { get; set; } = string.Empty;
        [Required, Phone]
        public string Phone { get; set; } = string.Empty;
        [Required, MinLength(6, ErrorMessage = "Xin nhập lớn hơn 6 ký tự")]
        public string Password { get; set; } = string.Empty;
        [Required, Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;


    }
}
