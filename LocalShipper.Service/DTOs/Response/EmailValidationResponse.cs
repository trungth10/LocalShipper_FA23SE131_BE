using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Response
{
    public class EmailValidationResponse
    {
        public bool IsValid { get; set; }
        public string Status { get; set; }
    }
    public class ValidationResponse
    {
        [JsonProperty("data")]
        public EmailValidationData Data { get; set; }

        public class EmailValidationData
        {
            public string Status { get; set; }
            public string Result { get; set; }
            public int Score { get; set; }
            public string Email { get; set; }
        }
    }
}
