using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Interface
{
    public interface IShipperService
    {


        Task<List<ShipperResponse>> GetShipper(int? id, string? firstName, string? email, string? phone, int? zoneId, int? status);
        //Task<List<ShipperResponse>> GetListShipper(int? zoneId);
        //Task<List<ShipperResponse>> GetAll();
        Task<ShipperResponse> UpdateShipperStatus(int shipperId, UpdateShipperStatusRequest request);
        Task<ShipperResponse> RegisterShipperInformation(ShipperInformationRequest request);
        Task<int> GetTotalShipperCount();
        Task<ShipperResponse> UpdateShipper(int id, PutShipperRequest shipperRequest);
        Task<MessageResponse> DeleteShipper(int id);
    }
}
