using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Interface
{
    public interface IPackageService
    {
        Task<List<PackageResponse>> GetPackage(int? batchId, int? id, int? status, int? actionId, int? typeId, string? customerName, string? customerAddress, string? customerPhome, string? custommerEmail, decimal? totalPrice);
        Task<PackageResponse> CreatePackage(PackageRequest request);
        Task<PackageResponse> UpdatePackage(int id, PackageRequest packageRequest);

        Task<MessageResponse> DeletePackage(int id);
        Task<int> GetTotalPackageCount();
    }
}
