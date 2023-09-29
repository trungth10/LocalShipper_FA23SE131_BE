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
        Task<List<PackageActionResponse>> GetPackageAction(int? id, string? actionType);

    }
}
