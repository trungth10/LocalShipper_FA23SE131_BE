using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Response
{
    public class TemplateResponse
    {
        public int Id { get; set; }
        public string TemplateName { get; set; }
        public string ImageUrl { get; set; }
        public DateTime? CreateAt { get; set; }
    }
}
