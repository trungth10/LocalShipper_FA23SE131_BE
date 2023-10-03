using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Interface
{
    public interface IZoneService
    {
        Task<List<ZoneResponse>> GetZones(int? id, string? zoneName, decimal? latitude, decimal? longtitude, decimal? radius, int? pageNumber, int? pageSize);
        Task<int> GetTotalZoneCount();
        Task<ZoneResponse> CreateZone(ZoneRequest request);
        Task<ZoneResponse> UpdateZone(int id, ZoneRequest request);
        Task<ZoneResponse> DeleteZone(int id);
        
    }
}
