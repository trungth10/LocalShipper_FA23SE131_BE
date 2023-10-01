using AutoMapper;
using LocalShipper.Data.Models;
using LocalShipper.Data.UnitOfWork;
using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Exceptions;
using LocalShipper.Service.Helpers;
using LocalShipper.Service.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Implement
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public PaymentService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        //Get Payment  
        public async Task<List<PaymentResponse>> GetPayment(int? id, string? paymentMethod, int? status, string? paymentCode
            , string? paymentImage, int? packageId)
        {

            var payments = await _unitOfWork.Repository<Payment>().GetAll()
                                                              .Include(o => o.Package)
                                                              .Where(a => id == 0 || a.Id == id)
                                                              .Where(a => string.IsNullOrWhiteSpace(paymentMethod) || a.PaymentMethod.Contains(paymentMethod))
                                                              .Where(a => status == 0 || a.Status == status)
                                                              .Where(a => string.IsNullOrWhiteSpace(paymentCode) || a.PaymentCode.Contains(paymentCode))
                                                              .Where(a => string.IsNullOrWhiteSpace(paymentImage) || a.PaymentImage.Contains(paymentImage))
                                                              .Where(a => packageId == 0 || a.PackageId == packageId)
                                                              .ToListAsync();

            if (payments == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không có hoặc không tồn tại", id.ToString());
            }

            var paymentResponses = payments.Select(payment => new PaymentResponse
            {
                Id = payment.Id,
                PaymentMethod = payment.PaymentMethod,
                PaymentDate = payment.PaymentDate,
                Status = payment.Status,
                PaymentCode = payment.PaymentCode,
                PaymentImage = payment.PaymentImage,
                PackageId = payment.PackageId,
                Package = payment.Package != null ? new PackageResponse
                {
                    Id = payment.Package.Id,
                    BatchId = payment.Package.BatchId,
                    Capacity = payment.Package.Capacity,
                    PackageWeight = payment.Package.PackageWeight,
                    PackageWidth = payment.Package.PackageWidth,
                    PackageHeight = payment.Package.PackageHeight,
                    PackageLength = payment.Package.PackageLength,
                    Status = payment.Package.Status,
                    CustomerAddress = payment.Package.CustomerAddress,
                    CustomerName = payment.Package.CustomerName,
                    CustomerEmail = payment.Package.CustomerEmail,
                    CancelReason = payment.Package.CancelReason,
                    SubtotalPrice = payment.Package.SubtotalPrice,
                    DistancePrice = payment.Package.DistancePrice,
                    ActionId = payment.Package.ActionId,
                    TypeId = payment.Package.TypeId,
                } : null
            }).ToList();
            return paymentResponses;
        }

        //GET count
        public async Task<int> GetTotalPaymentCount()
        {
            var count = await _unitOfWork.Repository<Payment>()
                .GetAll()
                .CountAsync();

            return count;
        }

        //CREATE Payment
        public async Task<PaymentResponse> CreatePayment(PaymentRequest request)
        {
            string paymentCode = GenerateRandomPaymentCode();

            var paymentCheck = await _unitOfWork.Repository<Payment>().GetAll()
                                                                 .Where(a => a.PackageId == request.PackageId)
                                                                 .ToListAsync();
            if(paymentCheck != null)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Sản phẩm đã được thanh toán rồi", request.PackageId.ToString());
            }

            Payment payment = new Payment
            {
                PaymentMethod = request.PaymentMethod,
                Status = (int)PaymentStatusEnum.ACTIVE,
                PaymentCode = paymentCode,
                PaymentImage = request.PaymentImage,
                PackageId = request.PackageId,
            };
            await _unitOfWork.Repository<Payment>().InsertAsync(payment);
            await _unitOfWork.CommitAsync();


            var paymentResponse = new PaymentResponse
            {
                Id = payment.Id,
                PaymentMethod = payment.PaymentMethod,
                PaymentDate = payment.PaymentDate,
                Status = payment.Status,
                PaymentCode= payment.PaymentCode,
                PaymentImage= payment.PaymentImage,
                PackageId = payment.PackageId,
                Package = payment.Package != null ? new PackageResponse
                {
                    Id = payment.Package.Id,
                    BatchId = payment.Package.BatchId,
                    Capacity = payment.Package.Capacity,
                    PackageWeight = payment.Package.PackageWeight,
                    PackageWidth = payment.Package.PackageWidth,
                    PackageHeight = payment.Package.PackageHeight,
                    PackageLength = payment.Package.PackageLength,
                    Status = payment.Package.Status,
                    CustomerAddress = payment.Package.CustomerAddress,
                    CustomerName = payment.Package.CustomerName,
                    CustomerEmail = payment.Package.CustomerEmail,
                    CancelReason = payment.Package.CancelReason,
                    SubtotalPrice = payment.Package.SubtotalPrice,
                    DistancePrice = payment.Package.DistancePrice,
                    ActionId = payment.Package.ActionId,
                    TypeId = payment.Package.TypeId,
                } : null

            };
            return paymentResponse;
        }

        private string GenerateRandomPaymentCode()
        {
            string allowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            int length = 7;

            Random random = new Random();

            char[] trackingNumber = new char[length];
            for (int i = 0; i < length; i++)
            {
                trackingNumber[i] = allowedChars[random.Next(0, allowedChars.Length)];
            }

            string generatedTrackingNumber = new string(trackingNumber);

            return generatedTrackingNumber;
        }
    }
}
