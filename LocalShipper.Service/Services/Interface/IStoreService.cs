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
        Task<List<StoreResponse>> GetStore(int? id, string? storeName, int? status, int? brandId, int? zoneId, int? walletId, int? accountId);
        Task<StoreResponse> CreateStore(StoreRequest request);
        Task<StoreResponse> UpdateStore(int id, StoreRequest storeRequest);
        Task<MessageResponse> DeleteStore(int id);
    }
}
