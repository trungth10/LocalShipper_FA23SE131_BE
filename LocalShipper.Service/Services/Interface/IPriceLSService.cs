using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Interface
{
    public interface IPriceLSService
    {
        Task<List<PriceLSResponse>> GetPrice(int? id, string? name, int? storeId, int? hourFilter,
            int? dateFilter, int? mode, int? status, int? priority, int? pageNumber, int? pageSize);
        Task<int> GetTotalPriceCount();
        Task<PriceLSResponse> CreatePrice(RegisterPriceRequest request, int accountId);
        Task<PriceLSResponse> UpdatePrice(int id, PutPriceRequest priceRequest, int accountId);
        Task<MessageResponse> DeletePrice(int id);
    }
}
