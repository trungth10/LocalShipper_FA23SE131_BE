using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Interface
{
    public interface IPriceItemService
    {
        Task<List<PriceItemResponse>> GetPriceItem(int? id, decimal? minAmount,
            decimal? maxAmount, decimal? price, int? pageNumber, int? pageSize);

        Task<int> GetTotalPriceItemCount();
        Task<PriceItemResponse> CreatePriceItem(RegisterPriceItemRequest request);
        Task<PriceItemResponse> UpdatePriceItem(int id, PutPriceItemRequest priceItemRequest);
        Task<MessageResponse> DeletePriceItem(int id);
    }
}
