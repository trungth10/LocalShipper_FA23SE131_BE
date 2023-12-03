﻿using LocalShipper.Service.Helpers;
using LocalShipper.Service.Services.Interface;
using MimeKit;
using MailKit.Net.Smtp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using MailKit.Security;
using System.Net.Http;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.DTOs.Request;
using Newtonsoft.Json;

namespace LocalShipper.Service.Services.Implement
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfiguration _emailConfig;
        public EmailService(EmailConfiguration emailConfig) => _emailConfig = emailConfig;
        public void SendEmail(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
            Send(emailMessage);
        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("LocalShipper", _emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content };

            return emailMessage;
        }

        private void Send(MimeMessage mailMessage)
        {
            using var client = new MailKit.Net.Smtp.SmtpClient();
            try
            {
                client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, SecureSocketOptions.StartTls);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(_emailConfig.UserName, _emailConfig.Password);

                client.Send(mailMessage);
            }
            catch
            {
                //log an error message or throw an exception or both.
                throw;
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
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

