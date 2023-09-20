using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Interface
{
    public interface IShipperService
    {
        Task<PagedResults<ShipperBatchResponse>> GetBatchsOfShipper(int shipperId, BatchRequest request);
        Task<PagedResults<ShipperResponse>> GetShipper(ShipperPagingRequest request, int brandId);
        Task<ShipperResponse> PostShipper(ShipperRequest model, FileStream file, string fileName);
        Task<ShipperResponse> PutShipper(int id, ShipperRequest model, FileStream file, string fileName);
        Task<ShipperResponse> UpdateShipperProfile(int id, UpdateShipperRequest model);
        Task<ShipperResponse> DeleteShipper(int id);
        Task<ShipperResponse> GetShipperByID(int id);
        Task<ShipperResponseJwt> SignInUserNamePass(LoginRequest request);
        Task<ShipperResponse> UpdateShipperStatus(int id, int status);
        Task<bool> UpdateShipperFCMToken(int shipperId, string FCMToken);
        Task<bool> Logout(int shipperId);
    }
}
