using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Interface
{

    public interface IPriceInZoneService
    {
        Task<List<PriceInZoneResponse>> GetPriceInZone(int? id, int? priceId, int? zoneId, int? pageNumber, int? pageSize);

        Task<int> GetTotalPriceInZoneCount();
        Task<PriceInZoneResponse> CreatePriceInZone(RegisterPriceInZoneRequest request);
        Task<PriceInZoneResponse> UpdatePriceInZone(int id, PutPriceInZoneRequest priceInZoneRequest);
        Task<MessageResponse> DeletePriceInZone(int id);
    }

}
