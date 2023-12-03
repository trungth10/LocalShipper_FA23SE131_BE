using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Interface
{
    public interface IStoreService
    {
        Task<List<StoreResponse>> GetStore(int? id, string? storeName, int? status, int? zoneId, int? walletId, int? accountId, int? pageNumber, int? pageSize);
        Task<StoreResponse> CreateStore(StoreRequest request);
        Task<StoreResponse> UpdateStore(int id, StoreRequestPut storeRequest);
        Task<MessageResponse> DeleteStore(int id);
        Task<int> GetTotalStoreCount();
        Task<StoreResponse> SetTimeDelivery(int id, StoreRequestTime storeRequest);
    }
}
