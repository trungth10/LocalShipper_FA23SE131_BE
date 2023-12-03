using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;

namespace MailLocal
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [FunctionName("Function1")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            IActionResult response = null;

            var queryParams = req.Query;
            string email = queryParams["email"];

            if (string.IsNullOrEmpty(email))
            {
                response = new BadRequestObjectResult("Please provide an email parameter in the query string.");
            }
            else
            {
                try
                {
                    var validationResponse = await CheckEmailValidity(email);
                    response = new OkObjectResult($"Email Validation Result: {validationResponse.Status}");
                }
                catch (Exception ex)
                {
                    response = new BadRequestObjectResult($"Error: {ex.Message}");
                }
            }

            return response;
        }

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

        public async Task<EmailValidationResponse> CheckEmailValidity(string email)
        {
            string apiKey = "2ca20a55119159634180276afbb156fd720e67d5";

            using (HttpClient client = new HttpClient())
            {
                string apiUrl = $"https://api.hunter.io/v2/email-verifier?email={email}&api_key={apiKey}";

                try
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    response.EnsureSuccessStatusCode(); // Ensure the response is successful.

                    string responseBody = await response.Content.ReadAsStringAsync();
                    var validationResponse = JsonConvert.DeserializeObject<ValidationResponse>(responseBody);

                    if (validationResponse.Data.Status.ToLower() == "valid")
                    {
                        return new EmailValidationResponse { IsValid = true, Status = "Valid" };
                    }
                    else if (validationResponse.Data.Status.ToLower() == "invalid")
                    {
                        return new EmailValidationResponse { IsValid = false, Status = validationResponse.Data.Result ?? "Invalid" };
                    }
                    else
                    {
                        throw new Exception("Không thể xác định trạng thái email.");
                    }
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception("Lỗi khi gửi yêu cầu kiểm tra email.", ex);
                }
            }
        }
    }
}