using AutoMapper;
using LocalShipper.Data.Models;
using LocalShipper.Data.UnitOfWork;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
