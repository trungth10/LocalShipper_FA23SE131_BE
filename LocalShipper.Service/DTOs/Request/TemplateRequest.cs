using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Request
{
    public class TemplateRequest
    {
        public string TemplateName { get; set; }
        public string ImageUrl { get; set; }
        public DateTime? CreateAt { get; set; }
        public bool? Deleted { get; set; }
    }
}
