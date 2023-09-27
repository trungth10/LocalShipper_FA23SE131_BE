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

        Task<ShipperResponse> UpdateShipperStatus(int shipperId, UpdateShipperStatusRequest request);
        Task<ShipperResponse> GetShipperById(int id);
        Task<List<ShipperResponse>> GetAll();
    }
}
