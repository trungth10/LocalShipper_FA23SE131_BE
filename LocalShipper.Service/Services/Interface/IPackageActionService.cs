using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Interface
{
    public interface IPackageActionService
    {
        Task<List<PackageActionResponse>> GetPackageAction(int? id, string? actionType, int? pageNumber, int? pageSize);
        Task<PackageActionResponse> CreatePackageAction(PackageActionRequest request);
        Task<PackageActionResponse> UpdatePackageAction(int id, PackageActionRequest packageActionRequest);
        Task<MessageResponse> DeletePackageAction(int id);
        Task<int> GetTotalPackageActionCount();

    }
}
