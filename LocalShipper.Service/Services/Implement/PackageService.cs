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


        public async Task<List<PackageResponse>> GetPackage(int? batchId, int? id, int? status,
            int? actionId, int? typeId, int? storeId, string? customerName, string? customerAddress,
            string? customerPhome, string? custommerEmail, decimal? totalPrice, int? pageNumber, int? pageSize)
        {
            var packages = _unitOfWork.Repository<Package>().GetAll().Include(b => b.Type).Include(b => b.Action).Include(b => b.Batch).Include(b => b.Store)
                .Where(f => f.BatchId == batchId || batchId == 0)
                .Where(f => f.Id == id || id == 0)
                .Where(f => f.Status == status || status == 0)
                .Where(f => f.ActionId == actionId || actionId == 0)
                .Where(f => typeId == id || typeId == 0)
                .Where(f => f.StoreId == storeId || storeId == 0)
                .Where(f => string.IsNullOrWhiteSpace(customerName) || f.CustomerName.Contains(customerName));


            // Xác định giá trị cuối cùng của pageNumber
            pageNumber = pageNumber.HasValue ? Math.Max(1, pageNumber.Value) : 1;
            // Áp dụng phân trang nếu có thông số pageNumber và pageSize
            if (pageNumber.HasValue && pageSize.HasValue)
            {
                packages = packages.Skip((pageNumber.Value - 1) * pageSize.Value)
                                       .Take(pageSize.Value);
            }

            var packageActionList = await packages.ToListAsync();
            if (packageActionList == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Package không có hoặc không tồn tại", id.ToString());
            }

            var packageList = await packages.ToListAsync();
            if (packageList == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Package không có hoặc không tồn tại", id.ToString());
            }
            var packageResponses = _mapper.Map<List<Package>, List<PackageResponse>>(packageList);
            return packageResponses;
        }

        public async Task<PackageResponse> CreatePackage(PackageRequestForCreate request)
        {
            decimal distancePrice = 0;

            if (request.StoreId.HasValue) // Kiểm tra nếu StoreId có giá trị
            {

                var priceL = await _unitOfWork.Repository<PriceL>().GetAll()
                                                                   .FirstOrDefaultAsync(pl => pl.StoreId == request.StoreId);

                if (priceL != null)
                {
                    if (request.DistancePrice < 1)
                    {
                        distancePrice = 3;
                    }
                    else if (request.DistancePrice >= 1 && request.DistancePrice <= 3)
                    {
                        distancePrice = request.DistancePrice * 3;
                    }
                    else if (request.DistancePrice >= 4 && request.DistancePrice <= 10)
                    {
                        distancePrice = 3 * 3 + (request.DistancePrice - 3) * 2;
                    }
                    else
                    {
                        distancePrice = 35;
                    }

                }               
            }

    
            var newPackage = new Package
            {
                StoreId = request.StoreId.HasValue ? request.StoreId.Value : 0,
                Capacity = request.Capacity,
                PackageWeight = request.PackageWeight,
                PackageWidth = request.PackageWidth,
                PackageHeight = request.PackageHeight,
                PackageLength = request.PackageLength,
                Status = (int)PackageStatusEnum.IDLE,
                CustomerAddress = request.CustomerAddress,
                CustomerPhone = request.CustomerPhone,
                CustomerName = request.CustomerName,
                CustomerEmail = request.CustomerEmail,
                PackagePrice = request.PackagePrice,
                DistancePrice = distancePrice,
                SubtotalPrice = request.SubtotalPrice,
                TotalPrice = distancePrice + request.SubtotalPrice,
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

            var updatedPackageResponse = _mapper.Map<PackageResponse>(package);
            return updatedPackageResponse;
        }

        public async Task<PackageResponse> UpdateStatusPackage(int id, PackageStatusEnum status)
        {
            var package = await _unitOfWork.Repository<Package>()
                .GetAll().Include(b => b.Type).Include(b => b.Action).Include(b => b.Batch)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (package == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy gói hàng", id.ToString());
            }

            package.Status = (int)status;

            await _unitOfWork.Repository<Package>().Update(package, id);
            await _unitOfWork.CommitAsync();

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
