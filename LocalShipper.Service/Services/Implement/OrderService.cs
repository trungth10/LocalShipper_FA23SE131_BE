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
                    if (order.ShipperId != null)
                    {
                        throw new CrudException(HttpStatusCode.NotFound, "Shipper khác đã nhận đơn hàng!", order.ToString());
                    }

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
                                     string? tracking_number, string? cancel_reason, decimal? distance,  decimal? distance_price,
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
                                                       .Where(a => (distance == null || distance == 0) || a.Distance == distance)
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

        //GET Order v2
        public async Task<List<OrderResponse>> GetOrderV2( OrderRequestV2 request,int? pageNumber, int? pageSize)
        {

            var orders = _unitOfWork.Repository<Order>().GetAll()
                                                       .Include(o => o.Store)
                                                       .Include(o => o.Shipper)
                                                       .Include(o => o.Action)
                                                       .Include(o => o.Type)
                                                       .Include(o => o.Route)
                                                       .Where(a => !request.id.Any()  || request.id.Contains(a.Id))
                                                       .Where(a => !request.status.Any() || request.status.Contains(a.Status))
                                                       .Where(a => !request.storeId.Any() || request.storeId.Contains(a.StoreId))
                                                       .Where(a => !request.shipperId.Any() || request.shipperId.Contains((int)a.ShipperId))
                                                       .Where(a => !request.tracking_number.Any() || request.tracking_number.Contains(a.TrackingNumber))
                                                       .Where(a => !request.cancel_reason.Any() || request.cancel_reason.Contains(a.CancelReason))                                                       
                                                       .Where(a => (request.COD == null || request.COD == 0) || a.Cod <= request.COD)
                                                       .Where(a => (request.Distance == null || request.Distance == 0) || a.Distance <= request.Distance)
                                                       .Where(a => !request.other.Any() || request.other.Contains(a.Other))
                                                       .Where(a => !request.routeId.Any() || request.routeId.Contains((int)a.RouteId))
                                                       .Where(a => (request.capacity == null || request.capacity == 0) || a.Capacity <= request.capacity)                                                      
                                                       .Where(a => !request.customer_city.Any() || request.customer_city.Contains(a.CustomerCity))
                                                       .Where(a => !request.customer_commune.Any() || request.customer_commune.Contains(a.CustomerCommune))
                                                       .Where(a => !request.customer_district.Any() || request.customer_district.Contains(a.CustomerDistrict))
                                                       .Where(a => !request.customer_phone.Any() || request.customer_phone.Contains(a.CustomerPhone))
                                                       .Where(a => !request.customer_name.Any() || request.customer_name.Contains(a.CustomerName))
                                                       .Where(a => !request.customer_email.Any() || request.customer_email.Contains(a.CustomerEmail))
                                                       .Where(a => !request.actionId.Any() || request.actionId.Contains(a.ActionId))
                                                       .Where(a => !request.typeId.Any() || request.typeId.Contains(a.TypeId))
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
                throw new CrudException(HttpStatusCode.NotFound, "Order không có hoặc không tồn tại", "ERROR");
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


        private int? GetPriceItemId(List<PriceItem> priceItems, double distancePrice)
        {
            foreach (var priceItem in priceItems)
            {

                if (distancePrice >= priceItem.MinDistance && distancePrice <= priceItem.MaxDistance)
                {

                    return priceItem.Id;
                }
            }


            return null;
        }
        public async Task<decimal?> GetMaxDistance(int? storeId)
        {
            var priceL = await _unitOfWork.Repository<PriceL>().GetAll()
                .FirstOrDefaultAsync(b => b.StoreId == storeId);

            if (priceL != null)
            {
                var maxDistance = await _unitOfWork.Repository<PriceItem>().GetAll()
                    .Where(b => b.PriceId == priceL.Id)
                    .Select(b => b.MaxDistance)
                    .MaxAsync();

                return (decimal)maxDistance;
            }

            return null;
        }
        //Store
        //CREATE ORDER
        public async Task<OrderCreateResponse> CreateOrder(OrderRequestForCreate request)
        {

            decimal distancePrice = 0;
            decimal distancePriceMax1 = 0;
            decimal max1;
            decimal min1;
            decimal max2;
            decimal min2;
            decimal minAmount1;
            decimal minAmount2;
            decimal maxAmount1;
            decimal maxAmount2;
            decimal price1;
            if (request.StoreId.HasValue)
            {
                var priceL = await _unitOfWork.Repository<PriceL>().GetAll()
                    .FirstOrDefaultAsync(b => b.StoreId == request.StoreId);

                decimal? maxDistance = await GetMaxDistance(priceL.StoreId);

                if (priceL != null)
                {
                    var priceItems = await _unitOfWork.Repository<PriceItem>().GetAll()
                        .Where(b => b.PriceId == priceL.Id)
                        .ToListAsync();
                    var id = priceItems.Select(b => b.Id).ToList();

                    var firstId = id.FirstOrDefault();

                    var secondItem = id.Skip(1).FirstOrDefault();
                    if (GetPriceItemId(priceItems, (double)request.DistancePrice) == priceItems.FirstOrDefault().Id)
                    {

                        price1 = priceItems
                       .Where(b => b.MinDistance <= (double)request.DistancePrice && b.MaxDistance >= (double)request.DistancePrice)
                       .Select(b => (decimal)b.Price)
                       .FirstOrDefault();
                        max1 = priceItems
                                .Where(b => b.MinDistance <= (double)request.DistancePrice && b.MaxDistance >= (double)request.DistancePrice && firstId == priceItems.FirstOrDefault().Id)
                                .Select(b => (decimal)b.MaxDistance)
                                .FirstOrDefault();
                        maxAmount1 = priceItems
                                .Where(b => b.MaxDistance < (double)request.DistancePrice)
                                .Select(b => (decimal)b.MaxAmount)
                                .FirstOrDefault();
                        min1 = priceItems
                                .Where(b => b.MinDistance <= (double)request.DistancePrice && b.MaxDistance >= (double)request.DistancePrice && firstId == priceItems.FirstOrDefault().Id)
                                .Select(b => (decimal)b.MinDistance)
                                .FirstOrDefault();
                        minAmount1 = priceItems
                                .Where(b => b.MinDistance <= (double)request.DistancePrice && b.MaxDistance >= (double)request.DistancePrice && firstId == priceItems.FirstOrDefault().Id)
                                .Select(b => (decimal)b.MinAmount)
                                .FirstOrDefault();
                        if (request.DistancePrice >= min1 && request.DistancePrice <= max1)
                        {
                            distancePrice = priceItems
                                .Where(b => b.MinDistance <= (double)request.DistancePrice && b.MaxDistance >= (double)request.DistancePrice)
                                .Select(b => b.Price)
                                .FirstOrDefault();
                            distancePriceMax1 = priceItems
                                .Where(b => b.MinDistance <= (double)request.DistancePrice && b.MaxDistance >= (double)request.DistancePrice)
                                .Select(b => (decimal)b.MaxDistance)
                                .FirstOrDefault();
                            if (request.DistancePrice <= distancePriceMax1 && request.DistancePrice >= 1)
                            {
                                distancePrice = request.DistancePrice * distancePrice;


                            }
                            else if (request.DistancePrice < 1)
                            {
                                distancePrice = minAmount1;
                            }
                            else if (request.DistancePrice > max1)
                            {
                                distancePrice = maxAmount1;

                            }

                        }

                    }
                      
                    else if (GetPriceItemId(priceItems, (double)request.DistancePrice) == priceItems.Skip(1).FirstOrDefault().Id || request.DistancePrice > maxDistance.Value)
                    {

                        max2 = priceItems
                            .Where(b => b.MinDistance <= (double)request.DistancePrice && b.MaxDistance >= (double)request.DistancePrice)
                            .Select(b => (decimal)b.MaxDistance)
                            .FirstOrDefault();
                        maxAmount2 = priceItems
                               .Where(b => b.MaxDistance < (double)request.DistancePrice)
                               .Select(b => (decimal)b.MaxAmount)
                               .FirstOrDefault();
                        min2 = priceItems
                                .Where(b => b.MinDistance <= (double)request.DistancePrice && b.MaxDistance >= (double)request.DistancePrice)
                                .Select(b => (decimal)b.MinDistance)
                                .FirstOrDefault();
                        minAmount2 = priceItems
                                .Where(b => b.MinDistance <= (double)request.DistancePrice && b.MaxDistance >= (double)request.DistancePrice)
                                .Select(b => (decimal)b.MinAmount)
                                .FirstOrDefault();

                        decimal max = (decimal)priceItems.FirstOrDefault().MaxDistance;
                        decimal price = (decimal)priceItems.FirstOrDefault().Price;
                        if (request.DistancePrice >= min2 && request.DistancePrice <= max2)
                        {
                            var distancePrice2 = priceItems
                                .Where(b => b.MinDistance >= (double)min2 && b.MaxDistance <= (double)max2)
                                .Select(b => b.Price)
                                .FirstOrDefault();
                            distancePrice = max * price + (request.DistancePrice - max) * distancePrice2;
                        }
                        else if (request.DistancePrice > max2 && request.DistancePrice <= min2)
                        {
                            distancePrice = minAmount2;
                        }
                        else if (request.DistancePrice > max2)
                        {
                            distancePrice = maxAmount2;
                        }



                    }
                }
            }


            var newOrder = new Order
            {
                StoreId = request.StoreId.HasValue ? request.StoreId.Value : 0,
                TrackingNumber = request.TrackingNumber.Trim(),
                Distance = request.Distance,
                DistancePrice = distancePrice,
                SubtotalPrice = request.SubtotalPrice,
                Cod = request.Cod,
                TotalPrice = distancePrice + request.SubtotalPrice + request.Cod,
                Capacity = request.Capacity,
                PackageWeight = request.PackageWeight,
                PackageHeight = request.PackageHeight,
                PackageLength = request.PackageLength,
                PackageWidth = request.PackageWidth,
                CustomerCity = request.CustomerCity.Trim(),
                CustomerCommune = request.CustomerCommune.Trim(),
                CustomerDistrict = request.CustomerDistrict.Trim(),
                CustomerEmail = request.CustomerEmail.Trim(),
                CustomerName = request.CustomerName.Trim(),
                CustomerPhone = request.CustomerPhone.Trim(),
                ActionId = request.ActionId,
                TypeId = request.TypeId,
                CreateTime = DateTime.Now,
                OrderTime = request.OrderTime,
                Status = (int)OrderStatusEnum.IDLE,
            };
            await _unitOfWork.Repository<Order>().InsertAsync(newOrder);
            await _unitOfWork.CommitAsync();
            var orderResponse = _mapper.Map<OrderCreateResponse>(newOrder);
           
            return orderResponse;
        }


        //UPDADTE ORDER
        public async Task<OrderResponse> UpdateOrder(int id, OrderRequestForUpdate request)
        {
            var storeId = await _unitOfWork.Repository<Order>().GetAll().Where(b => b.Id == id).Select(b => b.StoreId).FirstOrDefaultAsync();
            var customerName = await _unitOfWork.Repository<Order>().GetAll().Where(b => b.Id == id).Select(b => b.CustomerName).FirstOrDefaultAsync();
            var customerEmail = await _unitOfWork.Repository<Order>().GetAll().Where(b => b.Id == id).Select(b => b.CustomerEmail).FirstOrDefaultAsync();
            var customerPhone = await _unitOfWork.Repository<Order>().GetAll().Where(b => b.Id == id).Select(b => b.CustomerPhone).FirstOrDefaultAsync();
            var customerCity = await _unitOfWork.Repository<Order>().GetAll().Where(b => b.Id == id).Select(b => b.CustomerCity).FirstOrDefaultAsync();
            var customerCommune = await _unitOfWork.Repository<Order>().GetAll().Where(b => b.Id == id).Select(b => b.CustomerCommune).FirstOrDefaultAsync();
            var customerDistric = await _unitOfWork.Repository<Order>().GetAll().Where(b => b.Id == id).Select(b => b.CustomerDistrict).FirstOrDefaultAsync();
            var other = await _unitOfWork.Repository<Order>().GetAll().Where(b => b.Id == id).Select(b => b.Other).FirstOrDefaultAsync();

            var order = await _unitOfWork.Repository<Order>()
         .GetAll().Include(b => b.Type).Include(b => b.Action).Include(b => b.Route).Include(b => b.Shipper).Include(b => b.Store)
         .FirstOrDefaultAsync(p => p.Id == id);
            #region price
            decimal distancePrice = 0;
            decimal distancePriceMax1 = 0;

            decimal max1;
            decimal min1;
            decimal max2;
            decimal min2;
            decimal minAmount1;
            decimal minAmount2;
            decimal maxAmount1;
            decimal maxAmount2;
            decimal price1;

            if (storeId != 0)
            {
                var priceL = await _unitOfWork.Repository<PriceL>().GetAll()
                    .FirstOrDefaultAsync(b => b.StoreId == storeId);

                if (priceL != null)
                {
                    var priceItems = await _unitOfWork.Repository<PriceItem>().GetAll()
                        .Where(b => b.PriceId == priceL.Id)
                        .ToListAsync();
                    var ids = priceItems.Select(b => b.Id).ToList();

                    var firstId = ids.FirstOrDefault();

                    var secondItem = ids.Skip(1).FirstOrDefault();
                    if (GetPriceItemId(priceItems, (double)request.DistancePrice) == priceItems.FirstOrDefault().Id)
                    {

                        price1 = priceItems
                       .Where(b => b.MinDistance <= (double)request.DistancePrice && b.MaxDistance >= (double)request.DistancePrice)
                       .Select(b => (decimal)b.Price)
                       .FirstOrDefault();
                        max1 = priceItems
                                .Where(b => b.MinDistance <= (double)request.DistancePrice && b.MaxDistance >= (double)request.DistancePrice && firstId == priceItems.FirstOrDefault().Id)
                                .Select(b => (decimal)b.MaxDistance)
                                .FirstOrDefault();
                        maxAmount1 = priceItems
                                .Where(b => b.MaxDistance < (double)request.DistancePrice)
                                .Select(b => (decimal)b.MaxAmount)
                                .FirstOrDefault();
                        min1 = priceItems
                                .Where(b => b.MinDistance <= (double)request.DistancePrice && b.MaxDistance >= (double)request.DistancePrice && firstId == priceItems.FirstOrDefault().Id)
                                .Select(b => (decimal)b.MinDistance)
                                .FirstOrDefault();
                        minAmount1 = priceItems
                                .Where(b => b.MinDistance <= (double)request.DistancePrice && b.MaxDistance >= (double)request.DistancePrice && firstId == priceItems.FirstOrDefault().Id)
                                .Select(b => (decimal)b.MinAmount)
                                .FirstOrDefault();
                        if (request.DistancePrice >= min1 && request.DistancePrice <= max1)
                        {
                            distancePrice = priceItems
                                .Where(b => b.MinDistance <= (double)request.DistancePrice && b.MaxDistance >= (double)request.DistancePrice)
                                .Select(b => b.Price)
                                .FirstOrDefault();
                            distancePriceMax1 = priceItems
                                .Where(b => b.MinDistance <= (double)request.DistancePrice && b.MaxDistance >= (double)request.DistancePrice)
                                .Select(b => (decimal)b.MaxDistance)
                                .FirstOrDefault();
                            if (request.DistancePrice <= distancePriceMax1 && request.DistancePrice >= 1)
                            {
                                distancePrice = request.DistancePrice * distancePrice;


                            }
                            else if (request.DistancePrice < 1)
                            {
                                distancePrice = minAmount1;
                            }
                            else if (request.DistancePrice > max1)
                            {
                                distancePrice = maxAmount1;

                            }

                        }

                    }

                    else
                    {
                        max2 = priceItems
                       .Where(b => b.MinDistance <= (double)request.DistancePrice && b.MaxDistance >= (double)request.DistancePrice && secondItem == priceItems.Skip(1).FirstOrDefault().Id)
                       .Select(b => (decimal)b.MaxDistance)
                       .FirstOrDefault();
                        maxAmount2 = priceItems
                               .Where(b => b.MaxDistance < (double)request.DistancePrice)
                               .Select(b => (decimal)b.MaxAmount)
                               .FirstOrDefault();
                        min2 = priceItems
                                .Where(b => b.MinDistance <= (double)request.DistancePrice && b.MaxDistance >= (double)request.DistancePrice && secondItem == priceItems.Skip(1).FirstOrDefault().Id)
                                .Select(b => (decimal)b.MinDistance)
                                .FirstOrDefault();
                        minAmount2 = priceItems
                                .Where(b => b.MinDistance <= (double)request.DistancePrice && b.MaxDistance >= (double)request.DistancePrice && secondItem == priceItems.Skip(1).FirstOrDefault().Id)
                                .Select(b => (decimal)b.MinAmount)
                                .FirstOrDefault();

                        decimal max = (decimal)priceItems.FirstOrDefault().MaxDistance;
                        decimal price = (decimal)priceItems.FirstOrDefault().Price;
                        if (request.DistancePrice >= min2 && request.DistancePrice <= max2)
                        {
                            var distancePrice2 = priceItems
                                .Where(b => b.MinDistance >= (double)min2 && b.MaxDistance <= (double)max2)
                                .Select(b => b.Price)
                                .FirstOrDefault();
                            distancePrice = max * price + (request.DistancePrice - max) * distancePrice2;
                        }
                        else if (request.DistancePrice > max2 && request.DistancePrice <= min2)
                        {
                            distancePrice = minAmount2;
                        }
                        else if (request.DistancePrice > max2)
                        {
                            distancePrice = maxAmount2;
                        }

                    }
                }


            }
            #endregion
            if (order == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy gói hàng", id.ToString());
            }
            var currentName = customerName;
            var currentPhone = customerPhone;
            var currentEmail = customerEmail;
            var currentCity = customerCity;
            var currentCommune = customerDistric;
            var currentDistric = customerDistric;
            var currentOther = other;
            var storeId1 = storeId;
            order.StoreId = storeId1;
            order.ShipperId = request.ShipperId;
            order.Capacity = request.Capacity;
            order.PackageWeight = request.PackageWeight;
            order.PackageWidth = request.PackageWidth;
            order.PackageHeight = request.PackageHeight;
            order.PackageLength = request.PackageLength;
            order.CustomerPhone = request.CustomerPhone ?? currentPhone;
            order.CustomerName = request.CustomerName ?? currentName;
            order.CustomerEmail = request.CustomerEmail ?? currentEmail;
            order.CustomerCity = request.CustomerCity ?? currentCity;
            order.CustomerCommune = request.CustomerCommune ?? currentCommune;
            order.CustomerDistrict = request.CustomerDistrict ?? currentDistric;
            order.Cod = request.Cod;
            order.Distance = request.Distance;
            order.DistancePrice = distancePrice;
            order.SubtotalPrice = request.SubtotalPrice;
            order.Other = request.Other ?? currentOther;
            order.TotalPrice = distancePrice + request.SubtotalPrice + request.Cod;
            order.ActionId = request.ActionId;
            order.TypeId = request.TypeId;
            order.CreateTime = DateTime.Now;
            order.OrderTime = request.OrderTime;


            await _unitOfWork.Repository<Order>().Update(order, id);
            await _unitOfWork.CommitAsync();

            var updatedPackageResponse = _mapper.Map<OrderResponse>(order);

            //var updatedPackageResponse = new 
            return updatedPackageResponse;

        }
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

        //SHIPPER
        //Suggest Order
        public async Task<List<OrderResponse>> GetOrderSuggest(int id, SuggestEnum suggest, int money)
        {

            var order = await _unitOfWork.Repository<Order>().GetAll()
             .Include(o => o.Store)
             .Include(o => o.Shipper)
             .Include(o => o.Action)
             .Include(o => o.Type)
             .Include(o => o.Route)
             .FirstOrDefaultAsync(r => r.Id == id);

            var orderSuggest = _unitOfWork.Repository<Order>().GetAll()
             .Include(o => o.Store)
             .Include(o => o.Shipper)
             .Include(o => o.Action)
             .Include(o => o.Type)
             .Include(o => o.Route)
             .Where(o => o.ShipperId == order.ShipperId && o.PickupTime == null)
            ;


            if (suggest == SuggestEnum.DISTRICT && (money == null || money == 0))
            {
                orderSuggest.Where(a => a.CustomerDistrict.Equals(order.CustomerDistrict)).ToListAsync();
            }
            if (suggest == SuggestEnum.ACTION && (money == null || money == 0))
            {
                orderSuggest.Where(a => a.ActionId == order.ActionId).ToListAsync();
            }
            if (suggest == SuggestEnum.TYPE && (money == null || money == 0))
            {
                orderSuggest.Where(a => a.TypeId == order.TypeId).ToListAsync();
            }
            if (suggest == SuggestEnum.CAPACITY && (money == null || money == 0))
            {
                orderSuggest.Where(a => a.Capacity <= 5).ToListAsync();
            }           
            if (suggest == SuggestEnum.COD)
            {
                orderSuggest.Where(a => a.Capacity <= money).ToListAsync();
            }

            var orderResponse = _mapper.Map<List<OrderResponse>>(orderSuggest);
            return orderResponse;
        }

        




    }



}
