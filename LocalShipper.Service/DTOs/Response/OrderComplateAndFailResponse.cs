using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Response
{
    public class OrderComplateAndFailResponse
    {
       public int ComplateOrder { get; set; }
        public int FailOrder { get; set; }
    }
}
