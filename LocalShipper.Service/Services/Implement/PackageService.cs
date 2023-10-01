﻿using AutoMapper;
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
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

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

        public async Task<List<PackageResponse>> GetPackage(int? batchId,int? id, int? status,int? actionId, int? typeId,string? customerName,string? customerAddress, string? customerPhome, string? custommerEmail,decimal? totalPrice)
        {           
            var packages = await _unitOfWork.Repository<Package>().GetAll().Include(b => b.Type).Include(b => b.Action).Include(b => b.Batch)
                .Where(f => f.BatchId == batchId || batchId == 0)
                .Where(f => f.Id == id || id == 0)
                .Where(f => f.Status == status || status == 0)
                .Where(f => f.ActionId == actionId || actionId == 0)
                .Where(f => typeId== id || typeId == 0)
                .Where(f => string.IsNullOrWhiteSpace(customerName) ||f.CustomerName.Contains(customerName))
                .ToListAsync();

            //var packageResponses = packages.Select(package => new PackageResponse
            //{
            //    Id = package.Id,
            //    BatchId = package.BatchId,
            //    Capacity= package.Capacity,
            //    PackageWeight = package.PackageWeight,
            //    PackageWidth= package.PackageWidth,
            //    PackageHeight= package.PackageHeight,
            //    PackageLength= package.PackageLength,
            //    Status= package.Status,
            //    CustomerAddress= package.CustomerAddress,
            //    CustomerName= package.CustomerName,
            //    CustomerEmail= package.CustomerEmail,
            //    CancelReason= package.CancelReason,
            //    SubtotalPrice= package.SubtotalPrice,
            //    DistancePrice= package.DistancePrice,
            //    TotalPrice = package.TotalPrice,
            //    ActionId= package.ActionId,
            //    TypeId= package.TypeId,

            //}).ToList();
            var packageResponses = _mapper.Map<List<Package>, List<PackageResponse>>(packages);
            return packageResponses;
        }

        public async Task<PackageResponse> CreatePackage(PackageRequest request)
        {
            decimal distancePrice = 0;
            if (request.DistancePrice == 3)
            {
                distancePrice = 18;

            }
            else if (request.DistancePrice > 3)
            {
                distancePrice = 18 + (request.DistancePrice - 3) * 4;
            }
          
            var newPackage = new Package
            {

                BatchId = request.BatchId,
                Capacity = request.Capacity,
                PackageWeight = request.PackageWeight,
                PackageWidth = request.PackageWidth,
                PackageHeight = request.PackageHeight,
                PackageLength = request.PackageLength,
                Status = request.Status ?? 0,
                CustomerAddress = request.CustomerAddress,
                CustomerPhone = request.CustomerPhone,
                CustomerName = request.CustomerName,
                CustomerEmail = request.CustomerEmail,
                DistancePrice = distancePrice,
                SubtotalPrice = request.SubtotalPrice,
                TotalPrice = distancePrice+ request.SubtotalPrice,
                ActionId = request.ActionId,
                TypeId = request.TypeId ?? 0, 
            };

          
            await _unitOfWork.Repository<Package>().InsertAsync(newPackage);
            await _unitOfWork.CommitAsync();

            
            var packageResponse = _mapper.Map<PackageResponse>(newPackage);
            return packageResponse;
        }


        public async Task<PackageResponse> UpdatePackage(int id, PackageRequest packageRequest)
        {
            var package = await _unitOfWork.Repository<Package>()
                .GetAll().Include(b => b.Type).Include(b => b.Action).Include(b => b.Batch)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (package == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy gói hàng", id.ToString());
            }

            package.BatchId = packageRequest.BatchId;
            package.Capacity = packageRequest.Capacity;
            package.PackageWeight = packageRequest.PackageWeight;
            package.PackageWidth = packageRequest.PackageWidth;
            package.PackageHeight = packageRequest.PackageHeight;
            package.PackageLength = packageRequest.PackageLength;
            package.Status = packageRequest.Status ?? 0;
            package.CustomerAddress = packageRequest.CustomerAddress;
            package.CustomerPhone = packageRequest.CustomerPhone;
            package.CustomerName = packageRequest.CustomerName;
            package.CustomerEmail = packageRequest.CustomerEmail;
          

         
            package.DistancePrice = CalculateTotalPrice(packageRequest.DistancePrice);
            package.SubtotalPrice = packageRequest.SubtotalPrice;
            package.TotalPrice = CalculateTotalPrice(packageRequest.DistancePrice) + packageRequest.SubtotalPrice;

            await _unitOfWork.Repository<Package>().Update(package, id);
            await _unitOfWork.CommitAsync();

            //var updatedPackageResponse = new PackageResponse
            //{
            //    Id = package.Id,
            //    BatchId = package.BatchId,
            //    Capacity = package.Capacity,
            //    PackageWeight = package.PackageWeight,
            //    PackageWidth = package.PackageWidth,
            //    PackageHeight = package.PackageHeight,
            //    PackageLength = package.PackageLength,
            //    Status = package.Status,
            //    CustomerAddress = package.CustomerAddress,
            //    CustomerPhone = package.CustomerPhone,
            //    CustomerName = package.CustomerName,
            //    CustomerEmail = package.CustomerEmail,
            //    DistancePrice = package.DistancePrice,
            //    SubtotalPrice = package.SubtotalPrice,
            //    TotalPrice = package.TotalPrice,
            //    ActionId = package.ActionId,
            //    TypeId = package.TypeId
              
            //};
            var updatedPackageResponse = _mapper.Map<PackageResponse>(package);
            return updatedPackageResponse;
        }
        private decimal CalculateTotalPrice(decimal distancePrice)
        {
            if (distancePrice == 3)
            {
                return 18;
            }
            else if (distancePrice > 3)
            {
                return 18 + (distancePrice - 3) * 4;
            }
            else
            {
                return 0;
            }
        }

        public async Task<MessageResponse> DeletePackage(int id)
        {
            var package = await _unitOfWork.Repository<Package>().GetAll().FindAsync(id);

            if (package == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy gói hàng", id.ToString());
            }

            _unitOfWork.Repository<Package>().Delete(package);
            await _unitOfWork.CommitAsync();

            return new MessageResponse
            {
                Message = "Xóa gói hàng thành công",
            };
        }
        public async Task<int> GetTotalPackageCount()
        {
            var count = await _unitOfWork.Repository<Package>()
                .GetAll()
                .CountAsync();

            return count;
        }
    }
}
