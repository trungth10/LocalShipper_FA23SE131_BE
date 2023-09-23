using AutoMapper;
using LocalShipper.Data.Models;
using LocalShipper.Data.UnitOfWork;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Helpers;
using LocalShipper.Service.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Implement
{
    public class PackageService : IPackageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public PackageService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<PackageResponse>> GetPackageByBatchId(int batchId)
        {           
            var package = await _unitOfWork.Repository<Package>().GetAll()
                .Where(f => f.BatchId == batchId)
                .ToListAsync();

            var packageResponses = package.Select(package => new PackageResponse
            {
                Id = package.Id,
                BatchId = package.BatchId,
                Capacity= package.Capacity,
                PackageWeight = package.PackageWeight,
                PackageWidth= package.PackageWidth,
                PackageHeight= package.PackageHeight,
                PackageLength= package.PackageLength,
                Status= package.Status,
                CustomerAddress= package.CustomerAddress,
                CustomerName= package.CustomerName,
                CustomerEmail= package.CustomerEmail,
                CancelReason= package.CancelReason,
                SubtotalPrice= package.SubtotalPrice,
                DistancePrice= package.DistancePrice,
                ActionId= package.ActionId,
                TypeId= package.TypeId,
                
            }).ToList();

            return packageResponses;
        }
    }
}
