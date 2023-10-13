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
            .Where(a => string.IsNullOrWhiteSpace(fromStation) || a.FromStation.Contains(fromStation.Trim()))
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
        //Suggest Route
        public async Task<List<RouteEdgeResponse>> GetRouteSuggest(int id)
        {

            var route = _unitOfWork.Repository<RouteEdge>().GetAll()
            .Include(r => r.Shipper)
            .FirstOrDefaultAsync(r => r.Id == id);
            
            

            var routeResponse = _mapper.Map<List<RouteEdgeResponse>>(routeList);
            return routeResponse;
        }


    }


}
