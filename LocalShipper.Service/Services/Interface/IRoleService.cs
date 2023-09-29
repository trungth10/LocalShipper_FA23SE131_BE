using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Interface
{
    public interface IRoleService
    {
        Task<List<RoleResponse>> GetRole(int? id, string? name);

        Task<int> GetTotalRoleCount();
        Task<RoleResponse> CreateRole(RegisterRoleRequest request);
        Task<RoleResponse> UpdateRole(int id, PutRoleRequest roleRequest);
        Task<MessageResponse> DeleteRole(int id);
    }
}
