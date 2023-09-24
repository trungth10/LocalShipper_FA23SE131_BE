using AutoMapper;
using LocalShipper.Data.Models;
using LocalShipper.Data.Repository;
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
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public OrderService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<OrderResponse> UpdateShipperInOrder(int orderId, OrderRequest request)
        {
            try
            {
                var order = _unitOfWork.Repository<Order>().Find(x => x.Id == orderId);
                //var shipper = _unitOfWork.Repository<Shipper>().Find(x => x.Id == shipperId);

                if (order == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy đơn hàng", orderId.ToString());
                }

                order.ShipperId = (int)request.shipperId;
                order.AcceptTime = DateTime.Now;

                await _unitOfWork.Repository<Order>().Update(order, orderId);
                await _unitOfWork.CommitAsync();


                //return _mapper.Map<Shipper, OrderResponse>(order);
                return new OrderResponse
                {
                    Id = order.Id,
                    storeId = order.StoreId,
                    batchId = order.BatchId,
                    shipperId = order.ShipperId,
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

                };
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Cập nhật shipper đơn hàng thất bại", ex.InnerException?.Message);
            }
        }

        public async Task<OrderResponse> CompleteOrder(int orderId, UpdateOrderStatusRequest request)
        {
            try
            {
                var order = _unitOfWork.Repository<Order>().Find(x => x.Id == orderId);

                if (order == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy đơn hàng", order.ToString());
                }

                order.Status = (int)OrderStatusEnum.COMPLETED;
                order.CompleteTime = DateTime.Now;

                await _unitOfWork.Repository<Order>().Update(order, orderId);
                await _unitOfWork.CommitAsync();


                return _mapper.Map<Order, OrderResponse>(order);
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Hoàn tất đơn hàng thất bại", ex.InnerException?.Message);
            }
        }

        public async Task<OrderResponse> PickupProduct(int orderId, UpdateOrderStatusRequest request)
        {
            try
            {
                var order = _unitOfWork.Repository<Order>().Find(x => x.Id == orderId);

                if (order == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy đơn hàng", order.ToString());
                }

                order.Status = (int)OrderStatusEnum.INPROCESS;
                order.PickupTime = DateTime.Now;

                await _unitOfWork.Repository<Order>().Update(order, orderId);
                await _unitOfWork.CommitAsync();


                return _mapper.Map<Order, OrderResponse>(order);
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Nhận đơn hàng thất bại", ex.InnerException?.Message);
            }
        }

        public async Task<OrderResponse> CancelOrder(int orderId, UpdateOrderStatusRequest request)
        {
            try
            {
                var order = _unitOfWork.Repository<Order>().Find(x => x.Id == orderId);

                if (order == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy đơn hàng", order.ToString());
                }

                order.Status = (int)OrderStatusEnum.CANCELLED;
                order.CancelTime = DateTime.Now;
                order.CancelReason = request.cancelReason;

                await _unitOfWork.Repository<Order>().Update(order, orderId);
                await _unitOfWork.CommitAsync();


                return _mapper.Map<Order, OrderResponse>(order);
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Hủy đơn hàng thất bại", ex.InnerException?.Message);
            }
        }

        public async Task<OrderResponse> GetOrderById(int id)
        {
            var order = await _unitOfWork.Repository<Order>().GetAll().Include(o => o.Store).Include(o => o.Batch).Where(f => f.Id == id).FirstOrDefaultAsync();
            var orderResponse = new OrderResponse
            {
                Id = order.Id,
                storeId = order.StoreId,
                batchId = order.BatchId,
                shipperId = order.ShipperId,
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
            };

            if (order.Batch != null)
            {
                orderResponse.Batches = new BatchResponse
                {
                    Id = order.Batch.Id,
                    StoreId = order.Batch.StoreId,
                    BatchName = order.Batch.BatchName,
                    BatchDescription = order.Batch.BatchDescription,
                    CreatedAt = order.Batch.CreatedAt,
                    UpdateAt = order.Batch.UpdateAt,
                };
            }

            if (order.Store != null)
            {
                orderResponse.Store = new StoreResponse
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
                    BrandId = order.Store.BrandId,
                    TemplateId = order.Store.TemplateId,
                    ZoneId = order.Store.ZoneId,
                    AccountId = order.Store.AccountId,
                };
            }

            return orderResponse;
        }

        public async Task<List<OrderResponse>> GetOrdersByAssigning()
        {
            var status = (int)OrderStatusEnum.ASSIGNING;
            var orders = await _unitOfWork.Repository<Order>().GetAll()
                .Where(f => f.Status == status)
                .ToListAsync();

            var orderResponses = orders.Select(order => new OrderResponse
            {
                Id = order.Id,
                storeId = order.StoreId,
                batchId = order.BatchId,
                shipperId = order.ShipperId,
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
            }).ToList();

            return orderResponses;
        }

        public async Task<OrderListResponse> GetOrderByShipperId(int shipperId)
        {
            var orders = await _unitOfWork.Repository<Order>().GetAll().Include(o => o.Store).Include(o => o.Batch).Where(f => f.ShipperId == shipperId).ToListAsync();
            var orderResponses = new List<OrderResponse>();
            foreach (var order in orders)
            {
                var orderResponse = new OrderResponse
                {
                    Id = order.Id,
                    storeId = order.StoreId,
                    batchId = order.BatchId,
                    shipperId = order.ShipperId,
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
                };

                if (order.Batch != null)
                {
                    orderResponse.Batches = new BatchResponse
                    {
                        Id = order.Batch.Id,
                        StoreId = order.Batch.StoreId,
                        BatchName = order.Batch.BatchName,
                        BatchDescription = order.Batch.BatchDescription,
                        CreatedAt = order.Batch.CreatedAt,
                        UpdateAt = order.Batch.UpdateAt,
                    };
                }

                if (order.Store != null)
                {
                    orderResponse.Store = new StoreResponse
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
                        BrandId = order.Store.BrandId,
                        TemplateId = order.Store.TemplateId,
                        ZoneId = order.Store.ZoneId,
                        AccountId = order.Store.AccountId,
                    };
                }

                orderResponses.Add(orderResponse);
            }

            var orderListResponse = new OrderListResponse
            {
                Orders = orderResponses,
                OrderCount = orderResponses.Count
            };

            return orderListResponse;
        }

        public async Task<TotalPriceResponse> GetTotalPriceByOrderId(int orderId)
        {
            var order = _unitOfWork.Repository<Order>().Find(x => x.Id == orderId);
            var totalPriceResponse = new TotalPriceResponse
            {
                orderId = order.Id,
                subTotalprice = order.SubtotalPrice,
                distancePrice = order.DistancePrice,
                totalPrice = order.TotalPrice
            };
            return totalPriceResponse;
        }
        public async Task<decimal> GetTotalPriceSumByShipperId(int shipperId)
        {
            var orders = await _unitOfWork.Repository<Order>().GetAll().Where(f => f.ShipperId == shipperId).ToListAsync();
            decimal totalPriceSum = (decimal)orders.Sum(order => order.TotalPrice);
            return totalPriceSum;
        }


        public async Task<decimal> GetCancelRateByShipperId(int shipperId)
        {
            var packagesWithCancelReason = await _unitOfWork.Repository<Order>().GetAll()
                .Where(o => o.ShipperId == shipperId)
                .SelectMany(o => o.Batch.Packages) // Lấy tất cả các gói hàng từ batch của từng đơn hàng
                .Where(p => !string.IsNullOrEmpty(p.CancelReason)) // Lọc các gói hàng có lý do hủy
                .ToListAsync();

            int totalCancelReasonCount = packagesWithCancelReason.Count;

            var allPackages = await _unitOfWork.Repository<Order>().GetAll()
                .Where(o => o.ShipperId == shipperId)
                .SelectMany(o => o.Batch.Packages)
                .ToListAsync();

            if (allPackages.Count == 0)
            {
                return 0;
            }

            decimal cancelRate = (decimal)totalCancelReasonCount / allPackages.Count * 100;

            return cancelRate;
        }

        public async Task<decimal> GetReceiveRateByShipperId(int shipperId)
        {
            decimal cancelRate = await GetCancelRateByShipperId(shipperId);
            decimal receiveRate = 100 - cancelRate;
            return receiveRate;
        }
    }
}
