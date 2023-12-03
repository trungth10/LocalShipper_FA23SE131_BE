using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CheckMail
{
    public class Function1
    {
        private readonly ILogger _logger;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [Function("Function1")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            var queryParams = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            string email = queryParams["email"];

            if (string.IsNullOrEmpty(email))
            {
                response.WriteString("Please provide an email parameter in the query string.");
            }
            else
            {
                try
                {
                    var validationResponse = await CheckEmailValidity(email);
                    response.WriteString($"Email Validation Result: {validationResponse.Status}");
                }
                catch (Exception ex)
                {
                    response.WriteString($"Error: {ex.Message}");
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
