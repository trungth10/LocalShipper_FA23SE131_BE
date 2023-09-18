using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Response
{
    public class BaseResponseViewModel<T>
    {
        public StatusViewModel Status { get; set; } = new StatusViewModel();
        public T? Data { get; set; }
    }

    public class StatusViewModel
    {
        public bool IsSuccess { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        public HttpStatusCode Code { get; set; } = HttpStatusCode.OK;
    }
}
