using AutoMapper;
using LocalShipper.Data.Models;
using LocalShipper.Data.UnitOfWork;
using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Exceptions;
using LocalShipper.Service.Services.Interface;
using MailKit.Search;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Implement
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public RoleService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;

        }

        //CREATE Role
        public async Task<RoleResponse> CreateRole(RegisterRoleRequest request)
        {
            var roleExisted = _unitOfWork.Repository<Role>().Find(x => x.Name == request.Name);

            if (roleExisted != null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Role không tồn tại", request.Name.ToString());
            }
            Role role = new Role
            {
                Name = request.Name,
            };
            await _unitOfWork.Repository<Role>().InsertAsync(role);
            await _unitOfWork.CommitAsync();


            var createdRoleResponse = new RoleResponse
            {
                Id = role.Id,
                Name = role.Name,
            };
            return createdRoleResponse;
            
        }

        //GET 
        public async Task<List<RoleResponse>> GetRole(int? id, string? name)
        {

            var roles = await _unitOfWork.Repository<Role>().GetAll()
            .Where(t => !id.HasValue || t.Id == id)
            .Where(r => string.IsNullOrWhiteSpace(name) || r.Name.Contains(name))
            .ToListAsync();
            var roleReponses = roles.Select(role => new RoleResponse
            {
                Id = role.Id,
                Name = role.Name
            }).ToList();
            return roleReponses;
        }


        //GET Count
        public async Task<int> GetTotalRoleCount()
        {
            var count = await _unitOfWork.Repository<Role>()
                .GetAll()
                .CountAsync();

            return count;
        }

        //UPDATE Role
        public async Task<RoleResponse> UpdateRole(int id, PutRoleRequest roleRequest)
        {
            var role = await _unitOfWork.Repository<Role>()
                .GetAll()
                .FirstOrDefaultAsync(a => a.Id == id);

            if (role == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy role", id.ToString());
            }

            role.Name = roleRequest.Name;



            await _unitOfWork.Repository<Role>().Update(role, id);
            await _unitOfWork.CommitAsync();

            var updatedRoleResponse = new RoleResponse
            {
                Id = role.Id,
                Name = role.Name,
            };

            return updatedRoleResponse;
        }

        //DELETE Role
        //public async Task<MessageResponse> DeleteRole(int id)
        //{

        //    var role = await _unitOfWork.Repository<Role>().GetAll()
        //    .FirstOrDefaultAsync(a => a.Id == id);

        //    if (role == null)
        //    {
        //        throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy role", id.ToString());
        //    }

        //    _unitOfWork.Repository<Role>().Delete(role);
        //    await _unitOfWork.CommitAsync();

        //    return new MessageResponse
        //    {
        //        Message = "Xóa role thành công",
        //    };
        //}
    }
}



