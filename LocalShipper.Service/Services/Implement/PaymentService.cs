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
            , string? paymentImage, int? packageId, int? pageNumber, int? pageSize)
        {

            var payments = _unitOfWork.Repository<Payment>().GetAll()
                                                              .Include(o => o.Package)
                                                              .Where(a => id == 0 || a.Id == id)
                                                              .Where(a => string.IsNullOrWhiteSpace(paymentMethod) || a.PaymentMethod.Contains(paymentMethod.Trim()))
                                                              .Where(a => status == 0 || a.Status == status)
                                                              .Where(a => string.IsNullOrWhiteSpace(paymentCode) || a.PaymentCode.Contains(paymentCode.Trim()))
                                                              .Where(a => string.IsNullOrWhiteSpace(paymentImage) || a.PaymentImage.Contains(paymentImage.Trim()))
                                                              .Where(a => packageId == 0 || a.PackageId == packageId);
            // Xác định giá trị cuối cùng của pageNumber
            pageNumber = pageNumber.HasValue ? Math.Max(1, pageNumber.Value) : 1;
            // Áp dụng phân trang nếu có thông số pageNumber và pageSize
            if (pageNumber.HasValue && pageSize.HasValue)
            {
                payments = payments.Skip((pageNumber.Value - 1) * pageSize.Value)
                                       .Take(pageSize.Value);
            }

            var paymentList = await payments.ToListAsync();
            if (paymentList == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Package không có hoặc không tồn tại", id.ToString());
            }

            var paymentResponses = paymentList.Select(payment => new PaymentResponse
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
                    BatchId = (int)payment.Package.BatchId,
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
                                                                 .Include(o => o.Package)
                                                                 .Where(a => a.PackageId == request.PackageId)
                                                                 .ToListAsync();
            if(paymentCheck.Count != 0)
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
                    BatchId = (int)payment.Package.BatchId,
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

        public async Task<PaymentResponse> UpdatePayment(int id, PutPaymentRequest request)
        {
            var payments = await _unitOfWork.Repository<Payment>()
                .GetAll()
                .FirstOrDefaultAsync(a => a.Id == id);

            if (payments == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy ví", id.ToString());
            }

            var paymentCheck = await _unitOfWork.Repository<Payment>().GetAll()
                                                                 .Where(a => a.PackageId == request.PackageId && a.PackageId !=  payments.PackageId)
                                                                 .ToListAsync();

            if (paymentCheck.Count != 0)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Sản phẩm đã được thanh toán rồi", request.PackageId.ToString());
            }

            payments.PaymentMethod = request.PaymentMethod;
            payments.Status = (int)request.Status;
            payments.PaymentCode = request.PaymentCode;
            payments.PaymentImage = request.PaymentImage;
            payments.PackageId = request.PackageId;

            await _unitOfWork.Repository<Payment>().Update(payments, id);
            await _unitOfWork.CommitAsync();

            var payment = await _unitOfWork.Repository<Payment>()
                .GetAll()
                .Include(o => o.Package)
                .FirstOrDefaultAsync(a => a.Id == id);

            var paymentResponse = new PaymentResponse
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
                    BatchId = (int)payment.Package.BatchId,
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

        //DELETE Payment

        public async Task<MessageResponse> DeletePayment(int id)
        {
            var payments = await _unitOfWork.Repository<Payment>().GetAll()
            .FirstOrDefaultAsync(a => a.Id == id);

            if (payments == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy", id.ToString());
            }

            _unitOfWork.Repository<Payment>().Delete(payments);
            await _unitOfWork.CommitAsync();

            return new MessageResponse
            {
                Message = "Xóa thành công",
            };
        }

    }
}
