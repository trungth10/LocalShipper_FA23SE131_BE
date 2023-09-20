using LocalShipper.Service.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Request
{
    public class BatchRequest : PagingRequest
    {
        public BatchStatusEnum? Status { get; set; }
    }
}
