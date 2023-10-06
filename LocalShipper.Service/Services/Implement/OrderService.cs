using AutoMapper;
using LocalShipper.Data.Models;
using LocalShipper.Data.Repository;
using LocalShipper.Data.UnitOfWork;
using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Exceptions;
using LocalShipper.Service.Helpers;
using LocalShipper.Service.Services.Interface;
using MailKit.Search;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Implement
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public OrderService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }


        //SHIPPER -> ORDER
        public async Task<OrderResponse> ShipperToStatusOrder(int id, int shipperId, string? cancelReason, OrderStatusEnum status)
        {

            var order = await _unitOfWork.Repository<Order>()
             .GetAll()
             .FirstOrDefaultAsync(a => a.Id == id && string.IsNullOrWhiteSpace(cancelReason));

            var orderCancel = await _unitOfWork.Repository<Order>()
             .GetAll()
             .FirstOrDefaultAsync(a => a.Id == id);

            var shipper = await _unitOfWork.Repository<Shipper>()
             .GetAll()
             .FirstOrDefaultAsync(a => a.Id == shipperId);

            if (shipper == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy Shipper", order.ToString());
            }

            if (shipper.Status == (int)ShipperStatusEnum.Offline)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Shipper đang ngoại tuyến", order.ToString());
            }

            if (order == null & orderCancel == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy đơn hàng", order.ToString());
            }

            //Shipper thao tác Order

            if (status == OrderStatusEnum.ACCEPTED && string.IsNullOrWhiteSpace(cancelReason))
            {
                order.Status = (int)status;
                order.ShipperId = shipperId;
                order.AcceptTime = DateTime.Now;
            }


            if (status == OrderStatusEnum.INPROCESS || string.IsNullOrWhiteSpace(cancelReason))
            {
                order.Status = (int)status;
                order.PickupTime = DateTime.Now;
            }

            if (status == OrderStatusEnum.COMPLETED && string.IsNullOrWhiteSpace(cancelReason))
            {
                order.Status = (int)status;
                order.CompleteTime = DateTime.Now;
            }

            if (status == OrderStatusEnum.COMPLETED && string.IsNullOrWhiteSpace(cancelReason))
            {
                order.Status = (int)status;
                order.CompleteTime = DateTime.Now;
            }

            if (status == OrderStatusEnum.CANCELLED)
            {
                orderCancel.Status = (int)status;
                orderCancel.CancelTime = DateTime.Now;
                orderCancel.CancelReason = cancelReason;
                await _unitOfWork.Repository<Order>().Update(orderCancel, id);
                await _unitOfWork.CommitAsync();

                return _mapper.Map<Order, OrderResponse>(orderCancel);
            }

            await _unitOfWork.Repository<Order>().Update(order, id);
            await _unitOfWork.CommitAsync();



            return _mapper.Map<Order, OrderResponse>(order);
        }

        public async Task<decimal> GetTotalPriceSumByShipperId(int shipperId)
        {
            var orders = await _unitOfWork.Repository<Order>().GetAll().Where(f => f.ShipperId == shipperId).ToListAsync();
            decimal totalPriceSum = (decimal)orders.Sum(order => order.TotalPrice);
            return totalPriceSum;
        }

        public async Task<decimal> GetCancelRateByShipperId(int shipperId)
        {
            var orders = await _unitOfWork.Repository<Order>().GetAll()
                                                              .Where(o => o.ShipperId == shipperId)
                                                              .ToListAsync();

            if (orders.Count == 0)
            {
                return 0;
            }

            int totalCancelledOrders = orders.Count(o => o.CancelTime != null);
            decimal cancelRate = (decimal)totalCancelledOrders / orders.Count * 100;

            return cancelRate;
        }

        public async Task<decimal> GetReceiveRateByShipperId(int shipperId)
        {
            var orders = await _unitOfWork.Repository<Order>().GetAll()
                                                              .Where(o => o.ShipperId == shipperId)
                                                              .ToListAsync();

            if (orders.Count == 0)
            {
                return 0;
            }

            int totalReceivedOrders = orders.Count(o => o.AcceptTime != null);
            decimal receiveRate = (decimal)totalReceivedOrders / orders.Count * 100;

            return receiveRate;
        }

        public async Task<TotalPriceAndTotalResponse> GetTotalPriceAndOrderCount(int shipperId, int? month, int? year, int? day)
        {
            DateTime startDate;
            DateTime endDate;

            if (month.HasValue && (month < 1 || month > 12))
            {
                throw new ArgumentException("Invalid month.", nameof(month));
            }

            if (day.HasValue && (day < 1 || day > 31))
            {
                throw new ArgumentException("Invalid day.", nameof(day));
            }

            if (year.HasValue)
            {
                if (!month.HasValue)
                {
                    throw new ArgumentException("Month is required when year is provided.", nameof(month));
                }

                startDate = new DateTime(year.Value, month.Value, 1);
                endDate = startDate.AddMonths(1).AddDays(-1);

                if (day.HasValue)
                {
                    startDate = startDate.AddDays(day.Value - 1);
                    endDate = startDate.AddDays(1);
                }
            }
            else
            {
                startDate = new DateTime(1, 1, 1);
                endDate = new DateTime(9999, 12, 31);
            }

            var ordersInRange = await _unitOfWork.Repository<Order>().GetAll()
                .Where(o => o.ShipperId == shipperId && o.CompleteTime >= startDate && o.CompleteTime <= endDate)
                .ToListAsync();

            decimal total = (decimal)ordersInRange.Sum(o => o.TotalPrice);
            int totalCount = ordersInRange.Count;

            var result = new TotalPriceAndTotalResponse
            {
                TotalPrice = total,
                TotalCount = totalCount
            };

            return result;
        }



        //GET Order
        public async Task<List<OrderResponse>> GetOrder(int? id, int? status, int? storeId, int? batchId, int? shipperId,
                                      string? tracking_number, string? cancel_reason, decimal? distance_price,
                                     decimal? subtotal_price, decimal? totalPrice, string? other, int? pageNumber, int? pageSize)
        {

            var orders = _unitOfWork.Repository<Order>().GetAll()
                                                      .Include(o => o.Store)
                                                      .Include(o => o.Batch)
                                                      .Include(o => o.Shipper)
                                                      .Where(a => (id == null || id == 0) || a.Id == id)
                                                      .Where(a => (status == null || status == 0) || a.Status == status)
                                                      .Where(a => (storeId == null || storeId == 0) || a.StoreId == storeId)
                                                      .Where(a => (batchId == null || batchId == 0) || a.BatchId == batchId)
                                                      .Where(a => (shipperId == null || shipperId == 0) || a.ShipperId == shipperId)
                                                      .Where(a => string.IsNullOrWhiteSpace(tracking_number) || a.TrackingNumber.Contains(tracking_number.Trim()))
                                                      .Where(a => string.IsNullOrWhiteSpace(cancel_reason) || a.CancelReason.Contains(cancel_reason.Trim()))
                                                      .Where(a => (distance_price == null || distance_price == 0) || a.DistancePrice == distance_price)
                                                      .Where(a => (subtotal_price == null || subtotal_price == 0) || a.SubtotalPrice == subtotal_price)
                                                      .Where(a => (totalPrice == null || totalPrice == 0) || a.TotalPrice == totalPrice)
                                                      .Where(a => string.IsNullOrWhiteSpace(other) || a.Other.Contains(other.Trim()));


            // Xác định giá trị cuối cùng của pageNumber
            pageNumber = pageNumber.HasValue ? Math.Max(1, pageNumber.Value) : 1;
            // Áp dụng phân trang nếu có thông số pageNumber và pageSize
            if (pageNumber.HasValue && pageSize.HasValue)
            {
                orders = orders.Skip((pageNumber.Value - 1) * pageSize.Value)
                                       .Take(pageSize.Value);
            }

            var orderList = await orders.ToListAsync();
            if (orderList == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Order không có hoặc không tồn tại", id.ToString());
            }

            var orderResponses = orderList.Select(order => new OrderResponse
            {
                Id = order.Id,
                storeId = order.StoreId,
                batchId = order.BatchId,
                shipperId = order.ShipperId == null ? null : (int)order.ShipperId,
                status = order.Status,
                trackingNumber = order.TrackingNumber,
                createTime = order.CreateTime,
                orderTime = order.OrderTime,
                acceptTime = order.AcceptTime,
                pickupTime = order.PickupTime,
                cancelTime = order.CancelTime,
                cancelReason = order.CancelReason,
                completeTime = order.CompleteTime,
                distancePrice = order.DistancePrice,
                subTotalprice = order.SubtotalPrice,
                totalPrice = order.TotalPrice,
                other = order.Other,

                Store = order.Store != null ? new StoreResponse
                {
                    Id = order.StoreId,
                    StoreName = order.Store.StoreName,
                    StoreAddress = order.Store.StoreAddress,
                    StorePhone = order.Store.StorePhone,
                    StoreEmail = order.Store.StoreEmail,
                    OpenTime = order.Store.OpenTime,
                    CloseTime = order.Store.CloseTime,
                    StoreDescription = order.Store.StoreDescription,
                    Status = order.Store.Status,
                    TemplateId = order.Store.TemplateId,
                    ZoneId = order.Store.ZoneId,
                    AccountId = order.Store.AccountId,
                } : null,
                Batches = order.Batch != null ? new BatchResponse
                {
                    Id = order.Batch.Id,
                    BatchName = order.Batch.BatchName,
                    BatchDescription = order.Batch.BatchDescription,
                    CreatedAt = order.Batch.CreatedAt,
                    UpdateAt = order.Batch.UpdateAt,
                } : null,
                Shipper = order.Shipper != null ? new ShipperResponse
                {
                    Id = order.Shipper.Id,
                    FullName = order.Shipper.FullName,
                    EmailShipper = order.Shipper.EmailShipper,
                    PhoneShipper = order.Shipper.PhoneShipper,
                    AddressShipper = order.Shipper.AddressShipper,
                    TransportId = (int)order.Shipper.TransportId,
                    AccountId = order.Shipper.AccountId,
                    ZoneId = (int)order.Shipper.ZoneId,
                    Status = (ShipperStatusEnum)order.Shipper.Status,
                    Fcmtoken = order.Shipper.Fcmtoken,
                    WalletId = order.Shipper.WalletId
                } : null
            }).ToList();
            return orderResponses;
        }


        //GET Count
        public async Task<int> GetTotalOrderCount(int? storeId, int? shipperId)
        {
            var count = await _unitOfWork.Repository<Order>()
                .GetAll()
                .Where(a => storeId == 0 || a.StoreId == storeId)
                .Where(a => shipperId == 0 || a.ShipperId == shipperId)
                .CountAsync();
            return count;
        }

        //Store 
        //CREATE Order
        public async Task<MessageResponse> CreateOrder(OrderRequest request)
        {
            string trackingNumber = GenerateRandomTrackingNumber();

            var package = await _unitOfWork.Repository<Package>().GetAll()
                                                                 .Where(a => a.BatchId == request.batchId)
                                                                 .ToListAsync();

            decimal distancePrice = package.Sum(p => p.DistancePrice);
            decimal totalPrice = (decimal)package.Sum(p => p.TotalPrice);
            decimal subTotalPrice = (decimal)package.Sum(p => p.SubtotalPrice);

            var batchIdExisted = await _unitOfWork.Repository<Order>().FindAsync(x => x.BatchId == request.batchId);
            if (batchIdExisted != null)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Lô hàng đang trong đơn hàng khác", request.batchId.ToString());
            }

            Order order = new Order
            {
                Status = (int)OrderStatusEnum.IDLE,
                StoreId = request.storeId,
                BatchId = request.batchId,
                TrackingNumber = trackingNumber,
                DistancePrice = distancePrice,
                SubtotalPrice = subTotalPrice,
                TotalPrice = totalPrice,

            };
            await _unitOfWork.Repository<Order>().InsertAsync(order);
            await _unitOfWork.CommitAsync();


            var messageResponse = new MessageResponse
            {
                Message = "Tạo đơn hàng thành công",
                id = order.Id,
            };
            return messageResponse;

        }

        private string GenerateRandomTrackingNumber()
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

        //UPDATE Order
        public async Task<OrderResponse> UpdateOrder(int id, PutOrderRequest orderRequest)
        {
            var orders = await _unitOfWork.Repository<Order>()
                 .GetAll()
                 .Include(o => o.Store)
                 .Include(o => o.Batch)
                 .Include(o => o.Shipper)
                 .FirstOrDefaultAsync(a => a.Id == id);

            if (orders == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy đơn hàng", id.ToString());
            }

            var batchIdExisted = await _unitOfWork.Repository<Order>().FindAsync(x => x.BatchId == orderRequest.batchId);
            if (batchIdExisted != null)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Lô hàng đang trong đơn hàng khác", orderRequest.batchId.ToString());
            }

            var trackingNumberExisted = await _unitOfWork.Repository<Order>().FindAsync(x => x.TrackingNumber == orderRequest.trackingNumber);
            if (trackingNumberExisted != null)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Mã số định danh đơn hàng bị trùng", orderRequest.batchId.ToString());
            }

            orders.StoreId = orderRequest.storeId;
            orders.BatchId = orderRequest.batchId;
            orders.Status = orderRequest.status;
            orders.ShipperId = orderRequest.shipperId;
            orders.TrackingNumber = orderRequest.trackingNumber;
            orders.CreateTime = orderRequest.createTime;
            orders.OrderTime = orderRequest.orderTime;
            orders.AcceptTime = orderRequest.acceptTime;
            orders.PickupTime = orderRequest.pickupTime;
            orders.CancelTime = orderRequest.cancelTime;
            orders.CancelReason = orderRequest.cancelReason;
            orders.CompleteTime = orderRequest.completeTime;
            orders.DistancePrice = orderRequest.distancePrice;
            orders.SubtotalPrice = orderRequest.subTotalprice;
            orders.TotalPrice = orderRequest.totalPrice;
            orders.Other = orderRequest.other;



            await _unitOfWork.Repository<Order>().Update(orders, id);
            await _unitOfWork.CommitAsync();

            var order = await _unitOfWork.Repository<Order>()
                 .GetAll()
                 .Include(o => o.Store)
                 .Include(o => o.Batch)
                 .Include(o => o.Shipper)
                 .FirstOrDefaultAsync(a => a.Id == id);

            var updatedOrderResponse = new OrderResponse
            {
                Id = order.Id,
                storeId = order.StoreId,
                batchId = order.BatchId,
                shipperId = (int)order.ShipperId,
                status = order.Status,
                trackingNumber = order.TrackingNumber,
                createTime = order.CreateTime,
                orderTime = order.OrderTime,
                acceptTime = order.AcceptTime,
                pickupTime = order.PickupTime,
                cancelTime = order.CancelTime,
                cancelReason = order.CancelReason,
                completeTime = order.CompleteTime,
                distancePrice = order.DistancePrice,
                subTotalprice = order.SubtotalPrice,
                totalPrice = order.TotalPrice,
                other = order.Other,

                Store = order.Store != null ? new StoreResponse
                {
                    Id = order.StoreId,
                    StoreName = order.Store.StoreName,
                    StoreAddress = order.Store.StoreAddress,
                    StorePhone = order.Store.StorePhone,
                    StoreEmail = order.Store.StoreEmail,
                    OpenTime = order.Store.OpenTime,
                    CloseTime = order.Store.CloseTime,
                    StoreDescription = order.Store.StoreDescription,
                    Status = order.Store.Status,
                    TemplateId = order.Store.TemplateId,
                    ZoneId = order.Store.ZoneId,
                    AccountId = order.Store.AccountId,
                } : null,
                Batches = order.Batch != null ? new BatchResponse
                {
                    Id = order.Batch.Id,
                    BatchName = order.Batch.BatchName,
                    BatchDescription = order.Batch.BatchDescription,
                    CreatedAt = order.Batch.CreatedAt,
                    UpdateAt = order.Batch.UpdateAt,
                } : null,
                Shipper = order.Shipper != null ? new ShipperResponse
                {
                    Id = order.Shipper.Id,
                    FullName = order.Shipper.FullName,
                    EmailShipper = order.Shipper.EmailShipper,
                    PhoneShipper = order.Shipper.PhoneShipper,
                    AddressShipper = order.Shipper.AddressShipper,
                    TransportId = (int)order.Shipper.TransportId,
                    AccountId = order.Shipper.AccountId,
                    ZoneId = (int)order.Shipper.ZoneId,
                    Status = (ShipperStatusEnum)order.Shipper.Status,
                    Fcmtoken = order.Shipper.Fcmtoken,
                    WalletId = order.Shipper.WalletId
                } : null
            };

            return updatedOrderResponse;
        }

        //DELETE Order
        public async Task<OrderResponse> DeleteOrder(int id)
        {

            var order = await _unitOfWork.Repository<Order>()
           .GetAll()
                 .Include(o => o.Store)
                 .Include(o => o.Batch)
                 .Include(o => o.Shipper)
                 .FirstOrDefaultAsync(a => a.Id == id);

            if (order == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy đơn hàng", id.ToString());
            }

            if (order.Status == (int)OrderStatusEnum.DELETED)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Đơn hàng đã bị xóa rồi", id.ToString());
            }

            order.Status = (int)OrderStatusEnum.DELETED;

            await _unitOfWork.Repository<Order>().Update(order, id);
            await _unitOfWork.CommitAsync();

            var updatedOrderResponse = new OrderResponse
            {
                Id = order.Id,
                storeId = order.StoreId,
                batchId = order.BatchId,
                shipperId = (int)order.ShipperId,
                status = order.Status,
                trackingNumber = order.TrackingNumber,
                createTime = order.CreateTime,
                orderTime = order.OrderTime,
                acceptTime = order.AcceptTime,
                pickupTime = order.PickupTime,
                cancelTime = order.CancelTime,
                cancelReason = order.CancelReason,
                completeTime = order.CompleteTime,
                distancePrice = order.DistancePrice,
                subTotalprice = order.SubtotalPrice,
                totalPrice = order.TotalPrice,
                other = order.Other,

                Store = order.Store != null ? new StoreResponse
                {
                    Id = order.StoreId,
                    StoreName = order.Store.StoreName,
                    StoreAddress = order.Store.StoreAddress,
                    StorePhone = order.Store.StorePhone,
                    StoreEmail = order.Store.StoreEmail,
                    OpenTime = order.Store.OpenTime,
                    CloseTime = order.Store.CloseTime,
                    StoreDescription = order.Store.StoreDescription,
                    Status = order.Store.Status,
                    TemplateId = order.Store.TemplateId,
                    ZoneId = order.Store.ZoneId,
                    AccountId = order.Store.AccountId,
                } : null,
                Batches = order.Batch != null ? new BatchResponse
                {
                    Id = order.Batch.Id,
                    BatchName = order.Batch.BatchName,
                    BatchDescription = order.Batch.BatchDescription,
                    CreatedAt = order.Batch.CreatedAt,
                    UpdateAt = order.Batch.UpdateAt,
                } : null,
                Shipper = order.Shipper != null ? new ShipperResponse
                {
                    Id = order.Shipper.Id,
                    FullName = order.Shipper.FullName,
                    EmailShipper = order.Shipper.EmailShipper,
                    PhoneShipper = order.Shipper.PhoneShipper,
                    AddressShipper = order.Shipper.AddressShipper,
                    TransportId = (int)order.Shipper.TransportId,
                    AccountId = order.Shipper.AccountId,
                    ZoneId = (int)order.Shipper.ZoneId,
                    Status = (ShipperStatusEnum)order.Shipper.Status,
                    Fcmtoken = order.Shipper.Fcmtoken,
                    WalletId = order.Shipper.WalletId
                } : null
            };

            return updatedOrderResponse;
        }

    }



}
