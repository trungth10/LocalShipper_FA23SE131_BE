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
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities.Collections;
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
             .Include(o => o.Store)
             .Include(o => o.Shipper)
             .Include(o => o.Action)
             .Include(o => o.Type)
             .Include(o => o.Route)
             .FirstOrDefaultAsync(a => a.Id == id && string.IsNullOrWhiteSpace(cancelReason));

            var orderCancel = await _unitOfWork.Repository<Order>()
             .GetAll()
             .Include(o => o.Store)
             .Include(o => o.Shipper)
             .Include(o => o.Action)
             .Include(o => o.Type)
             .Include(o => o.Route)
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

                if (order.Status == (int)OrderStatusEnum.WAITING)
                {
                    OrderHistory orderHistory = new OrderHistory
                    {
                        FromStatus = (int)OrderStatusEnum.WAITING,
                        ToStatus = (int)status,
                        OrderId = order.Id,
                        ShipperId = shipperId,
                        Status = (int)OrderHistoryStatusEnum.ACTICE
                    };
                    await _unitOfWork.Repository<OrderHistory>().InsertAsync(orderHistory);
                    await _unitOfWork.CommitAsync();
                }

                if (order.Status == (int)OrderStatusEnum.ASSIGNING)
                {
                    OrderHistory orderHistory = new OrderHistory
                    {
                        FromStatus = (int)OrderStatusEnum.ASSIGNING,
                        ToStatus = (int)status,
                        OrderId = order.Id,
                        ShipperId = shipperId,
                        Status = (int)OrderHistoryStatusEnum.ACTICE
                    };
                    await _unitOfWork.Repository<OrderHistory>().InsertAsync(orderHistory);
                    await _unitOfWork.CommitAsync();
                }


            }

            if (status == OrderStatusEnum.WAITING && string.IsNullOrWhiteSpace(cancelReason))
            {
                order.Status = (int)status;
                order.ShipperId = shipperId;

                OrderHistory orderHistory = new OrderHistory
                {
                    FromStatus = (int)OrderStatusEnum.IDLE,
                    ToStatus = (int)status,
                    OrderId = order.Id,
                    ShipperId = shipperId,
                    Status = (int)OrderHistoryStatusEnum.ACTICE
                };
                await _unitOfWork.Repository<OrderHistory>().InsertAsync(orderHistory);
                await _unitOfWork.CommitAsync();
            }


            if (status == OrderStatusEnum.INPROCESS && string.IsNullOrWhiteSpace(cancelReason))
            {
                order.Status = (int)status;
                order.PickupTime = DateTime.Now;

                OrderHistory orderHistory = new OrderHistory
                {
                    FromStatus = (int)OrderStatusEnum.ACCEPTED,
                    ToStatus = (int)status,
                    OrderId = order.Id,
                    ShipperId = shipperId,
                    Status = (int)OrderHistoryStatusEnum.ACTICE
                };
                await _unitOfWork.Repository<OrderHistory>().InsertAsync(orderHistory);
                await _unitOfWork.CommitAsync();
            }

            if (status == OrderStatusEnum.ASSIGNING && string.IsNullOrWhiteSpace(cancelReason))
            {
                order.Status = (int)status;
                order.PickupTime = DateTime.Now;

                OrderHistory orderHistory = new OrderHistory
                {
                    FromStatus = (int)OrderStatusEnum.IDLE,
                    ToStatus = (int)status,
                    OrderId = order.Id,
                    ShipperId = shipperId,
                    Status = (int)OrderHistoryStatusEnum.ACTICE
                };
                await _unitOfWork.Repository<OrderHistory>().InsertAsync(orderHistory);
                await _unitOfWork.CommitAsync();
            }

            if (status == OrderStatusEnum.IDLE && string.IsNullOrWhiteSpace(cancelReason))
            {
                order.Status = (int)status;
                order.ShipperId = null;
                OrderHistory orderHistory = new OrderHistory
                {
                    FromStatus = (int)OrderStatusEnum.CANCELLED,
                    ToStatus = (int)status,
                    OrderId = order.Id,
                    ShipperId = shipperId,
                    Status = (int)OrderHistoryStatusEnum.ACTICE
                };
                await _unitOfWork.Repository<OrderHistory>().InsertAsync(orderHistory);
                await _unitOfWork.CommitAsync();
            }

            if (status == OrderStatusEnum.COMPLETED && string.IsNullOrWhiteSpace(cancelReason))
            {
                order.Status = (int)status;
                order.CompleteTime = DateTime.Now;
                OrderHistory orderHistory = new OrderHistory
                {
                    FromStatus = (int)OrderStatusEnum.INPROCESS,
                    ToStatus = (int)status,
                    OrderId = order.Id,
                    ShipperId = shipperId,
                    Status = (int)OrderHistoryStatusEnum.ACTICE
                };
                await _unitOfWork.Repository<OrderHistory>().InsertAsync(orderHistory);
                await _unitOfWork.CommitAsync();

            }

            if (status == OrderStatusEnum.CANCELLED && string.IsNullOrWhiteSpace(cancelReason))
            {
                order.Status = (int)status;
                order.CompleteTime = DateTime.Now;
                OrderHistory orderHistory = new OrderHistory
                {
                    FromStatus = (int)OrderStatusEnum.INPROCESS,
                    ToStatus = (int)status,
                    OrderId = order.Id,
                    ShipperId = shipperId,
                    Status = (int)OrderHistoryStatusEnum.ACTICE
                };
                await _unitOfWork.Repository<OrderHistory>().InsertAsync(orderHistory);
                await _unitOfWork.CommitAsync();

            }


            if (status == OrderStatusEnum.RETURN)
            {
                orderCancel.Status = (int)status;
                orderCancel.CompleteTime = DateTime.Now;
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
        public async Task<List<OrderResponse>> GetOrder(int? id, int? status, int? storeId, int? shipperId,
                                     string? tracking_number, string? cancel_reason, decimal? distance_price,
                                     decimal? subtotal_price, decimal? COD, decimal? totalPrice, string? other, int? routeId,
                                     int? capacity, int? package_weight, int? package_width, int? package_height, int? package_length,
                                     string? customer_city, string? customer_commune, string? customer_district, string? customer_phone,
                                     string? customer_name, string? customer_email, int? actionId, int? typeId, int? pageNumber, int? pageSize)
        {

            var orders = _unitOfWork.Repository<Order>().GetAll()
                                                       .Include(o => o.Store)
                                                       .Include(o => o.Shipper)
                                                       .Include(o => o.Action)
                                                       .Include(o => o.Type)
                                                       .Include(o => o.Route)
                                                       .Where(a => (id == null || id == 0) || a.Id == id)
                                                       .Where(a => (status == null || status == 0) || a.Status == status)
                                                       .Where(a => (storeId == null || storeId == 0) || a.StoreId == storeId)
                                                       .Where(a => (shipperId == null || shipperId == 0) || a.ShipperId == shipperId)
                                                       .Where(a => string.IsNullOrWhiteSpace(tracking_number) || a.TrackingNumber.Contains(tracking_number.Trim()))
                                                       .Where(a => string.IsNullOrWhiteSpace(cancel_reason) || a.CancelReason.Contains(cancel_reason.Trim()))
                                                       .Where(a => (distance_price == null || distance_price == 0) || a.DistancePrice == distance_price)
                                                       .Where(a => (subtotal_price == null || subtotal_price == 0) || a.SubtotalPrice == subtotal_price)
                                                       .Where(a => (COD == null || COD == 0) || a.Cod == COD)
                                                       .Where(a => (totalPrice == null || totalPrice == 0) || a.TotalPrice == totalPrice)
                                                       .Where(a => string.IsNullOrWhiteSpace(other) || a.Other.Contains(other.Trim()))
                                                       .Where(a => (routeId == null || routeId == 0) || a.RouteId == routeId)
                                                       .Where(a => (capacity == null || capacity == 0) || a.Capacity == capacity)
                                                       .Where(a => (package_weight == null || package_weight == 0) || a.PackageWeight == routeId)
                                                       .Where(a => (package_width == null || package_width == 0) || a.PackageWidth == package_width)
                                                       .Where(a => (package_height == null || package_height == 0) || a.PackageHeight == package_height)
                                                       .Where(a => (package_length == null || package_length == 0) || a.PackageLength == package_length)
                                                       .Where(a => string.IsNullOrWhiteSpace(customer_city) || a.CustomerCity.Contains(customer_city.Trim()))
                                                       .Where(a => string.IsNullOrWhiteSpace(customer_commune) || a.CustomerCommune.Contains(customer_commune.Trim()))
                                                       .Where(a => string.IsNullOrWhiteSpace(customer_district) || a.CustomerDistrict.Contains(customer_district.Trim()))
                                                       .Where(a => string.IsNullOrWhiteSpace(customer_phone) || a.CustomerPhone.Contains(customer_phone.Trim()))
                                                       .Where(a => string.IsNullOrWhiteSpace(customer_name) || a.CustomerName.Contains(customer_name.Trim()))
                                                       .Where(a => string.IsNullOrWhiteSpace(customer_email) || a.CustomerEmail.Contains(customer_email.Trim()))
                                                       .Where(a => (actionId == null || actionId == 0) || a.ActionId == actionId)
                                                       .Where(a => (typeId == null || typeId == 0) || a.TypeId == typeId)
                                                       ;




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

            var orderResponse = _mapper.Map<List<OrderResponse>>(orderList);
            return orderResponse;
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
        //CREATE ORDER
        //UPDADTE ORDER

        //DELETE Order
        public async Task<OrderResponse> DeleteOrder(int id)
        {

            var order = await _unitOfWork.Repository<Order>()
                                         .GetAll()
                                         .Include(o => o.Store)
                                         .Include(o => o.Shipper)
                                         .Include(o => o.Action)
                                         .Include(o => o.Type)
                                         .Include(o => o.Route)
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

            var orderResponse = _mapper.Map<OrderResponse>(order);
            return orderResponse;
        }

    }



}
