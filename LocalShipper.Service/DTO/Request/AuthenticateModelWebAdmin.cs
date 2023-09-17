using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTO.Request
{
    public class AuthenticateModelWebAdmin
    {
        [Required]
        public string IdToken { get; set; }

        public string FcmToken { get; set; }
    }
}
