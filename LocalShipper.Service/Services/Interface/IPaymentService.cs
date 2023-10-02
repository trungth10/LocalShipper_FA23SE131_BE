using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Interface
{
    public interface IPaymentService
    {
        Task<List<PaymentResponse>> GetPayment(int? id, string? paymentMethod, int? status, string? paymentCode
            , string? paymentImage, int? packageId);
        Task<int> GetTotalPaymentCount();
        Task<PaymentResponse> CreatePayment(PaymentRequest request);
        Task<PaymentResponse> UpdatePayment(int id, PutPaymentRequest request);
        Task<MessageResponse> DeletePayment(int id);
    }
}
