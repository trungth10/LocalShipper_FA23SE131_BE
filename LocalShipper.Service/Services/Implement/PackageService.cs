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

        public async Task<List<PackageResponse>> GetPackage(int? batchId,int? id, int? status,int? actionId, int? typeId,string? customerName)
        {           
            var package = await _unitOfWork.Repository<Package>().GetAll()
                .Where(f => f.BatchId == batchId || batchId == 0)
                .Where(f => f.Id == id || id == 0)
                .Where(f => f.Status == status || status == 0)
                .Where(f => f.ActionId == actionId || actionId == 0)
                .Where(f => typeId== id || typeId == 0)
                .Where(f => string.IsNullOrWhiteSpace(customerName) ||f.CustomerName.Contains(customerName))
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
