using AutoMapper;
using Google.OrTools.ConstraintSolver;
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
using Org.BouncyCastle.Utilities.Net;
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
            route.FromStation = request.FromStation;
            route.ToStation = request.ToStation;
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
        public async Task<RouteEdgeResponse> CreateRouteSuggest(int shiperId, int money, SuggestEnum suggest, int capacityLow, int capacityHight, CreateRouteRequestAuto request,double shipperLatitude, double shipperLongitude)
        {

            var orderSuggest = _unitOfWork.Repository<Order>().GetAll()
             .Include(o => o.Store)
             .Include(o => o.Shipper)
             .Include(o => o.Action)
             .Include(o => o.Type)
             .Include(o => o.Route)
             .Where(o => o.ShipperId == shiperId && o.PickupTime == null && o.RouteId == null)
            ;

            IEnumerable<Order> filteredOrders = null;
            IEnumerable<Order> _filteredOrders = null;
            IEnumerable<Order> largestGroup = null;

            if (suggest == SuggestEnum.DISTRICT && (money == null || money == 0) && (capacityLow == null || capacityLow == 0) && (capacityHight == null || capacityHight == 0))
            {
                var groupedOrders = orderSuggest.AsEnumerable()
                                                .GroupBy(o => o.CustomerDistrict)
                                                .Where(group => group.Count() > 1)
                                                .OrderByDescending(group => group.Count())
                                                .SelectMany(group => group);

                filteredOrders = groupedOrders.ToList();
            }
            if (suggest == SuggestEnum.ACTION && (money == null || money == 0) && (capacityLow == null || capacityLow == 0) && (capacityHight == null || capacityHight == 0))
            {
                var groupedOrders = orderSuggest.AsEnumerable()
                                                .GroupBy(o => o.ActionId)
                                                .Where(group => group.Count() > 1)
                                                .OrderByDescending(group => group.Count())
                                                .SelectMany(group => group);

                filteredOrders = groupedOrders.ToList();
            }
            if (suggest == SuggestEnum.TYPE && (money == null || money == 0) && (capacityLow == null || capacityLow == 0) && (capacityHight == null || capacityHight == 0))
            {
                var groupedOrders = orderSuggest.AsEnumerable()
                                                 .GroupBy(o => o.TypeId)
                                                 .Where(group => group.Count() > 1)
                                                 .OrderByDescending(group => group.Count())
                                                 .SelectMany(group => group);

                filteredOrders = groupedOrders.ToList();
            }
            if (suggest == SuggestEnum.CAPACITY && (money == null || money == 0))
            {
                _filteredOrders = orderSuggest.Where(a => capacityLow <= a.Capacity && a.Capacity <= capacityHight).ToList();
            }
            if (suggest == SuggestEnum.COD && (capacityLow == null || capacityLow == 0) && (capacityHight == null || capacityHight == 0))
            {
                _filteredOrders = orderSuggest.Where(a => a.Cod <= money).ToList();

            }

            Random random = new Random();
            int randomNumber = random.Next(10, 99);

            string randomLetters = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ", 3).Select(s => s[random.Next(s.Length)]).ToArray());

            string code = randomLetters + randomNumber.ToString();








            if (suggest == SuggestEnum.COD || suggest == SuggestEnum.CAPACITY)
            {
                largestGroup = _filteredOrders;
            }
            if (suggest == SuggestEnum.DISTRICT)
            {
                largestGroup = filteredOrders.GroupBy(o => o.CustomerDistrict)
                                            .OrderByDescending(group => group.Count())
                                            .First();
            }
            if (suggest == SuggestEnum.ACTION)
            {
                largestGroup = filteredOrders.GroupBy(o => o.CustomerDistrict)
                                            .OrderByDescending(group => group.Count())
                                            .First();
            }
            if (suggest == SuggestEnum.TYPE)
            {
                largestGroup = filteredOrders.GroupBy(o => o.CustomerDistrict)
                                            .OrderByDescending(group => group.Count())
                                            .First();
            }


            List<(double Latitude, double Longitude)> fullLatLng = new List<(double, double)>();
            List<int[]> pickupsDeliveriesList = new List<int[]>();
            fullLatLng.Add((shipperLatitude, shipperLongitude));

            Dictionary<(double Latitude, double Longitude), int> locationIndexMap = new Dictionary<(double, double), int>
{
    { (shipperLatitude, shipperLongitude), 0 }
};

            foreach (var order in largestGroup)
            {
                string storeAddress = order.Store.StoreAddress;
                var storeCoordinates = await ConvertAddress(storeAddress);
                string customerAddress = $"{order.CustomerCommune}, {order.CustomerDistrict}, {order.CustomerCity}";
                var customerCoordinates = await ConvertAddress(customerAddress);

                if (!locationIndexMap.ContainsKey(storeCoordinates))
                {
                    locationIndexMap.Add(storeCoordinates, fullLatLng.Count);
                    fullLatLng.Add(storeCoordinates);
                }
                locationIndexMap.Add(customerCoordinates, fullLatLng.Count);
                fullLatLng.Add(customerCoordinates);
                int pickupIndex = locationIndexMap[storeCoordinates];
                int deliveryIndex = fullLatLng.Count - 1;
                pickupsDeliveriesList.Add(new int[] { pickupIndex, deliveryIndex });
            }

            var distanceMatrix = await GetDistanceMatrix(fullLatLng);

            int[][] pickupsDeliveriesArray = pickupsDeliveriesList.ToArray();



            (List<int> _route, List<(int, int)> pickupDeliveries) = await SolvePDPAsync(distanceMatrix, pickupsDeliveriesArray);

            List<int> pdpSolution = _route;
            List<string> sortedAddresses = new List<string>();
            List<string> sortedAddressesConvert = new List<string>();

            foreach (var index in pdpSolution)
            {
                if (index != 0)
                {
                    var coordinates = fullLatLng[index];

                    var address = locationIndexMap.FirstOrDefault(x => x.Value == index).Key;

                    sortedAddresses.Add($"{address.Latitude}, {address.Longitude}");

                    var formattedAddress = await ConvertLatLng(coordinates.Latitude, coordinates.Longitude);
                    sortedAddressesConvert.Add(formattedAddress);
                }

            }

            int count = largestGroup.Count();

            var route = new RouteEdge
            {
                Name = "Lộ trình gợi ý " + code,
                StartDate = request.StartDate,
                FromStation = sortedAddressesConvert.First(),
                ToStation = sortedAddressesConvert.Last(),
                Quantity = count,
                Status = (int)RouteEdgeStatusEnum.IDLE,
                ShipperId = shiperId,
            };

            await _unitOfWork.Repository<RouteEdge>().InsertAsync(route);
            await _unitOfWork.CommitAsync();



            foreach (var order in largestGroup)
            {
                order.RouteId = route.Id;
                await _unitOfWork.Repository<Order>().Update(order, order.Id);
            }
            await _unitOfWork.CommitAsync();

            /* var orderResponse = _mapper.Map<RouteEdgeResponse>(route);
             return orderResponse;*/
            return _mapper.Map<RouteEdge, RouteEdgeResponse>(route);
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


        public async Task<(double Latitude, double Longitude)> ConvertAddress(string address)
        {
            string apiKey = "Y3afHdEef5El4LnR3o4FjwSdMWpXIhKnA5hvHCrj";
            string geocodingApiUrl = "https://rsapi.goong.io/Geocode";

            using (var httpClient = new HttpClient())
            {
                var requestUri = $"{geocodingApiUrl}?address={Uri.EscapeDataString(address)}&api_key={apiKey}";
                var response = await httpClient.GetAsync(requestUri);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = Newtonsoft.Json.JsonConvert.DeserializeObject<GeocodingResponse>(content);
                    return (result.results[0].geometry.location.lat, result.results[0].geometry.location.lng);
                }
                else
                {
                    throw new Exception($"Failed to retrieve geocoding data. Status code: {response.StatusCode}");
                }
            }
        }

        public async Task<string> ConvertLatLng(double lat, double lng)
        {
            string apiKey = "Y3afHdEef5El4LnR3o4FjwSdMWpXIhKnA5hvHCrj";
            string geocodingApiUrl = "https://rsapi.goong.io/Geocode";

            using (var httpClient = new HttpClient())
            {
                var requestUri = $"{geocodingApiUrl}?latlng={lat},{lng}&api_key={apiKey}";
                var response = await httpClient.GetAsync(requestUri);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = Newtonsoft.Json.JsonConvert.DeserializeObject<GeocodingResponse>(content);
                    return (result.results[0].formatted_address);
                }
                else
                {
                    throw new Exception($"Failed to retrieve geocoding data. Status code: {response.StatusCode}");
                }
            }
        }


        //SHIPPER
        //Add Order to Route
        public async Task<RouteEdgeResponse> AddOrderToRoute(IEnumerable<int> id, int shipperId, int routeId, double shipperLatitude, double shipperLongitude)
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

            List<(double Latitude, double Longitude)> fullLatLng = new List<(double, double)>();
            List<int[]> pickupsDeliveriesList = new List<int[]>();
            fullLatLng.Add((shipperLatitude, shipperLongitude));

            Dictionary<(double Latitude, double Longitude), int> locationIndexMap = new Dictionary<(double, double), int>
{
    { (shipperLatitude, shipperLongitude), 0 }
};

            foreach (var order in orders)
            {
                string storeAddress = order.Store.StoreAddress;
                var storeCoordinates = await ConvertAddress(storeAddress);
                string customerAddress = $"{order.CustomerCommune}, {order.CustomerDistrict}, {order.CustomerCity}";
                var customerCoordinates = await ConvertAddress(customerAddress);

                if (!locationIndexMap.ContainsKey(storeCoordinates))
                {
                    locationIndexMap.Add(storeCoordinates, fullLatLng.Count);
                    fullLatLng.Add(storeCoordinates);
                }
                locationIndexMap.Add(customerCoordinates, fullLatLng.Count);
                fullLatLng.Add(customerCoordinates);
                int pickupIndex = locationIndexMap[storeCoordinates];
                int deliveryIndex = fullLatLng.Count - 1;
                pickupsDeliveriesList.Add(new int[] { pickupIndex, deliveryIndex });
            }

            var distanceMatrix = await GetDistanceMatrix(fullLatLng);

            int[][] pickupsDeliveriesArray = pickupsDeliveriesList.ToArray();



            (List<int> _route, List<(int, int)> pickupDeliveries) = await SolvePDPAsync(distanceMatrix, pickupsDeliveriesArray);

            List<int> pdpSolution = _route;
            List<string> sortedAddresses = new List<string>();
            List<string> sortedAddressesConvert = new List<string>();

            foreach (var index in pdpSolution)
            {
                if (index != 0)
                {
                    var coordinates = fullLatLng[index];

                    var address = locationIndexMap.FirstOrDefault(x => x.Value == index).Key;

                    sortedAddresses.Add($"{address.Latitude}, {address.Longitude}");

                    var formattedAddress = await ConvertLatLng(coordinates.Latitude, coordinates.Longitude);
                    sortedAddressesConvert.Add(formattedAddress);
                }

            }

            var route = await _unitOfWork.Repository<RouteEdge>().GetAll()
                .FirstOrDefaultAsync(r => r.Id == routeId);
            var countOrder = await _unitOfWork.Repository<Order>()
                    .GetAll()
                    .Where(a => a.RouteId == routeId)
                    .CountAsync();

            route.Quantity = countOrder;
            route.FromStation = sortedAddressesConvert.First();
            route.ToStation = sortedAddressesConvert.Last();
            await _unitOfWork.Repository<RouteEdge>().Update(route, route.Id);
            await _unitOfWork.CommitAsync();


            return  _mapper.Map<RouteEdge, RouteEdgeResponse>(route); 
        }

        public async Task<long[,]> GetDistanceMatrix(List<(double Latitude, double Longitude)> location)
        {
            string apiKey = "Y3afHdEef5El4LnR3o4FjwSdMWpXIhKnA5hvHCrj";
            string distanceMatrixApiUrl = "https://rsapi.goong.io/DistanceMatrix";

            using (var httpClient = new HttpClient())
            {
                var origins = string.Join("%7C", location.Select(location => $"{location.Latitude},{location.Longitude}"));
                var destinations = string.Join("%7C", location.Select(location => $"{location.Latitude},{location.Longitude}"));

                var requestUri = $"{distanceMatrixApiUrl}?origins={origins}&destinations={destinations}&vehicle=car&api_key={apiKey}";
                var response = await httpClient.GetAsync(requestUri);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = Newtonsoft.Json.JsonConvert.DeserializeObject<DistanceMatrixResponse>(content);

                    if (result != null && result.rows != null && result.rows.Count > 0)
                    {
                        int numLocations = location.Count;
                        long[,] distanceMatrix = new long[numLocations, numLocations];

                        for (int i = 0; i < numLocations; i++)
                        {
                            for (int j = 0; j < numLocations; j++)
                            {
                                distanceMatrix[i, j] = result.rows[i].elements[j].distance.value;
                            }
                        }

                        return distanceMatrix;
                    }
                    else
                    {
                        throw new Exception("Không thể lấy dữ liệu ma trận khoảng cách từ phản hồi.");
                    }
                }
                else
                {
                    throw new Exception($"Không thể lấy dữ liệu ma trận khoảng cách. Mã trạng thái: {response.StatusCode}");
                }
            }
        }


        public async Task<List<int>> SolveTSPAsync(long[,] distanceMatrix)
        {
              
            RoutingIndexManager manager = new RoutingIndexManager(distanceMatrix.GetLength(0), 1, 0);
            RoutingModel routing = new RoutingModel(manager);

            int transitCallbackIndex = routing.RegisterTransitCallback((long fromIndex, long toIndex) =>
            {
                var fromNode = manager.IndexToNode(fromIndex);
                var toNode = manager.IndexToNode(toIndex);
                return distanceMatrix[fromNode, toNode];
            });

            routing.SetArcCostEvaluatorOfAllVehicles(transitCallbackIndex);


            Assignment solution = routing.Solve();

            List<int> tspSolution = new List<int>();
            long index = routing.Start(0);
            while (!routing.IsEnd(index))
            {
                tspSolution.Add(manager.IndexToNode(index));
                index = solution.Value(routing.NextVar(index));
            }
            tspSolution.Add(manager.IndexToNode(index));

            return tspSolution;
        }

        public async Task<(List<int>, List<(int, int)>)> SolvePDPAsync(long[,] distanceMatrix, int[][] pickupsDeliveries)
        {
            RoutingIndexManager manager = new RoutingIndexManager(distanceMatrix.GetLength(0), 1, 0);
            RoutingModel routing = new RoutingModel(manager);

            int transitCallbackIndex = routing.RegisterTransitCallback((long fromIndex, long toIndex) =>
            {
                var fromNode = manager.IndexToNode(fromIndex);
                var toNode = manager.IndexToNode(toIndex);
                return distanceMatrix[fromNode, toNode];
            });

            routing.SetArcCostEvaluatorOfAllVehicles(transitCallbackIndex);         

            foreach (var pair in pickupsDeliveries)
            {
                long pickupNode = manager.NodeToIndex(pair[0]);
                long deliveryNode = manager.NodeToIndex(pair[1]);
                routing.AddPickupAndDelivery(pickupNode, deliveryNode);
            }

            Assignment solution = routing.Solve();

            List<int> route = new List<int>();
            long index = routing.Start(0);
            while (!routing.IsEnd(index))
            {
                route.Add(manager.IndexToNode(index));
                index = solution.Value(routing.NextVar(index));
            }
            route.Add(manager.IndexToNode(index));

            List<(int, int)> pickupDeliveriesResult = new List<(int, int)>();
            foreach (var pair in pickupsDeliveries)
            {
                long pickupNode = manager.NodeToIndex(pair[0]);
                long deliveryNode = manager.NodeToIndex(pair[1]);
                pickupDeliveriesResult.Add((route.IndexOf((int)pickupNode), route.IndexOf((int)deliveryNode)));
            }

            return (route, pickupDeliveriesResult);
        }

       /* public async Task<(List<int>, List<(int, int)>)> SolvePDPAsync(long[,] distanceMatrix, int[][] pickupsDeliveries)
        {
            RoutingIndexManager manager = new RoutingIndexManager(distanceMatrix.GetLength(0), 1, 0);
            RoutingModel routing = new RoutingModel(manager);

            int transitCallbackIndex = routing.RegisterTransitCallback((long fromIndex, long toIndex) =>
            {
                var fromNode = manager.IndexToNode(fromIndex);
                var toNode = manager.IndexToNode(toIndex);
                return distanceMatrix[fromNode, toNode];
            });

            routing.SetArcCostEvaluatorOfAllVehicles(transitCallbackIndex);

            foreach (var pair in pickupsDeliveries)
            {
                long pickupNode = manager.NodeToIndex(pair[0]);
                long deliveryNode = manager.NodeToIndex(pair[1]);
                routing.AddPickupAndDelivery(pickupNode, deliveryNode);
            }

            Assignment solution = routing.Solve();

            List<int> route = new List<int>();
            long index = routing.Start(0);
            while (!routing.IsEnd(index))
            {
                route.Add(manager.IndexToNode(index));
                index = solution.Value(routing.NextVar(index));
            }
            route.Add(manager.IndexToNode(index));

            List<(int, int)> pickupDeliveriesResult = new List<(int, int)>();

            Dictionary<int, List<int>> pickupDeliveryMap = new Dictionary<int, List<int>>();

            foreach (var pair in pickupsDeliveries)
            {
                int pickupPoint = pair[0];
                int deliveryPoint = pair[1];

                if (!pickupDeliveryMap.ContainsKey(pickupPoint))
                {
                    pickupDeliveryMap[pickupPoint] = new List<int>();
                }

                pickupDeliveryMap[pickupPoint].Add(deliveryPoint);
            }

            foreach (var pickupPoint in pickupDeliveryMap.Keys)
            {
                foreach (var deliveryPoint in pickupDeliveryMap[pickupPoint])
                {
                    pickupDeliveriesResult.Add((route.IndexOf(pickupPoint), route.IndexOf(deliveryPoint)));
                }
            }

            return (route, pickupDeliveriesResult);
        }*/



    }
}
