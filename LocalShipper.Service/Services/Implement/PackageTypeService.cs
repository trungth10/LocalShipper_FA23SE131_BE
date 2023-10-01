using AutoMapper;
using LocalShipper.Data.Models;
using LocalShipper.Data.UnitOfWork;
using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Exceptions;
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
    public class PackageTypeService : IPackageTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public PackageTypeService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<PackageTypeResponse>> GetPackageType(int? id, string? packageType)
        {

            var packageTypes = await _unitOfWork.Repository<PackageType>().GetAll()
                                                              .Where(b => id == 0 || b.Id == id)

                                                              .Where(b => string.IsNullOrWhiteSpace(packageType) || b.PackageType1.Contains(packageType))
                                                              .ToListAsync();
            var packageTypeResponses = _mapper.Map<List<PackageType>, List<PackageTypeResponse>>(packageTypes);
            return packageTypeResponses;
        }

        public async Task<PackageTypeResponse> CreatePackageType(PackageTypeRequest request)
        {
            var existingPackageType = await _unitOfWork.Repository<PackageType>().FindAsync(b => b.PackageType1 == request.PackageType);
            if (existingPackageType != null)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "PackageType đã tồn tại", request.PackageType);
            }


            var newPackageType = new PackageType
            {
                PackageType1 = request.PackageType,
                CreatedAt = request.CreateAt,

            };

            await _unitOfWork.Repository<PackageType>().InsertAsync(newPackageType);
            await _unitOfWork.CommitAsync();


            var PackageTypeResponses = _mapper.Map<PackageTypeResponse>(newPackageType);
            return PackageTypeResponses;
        }

        public async Task<PackageTypeResponse> UpdatePackageType(int id, PackageTypeRequest packageTypeRequest)
        {
            var packageType = await _unitOfWork.Repository<PackageType>()
                .GetAll()
                .FirstOrDefaultAsync(pt => pt.Id == id);

            if (packageType == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy loại gói hàng", id.ToString());
            }

            packageType.PackageType1 = packageTypeRequest.PackageType;
            packageType.CreatedAt= DateTime.Now;
            // Cập nhật các thuộc tính khác tương tự

            await _unitOfWork.Repository<PackageType>().Update(packageType, id);
            await _unitOfWork.CommitAsync();

            var updatedPackageTypeResponse = new PackageTypeResponse
            {
                Id = packageType.Id,
                CreatedAt= DateTime.Now,
               
            };

            return updatedPackageTypeResponse;
        }

    }
}
