using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Interface
{
    public interface IPackageService
    {

        Task<List<PackageResponse>> GetPackage(int? batchId, int? id, int? status, 
            int? actionId, int? typeId, int? storeId, string? customerName,
            string? customerAddress, string? customerPhome, string? custommerEmail, decimal? totalPrice, int? pageNumber, int? pageSize);
        Task<PackageResponse> CreatePackage(PackageRequestForCreate request);
        Task<PackageResponse> UpdatePackage(int id, PackageRequest packageRequest);

        Task<MessageResponse> DeletePackage(int id);
        Task<int> GetTotalPackageCount();
        Task<PackageResponse> UpdateStatusPackage(int id, PackageStatusEnum status);
    }
}
