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
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
        //Add Order to Route
        public async Task<List<OrderResponse>> AddOrderToRoute(IEnumerable<int> id, int shipperId, int routeId)
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



            foreach (var order in orders)
            {
                order.RouteId = routeId;
                await _unitOfWork.Repository<Order>().Update(order, order.Id);
                await _unitOfWork.CommitAsync();
            }

            var route = await _unitOfWork.Repository<RouteEdge>().GetAll()
                .FirstOrDefaultAsync(r => r.Id == routeId);
            var countOrder = await _unitOfWork.Repository<Order>()
                    .GetAll()
                    .Where(a => a.RouteId == routeId)
                    .CountAsync();

            route.Quantity = countOrder;
            await _unitOfWork.Repository<RouteEdge>().Update(route, route.Id);
            await _unitOfWork.CommitAsync();


            var orderResponse = _mapper.Map<List<OrderResponse>>(orders);


            return orderResponse;
        }
        public async Task<RouteEdgeResponse> CreateRoute(CreateRouteRequest request)
        {
            var storeNames = await _unitOfWork.Repository<Store>().GetAll().Select(s => s.Id).ToListAsync();


            var route = new RouteEdge
            {
                Name = request.Name.Trim(),
                StartDate = request.StartDate,
                Status = (int)RouteEdgeStatusEnum.IDLE,
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
            route.FromStation = request.FromStation.Trim();
            route.ToStation = request.ToStation.Trim();
            route.CreatedDate = request.CreatedDate;
            route.StartDate = request.StartDate;
            route.Eta = request.Eta;
            route.Quantity = request.Quantity;
            route.Progress = request.Progress;
            route.Priority = request.Priority;
            route.Status = request.Status;
            route.ShipperId = request.ShipperId;


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

            return new MessageResponse
            {
                Message = "Đã xóa",
            };
        }

        //SHIPPER
        //Suggest Order
        public async Task<List<OrderResponse>> CreateRouteSuggest(int shiperId, int money, SuggestEnum suggest, int capacityLow, int capacityHight, CreateRouteRequestAuto request)
        {

            var orderSuggest = _unitOfWork.Repository<Order>().GetAll()
             .Include(o => o.Store)
             .Include(o => o.Shipper)
             .Include(o => o.Action)
             .Include(o => o.Type)
             .Include(o => o.Route)
             .Where(o => o.ShipperId == shiperId && o.PickupTime == null && o.RouteId == null)
            ;


            if (suggest == SuggestEnum.DISTRICT && (money == null || money == 0) && (capacityLow == null || capacityLow == 0) && (capacityHight == null || capacityHight == 0))
            {
                orderSuggest.GroupBy(o => o.CustomerDistrict)
                            .Where(group => group.Count() > 1)
                            .SelectMany(group => group)
                            .ToListAsync();
            }
            if (suggest == SuggestEnum.ACTION && (money == null || money == 0) && (capacityLow == null || capacityLow == 0) && (capacityHight == null || capacityHight == 0))
            {
                orderSuggest.GroupBy(o => o.ActionId)
                            .Where(group => group.Count() > 1)
                            .SelectMany(group => group)
                            .ToListAsync();
            }
            if (suggest == SuggestEnum.TYPE && (money == null || money == 0) && (capacityLow == null || capacityLow == 0) && (capacityHight == null || capacityHight == 0))
            {
                orderSuggest.GroupBy(o => o.TypeId)
                            .Where(group => group.Count() > 1)
                            .SelectMany(group => group)
                            .ToListAsync();
            }          
            if (suggest == SuggestEnum.CAPACITY && (money == null || money == 0) )
            {
                orderSuggest.Where(a => capacityLow <= a.Capacity && a.Capacity <= capacityHight).ToListAsync();
            }
            if (suggest == SuggestEnum.COD && (capacityLow == null || capacityLow == 0) && (capacityHight == null || capacityHight == 0))
            {
                orderSuggest.Where(a => a.Cod <= money).ToListAsync();
            }

            Random random = new Random();
            int randomNumber = random.Next(10, 99); 

            string randomLetters = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ", 3).Select(s => s[random.Next(s.Length)]).ToArray());
      
            string code = randomLetters + randomNumber.ToString();

            var countOrder = await orderSuggest
                   .CountAsync();
          
            var route = new RouteEdge
            {
                Name = "Lộ trình gợi ý #" + code,
                StartDate = request.StartDate,
                Quantity = countOrder,
                Status = (int)RouteEdgeStatusEnum.IDLE,
                ShipperId = shiperId,
            };

            await _unitOfWork.Repository<RouteEdge>().InsertAsync(route);
            await _unitOfWork.CommitAsync();

            foreach (var order in orderSuggest)
            {
                order.RouteId = route.Id;
                await _unitOfWork.Repository<Order>().Update(order, order.Id);
               
            }
            await _unitOfWork.CommitAsync();

            var orderResponse = _mapper.Map<List<OrderResponse>>(orderSuggest);
            return orderResponse;
        }

        public async Task<List<OrderResponse>> UpdateOrderRouteId(IEnumerable<int> orderid)
        {
            var orders = await _unitOfWork.Repository<Order>()
                .GetAll()
                .Where(o => orderid.Contains(o.Id))
                .ToListAsync();

            foreach (var order in orders)
            {
                order.RouteId = null; 
                await _unitOfWork.Repository<Order>().Update(order, order.Id);
            }

            await _unitOfWork.CommitAsync();

            var orderResponse = _mapper.Map<List<OrderResponse>>(orders);
            return orderResponse;
        }


        public async Task<GeocodingResponse> ConvertAddress(string address)
        {
            string apiKey = "AIzaSyBBHXLtEw2nMmiMq7dBZHKytUwhzezi7UU";
            string geocodingApiUrl = "https://maps.googleapis.com/maps/api/geocode/json";

            using (var httpClient = new HttpClient())
            {
                var requestUri = $"{geocodingApiUrl}?address={Uri.EscapeDataString(address)}&key={apiKey}";
                var response = await httpClient.GetAsync(requestUri);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();                   
                    var result = Newtonsoft.Json.JsonConvert.DeserializeObject<GeocodingResponse>(content);
                    return result;
                }
                else
                {
                    throw new Exception($"Failed to retrieve geocoding data. Status code: {response.StatusCode}");
                }
            }
        }

    }
}
