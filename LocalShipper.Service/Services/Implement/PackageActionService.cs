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
    public class PackageActionService : IPackageActionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public PackageActionService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<PackageActionResponse>> GetPackageAction(int? id, string? actionType)
        {

            var packageAction = await _unitOfWork.Repository<PackageAction>().GetAll()
                                                              .Where(b => id == 0 || b.Id == id)

                                                              .Where(b => string.IsNullOrWhiteSpace(actionType) || b.ActionType.Contains(actionType))
                                                              .ToListAsync();
            var packageActionResponses = _mapper.Map<List<PackageAction>, List<PackageActionResponse>>(packageAction);
            return packageActionResponses;
        }
    }
}
