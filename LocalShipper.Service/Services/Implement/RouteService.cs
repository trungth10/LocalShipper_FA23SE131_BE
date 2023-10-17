using AutoMapper;
using LocalShipper.Data.Models;
using LocalShipper.Data.UnitOfWork;
using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Exceptions;
using LocalShipper.Service.Helpers;
using LocalShipper.Service.Services.Interface;
using MailKit.Search;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Implement
{
    public class RouteService : IRouteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public RouteService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        //GET  
        public async Task<List<RouteEdgeResponse>> GetRoute(int? id, string? fromStation, string? toStation, int? quantity, int? progress, int? priority, int? status, int? shipperId, int? pageNumber, int? pageSize)
        {

            var route = _unitOfWork.Repository<RouteEdge>().GetAll()
            .Include(r => r.Shipper)
            .Where(a => (id == null || id == 0) || a.Id == id)
            .Where(a => string.IsNullOrWhiteSpace(fromStation) || a.StoreId.Contains(fromStation.Trim()))
            .Where(a => string.IsNullOrWhiteSpace(toStation) || a.ToStation.Contains(toStation.Trim()))
            .Where(a => (quantity == null || quantity == 0) || a.Quantity == quantity)
            .Where(a => (progress == null || progress == 0) || a.Progress == progress)
            .Where(a => (priority == null || priority == 0) || a.Priority == priority)
            .Where(a => (status == null || status == 0) || a.Status == status)
            .Where(a => (shipperId == null || shipperId == 0) || a.ShipperId == shipperId);

            // Xác định giá trị cuối cùng của pageNumber
            pageNumber = pageNumber.HasValue ? Math.Max(1, pageNumber.Value) : 1;
            // Áp dụng phân trang nếu có thông số pageNumber và pageSize
            if (pageNumber.HasValue && pageSize.HasValue)
            {
                route = route.Skip((pageNumber.Value - 1) * pageSize.Value)
                                       .Take(pageSize.Value);
            }

            var routeList = await route.ToListAsync();
            if (routeList == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Lộ trình không có hoặc không tồn tại", id.ToString());
            }

            var routeResponse = _mapper.Map<List<RouteEdgeResponse>>(routeList);
            return routeResponse;
        }

        //SHIPPER
        //Add Order to Route
        public async Task<List<OrderResponse>> AddOrderToRoute(IEnumerable<int> id, int shipperId)
        {

            var orders = await _unitOfWork.Repository<Order>()
                                         .GetAll()
                                         .Include(o => o.Store)
                                         .Include(o => o.Shipper)
                                         .Include(o => o.Action)
                                         .Include(o => o.Type)
                                         .Include(o => o.Route)
                                         .Where(o => id.Contains(o.Id))
                                         .ToListAsync();

            if (orders.Count == 0)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy đơn hàng", id.ToString());
            }

            RouteEdge route = new RouteEdge
            {
                Quantity = orders.Count,
                Status = (int)RouteEdgeStatusEnum.IDLE,
                ShipperId = shipperId
            };

            await _unitOfWork.Repository<RouteEdge>().InsertAsync(route);
            await _unitOfWork.CommitAsync();

            foreach (var order in orders)
            {
                order.RouteId = route.Id;
                await _unitOfWork.Repository<Order>().Update(order, order.Id);
                await _unitOfWork.CommitAsync();
            }

            var orderResponse = _mapper.Map<List<OrderResponse>>(orders);
            return orderResponse;
        }
        public async Task<RouteEdgeResponse> CreateRoute(CreateRouteRequest request)
        { 
            var storeNames = await _unitOfWork.Repository<Store>().GetAll().Select(s => s.Id).ToListAsync();


            if (!storeNames.Contains(request.StoreId))
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Tên cửa hàng không hợp lệ", request.StoreId.ToString());
            }

            var existingName = await _unitOfWork.Repository<RouteEdge>().FindAsync(b => b.Name == request.Name);
            if (existingName != null)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Name đã tồn tại", request.Name);
            }

            var route = new RouteEdge
            {
                Name = request.Name.Trim(),
                StoreId = request.StoreId.ToString(), 
                ToStation = request.ToStation.Trim(),
                CreatedDate = request.CreatedDate,
                StartDate = request.StartDate,
                Eta = request.Eta,
                Quantity = request.Quantity,
                Progress = request.Progress,
                Priority = request.Priority,
                Status = request.Status,
                ShipperId = request.ShipperId
            };

            await _unitOfWork.Repository<RouteEdge>().InsertAsync(route);
            await _unitOfWork.CommitAsync();

            var routeResponse = _mapper.Map<RouteEdgeResponse>(route);
            return routeResponse;
        }

        public async Task<RouteEdgeResponse> UpdateRoute(int routeId, RouteRequest request)
        {

            var route = await _unitOfWork.Repository<RouteEdge>().GetAll()
                .FirstOrDefaultAsync(r => r.Id == routeId);

            if (route == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Route không tồn tại", routeId.ToString());
            }
            route.StoreId = request.StoreId.Trim();
            route.ToStation = request.ToStation.Trim();
            route.CreatedDate = request.CreatedDate;
            route.StartDate = request.StartDate;
            route.Eta = request.Eta;
            route.Quantity= request.Quantity;
            route.Progress= request.Progress;
            route.Priority= request.Priority;
            route.Status= request.Status;
            route.ShipperId= request.ShipperId;
                

            await _unitOfWork.Repository<RouteEdge>().Update(route, route.Id);
            await _unitOfWork.CommitAsync();

            var routeResponse = _mapper.Map<RouteEdgeResponse>(route);
            return routeResponse;
        }

        public async Task<MessageResponse> DeleteRoute(int routeId)
        {          
            var route = await _unitOfWork.Repository<RouteEdge>().GetAll()
                .FirstOrDefaultAsync(r => r.Id == routeId);

            if (route == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Route không tồn tại", routeId.ToString());
            }
            var ordersInRoute = await _unitOfWork.Repository<Order>().GetAll()
                .Where(o => o.RouteId == routeId)
                .ToListAsync();

            
            foreach (var order in ordersInRoute)
            {
                order.RouteId = null;
                await _unitOfWork.Repository<Order>().Update(order, order.Id);
            }

            _unitOfWork.Repository<RouteEdge>().Delete(route);
            await _unitOfWork.CommitAsync();

            return new  MessageResponse
            {
                Message = "Đã xóa",
            }; 
        }
    }
}
