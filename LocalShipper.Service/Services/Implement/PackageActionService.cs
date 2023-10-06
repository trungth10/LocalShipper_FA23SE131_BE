using AutoMapper;
using LocalShipper.Data.Models;
using LocalShipper.Data.UnitOfWork;
using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Exceptions;
using LocalShipper.Service.Helpers;
using LocalShipper.Service.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Implement
{
    public class PackageActionService : IPackageActionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public PackageActionService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<PackageActionResponse>> GetPackageAction(int? id, string? actionType, int? pageNumber, int? pageSize)
        {

            var packageAction = _unitOfWork.Repository<PackageAction>().GetAll()
                                                              .Where(b => id == 0 || b.Id == id)
                                                              .Where(b => string.IsNullOrWhiteSpace(actionType) || b.ActionType.Contains(actionType.Trim()));
            // Xác định giá trị cuối cùng của pageNumber
            pageNumber = pageNumber.HasValue ? Math.Max(1, pageNumber.Value) : 1;
            // Áp dụng phân trang nếu có thông số pageNumber và pageSize
            if (pageNumber.HasValue && pageSize.HasValue)
            {
                packageAction = packageAction.Skip((pageNumber.Value - 1) * pageSize.Value)
                                       .Take(pageSize.Value);
            }

            var packageActionList = await packageAction.ToListAsync();
            if (packageActionList == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Package không có hoặc không tồn tại", id.ToString());
            }
            var packageActionResponses = _mapper.Map<List<PackageAction>, List<PackageActionResponse>>(packageActionList);
            return packageActionResponses;
        }

        public async Task<PackageActionResponse> CreatePackageAction(PackageActionRequest request)
        {
            var existingActionType = await _unitOfWork.Repository<PackageAction>().FindAsync(b => b.ActionType == request.ActionType);
            if (existingActionType != null)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "ActionType đã tồn tại", request.ActionType);
            }


            var newPackageAction = new PackageAction
            {
               ActionType= request.ActionType.Trim(),
               CreatedAt= request.CreatedAt,
               Deleted =request.Deleted,
            };

            await _unitOfWork.Repository<PackageAction>().InsertAsync(newPackageAction);
            await _unitOfWork.CommitAsync();

           
            var PackageActionResponses = _mapper.Map<PackageActionResponse>(newPackageAction);
            return PackageActionResponses;
        }

        public async Task<PackageActionResponse> UpdatePackageAction(int id, PackageActionRequest packageActionRequest)
        {
            var packageAction = await _unitOfWork.Repository<PackageAction>()
                .GetAll()
                .FirstOrDefaultAsync(pa => pa.Id == id);

            if (packageAction == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy PackageAction", id.ToString());
            }

            packageAction.ActionType = packageActionRequest.ActionType.Trim();
           packageAction.CreatedAt = DateTime.Now;
         

            await _unitOfWork.Repository<PackageAction>().Update(packageAction, id);
            await _unitOfWork.CommitAsync();

            var updatedPackageActionResponse = new PackageActionResponse
            {
                Id = packageAction.Id,
                ActionType = packageAction.ActionType,
              CreatedAt = DateTime.Now,
             
            };

            return updatedPackageActionResponse;
        }

        public async Task<MessageResponse> DeletePackageAction(int id)
        {
            var packageAction = await _unitOfWork.Repository<PackageAction>().GetAll().FindAsync(id);

            if (packageAction == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy gói hàng", id.ToString());
            }

          
            packageAction.Deleted = true;

           
            await _unitOfWork.CommitAsync();

            return new MessageResponse
            {
                Message = "Đã xóa",
            };
        }
        public async Task<int> GetTotalPackageActionCount()
        {
            var count = await _unitOfWork.Repository<PackageAction>()
                .GetAll()
                .CountAsync();

            return count;
        }

    }
}
