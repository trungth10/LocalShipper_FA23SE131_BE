using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Request
{
    public class PostBrandRequest
    {
        public string BrandName { get; set; }
        public string BrandDescription { get; set; }
        public DateTime CreatedAt { get; set; }
        public string IconUrl { get; set; }
        public string ImageUrl { get; set; }
        public bool Active { get; set; }
        public int AccountId { get; set; }
    }
}
