using AutoMapper;
using LocalShipper.Data.Models;
using LocalShipper.Data.UnitOfWork;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Exceptions;
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
    }
}
