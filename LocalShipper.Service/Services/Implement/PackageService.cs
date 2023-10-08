using AutoMapper;
using LocalShipper.Data.Models;
using LocalShipper.Data.UnitOfWork;
using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Exceptions;
using LocalShipper.Service.Helpers;
using LocalShipper.Service.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Crmf;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Math;
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
            int? actionId, int? typeId, int? storeId, string? customerName,
             int? pageNumber, int? pageSize)
        {
            var packages = _unitOfWork.Repository<Package>().GetAll().Include(b => b.Type).Include(b => b.Action).Include(b => b.Batch).Include(b => b.Store)
                .Where(f => f.BatchId == batchId || batchId == 0)
                .Where(f => f.Id == id || id == 0)
                .Where(f => f.Status == status || status == 0)
                .Where(f => f.ActionId == actionId || actionId == 0)
                .Where(f => typeId == id || typeId == 0)
                .Where(f => f.StoreId == storeId || storeId == 0)
                .Where(f => string.IsNullOrWhiteSpace(customerName) || f.CustomerName.Contains(customerName.Trim()));


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
        private int? GetPriceItemId(List<PriceItem> priceItems, double distancePrice)
        {
            foreach (var priceItem in priceItems)
            {

                if (distancePrice >= priceItem.MinDistance && distancePrice <= priceItem.MaxDistance)
                {

                    return priceItem.Id;
                }
            }

            // Nếu không tìm thấy PriceItem phù hợp, trả về null
            return null;
        }
        public async Task<decimal?> GetMaxDistance(int? storeId)
        {
            var priceL = await _unitOfWork.Repository<PriceL>().GetAll()
                .FirstOrDefaultAsync(b => b.StoreId == storeId);

            if (priceL != null)
            {
                var maxDistance = await _unitOfWork.Repository<PriceItem>().GetAll()
                    .Where(b => b.PriceId == priceL.Id)
                    .Select(b => b.MaxDistance)
                    .MaxAsync();

                return (decimal)maxDistance;
            }

            return null; // Trả về null nếu không tìm thấy PriceL
        }
        public async Task<PackageResponse> CreatePackage(PackageRequestForCreate request)
        {
            decimal distancePrice = 0;
            decimal distancePriceMax1 = 0;

            decimal max1;
            decimal min1;
            decimal max2;
            decimal min2;
            decimal minAmount1;
            decimal minAmount2;
            decimal maxAmount1;
            decimal maxAmount2;
            decimal price1;
            if (request.StoreId.HasValue)
            {
                var priceL = await _unitOfWork.Repository<PriceL>().GetAll()
                    .FirstOrDefaultAsync(b => b.StoreId == request.StoreId);

                decimal? maxDistance = await GetMaxDistance(priceL.StoreId);

                if (priceL != null )
                {
                    var priceItems = await _unitOfWork.Repository<PriceItem>().GetAll()
                        .Where(b => b.PriceId == priceL.Id)
                        .ToListAsync();
                    var id = priceItems.Select(b => b.Id).ToList();

                    var firstId = id.FirstOrDefault();

                    var secondItem = id.Skip(1).FirstOrDefault();
                    if (GetPriceItemId(priceItems, (double)request.DistancePrice) == priceItems.FirstOrDefault().Id)
                    {

                        price1 = priceItems
                       .Where(b => b.MinDistance <= (double)request.DistancePrice && b.MaxDistance >= (double)request.DistancePrice)
                       .Select(b => (decimal)b.Price)
                       .FirstOrDefault();
                        max1 = priceItems
                                .Where(b => b.MinDistance <= (double)request.DistancePrice && b.MaxDistance >= (double)request.DistancePrice && firstId == priceItems.FirstOrDefault().Id)
                                .Select(b => (decimal)b.MaxDistance)
                                .FirstOrDefault();
                        maxAmount1 = priceItems
                                .Where(b =>b.MaxDistance < (double)request.DistancePrice)
                                .Select(b => (decimal)b.MaxAmount)
                                .FirstOrDefault();
                        min1 = priceItems
                                .Where(b => b.MinDistance <= (double)request.DistancePrice && b.MaxDistance >= (double)request.DistancePrice && firstId == priceItems.FirstOrDefault().Id)
                                .Select(b => (decimal)b.MinDistance)
                                .FirstOrDefault();
                        minAmount1 = priceItems
                                .Where(b => b.MinDistance <= (double)request.DistancePrice && b.MaxDistance >= (double)request.DistancePrice && firstId == priceItems.FirstOrDefault().Id)
                                .Select(b => (decimal)b.MinAmount)
                                .FirstOrDefault();
                        if (request.DistancePrice >= min1 && request.DistancePrice <= max1)
                        {
                            distancePrice = priceItems
                                .Where(b => b.MinDistance <= (double)request.DistancePrice && b.MaxDistance >= (double)request.DistancePrice)
                                .Select(b => b.Price)
                                .FirstOrDefault();
                            distancePriceMax1 = priceItems
                                .Where(b => b.MinDistance <= (double)request.DistancePrice && b.MaxDistance >= (double)request.DistancePrice)
                                .Select(b => (decimal)b.MaxDistance)
                                .FirstOrDefault();
                            if (request.DistancePrice <= distancePriceMax1 && request.DistancePrice >= 1)
                            {
                                distancePrice = request.DistancePrice * distancePrice;


                            }
                            else if (request.DistancePrice < 1)
                            {
                                distancePrice = minAmount1;
                            }
                            else if (request.DistancePrice > max1)
                            {
                                distancePrice = maxAmount1;

                            }

                        }

                    }

                    else if(GetPriceItemId(priceItems, (double)request.DistancePrice) == priceItems.Skip(1).FirstOrDefault().Id || request.DistancePrice > maxDistance.Value)
                    {

                        max2 = priceItems
                            .Where(b => b.MinDistance <= (double)request.DistancePrice && b.MaxDistance >= (double)request.DistancePrice)
                            .Select(b => (decimal)b.MaxDistance)
                            .FirstOrDefault();
                        maxAmount2 = priceItems
                               .Where (b => b.MaxDistance < (double)request.DistancePrice)
                               .Select(b => (decimal)b.MaxAmount)
                               .FirstOrDefault();
                        min2 = priceItems
                                .Where(b => b.MinDistance <= (double)request.DistancePrice && b.MaxDistance >= (double)request.DistancePrice)
                                .Select(b => (decimal)b.MinDistance)
                                .FirstOrDefault();
                        minAmount2 = priceItems
                                .Where(b => b.MinDistance <= (double)request.DistancePrice && b.MaxDistance >= (double)request.DistancePrice)
                                .Select(b => (decimal)b.MinAmount)
                                .FirstOrDefault();

                        decimal max = (decimal)priceItems.FirstOrDefault().MaxDistance;
                        decimal price = (decimal)priceItems.FirstOrDefault().Price;
                        if (request.DistancePrice >= min2 && request.DistancePrice <= max2)
                        {
                            var distancePrice2 = priceItems
                                .Where(b => b.MinDistance >= (double)min2 && b.MaxDistance <= (double)max2)
                                .Select(b => b.Price)
                                .FirstOrDefault();
                            distancePrice = max * price + (request.DistancePrice - max) * distancePrice2;
                        }
                        else if (request.DistancePrice > max2 && request.DistancePrice <= min2)
                        {
                            distancePrice = minAmount2;
                        }
                        else if (request.DistancePrice > max2)
                        {
                            distancePrice = maxAmount2;
                        }



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


        public async Task<PackageResponse> UpdatePackage(int id, PackageRequestForCreate request)
        {
            var storeId = await _unitOfWork.Repository<Package>().GetAll().Where(b => b.Id == id).Select(b => b.StoreId).FirstOrDefaultAsync();
              

            var package = await _unitOfWork.Repository<Package>()
           .GetAll().Include(b => b.Type).Include(b => b.Action).Include(b => b.Batch)
           .FirstOrDefaultAsync(p => p.Id == id);
            #region price
            decimal distancePrice = 0;
            decimal distancePriceMax1 = 0;

            decimal max1;
            decimal min1;
            decimal max2;
            decimal min2;
            decimal minAmount1;
            decimal minAmount2;
            decimal maxAmount1;
            decimal maxAmount2;
            decimal price1;

            if (request.StoreId.HasValue)
            {
                var priceL = await _unitOfWork.Repository<PriceL>().GetAll()
                    .FirstOrDefaultAsync(b => b.StoreId == request.StoreId);

                if (priceL != null)
                {
                    var priceItems = await _unitOfWork.Repository<PriceItem>().GetAll()
                        .Where(b => b.PriceId == priceL.Id)
                        .ToListAsync();
                    var ids = priceItems.Select(b => b.Id).ToList();

                    var firstId = ids.FirstOrDefault();

                    var secondItem = ids.Skip(1).FirstOrDefault();
                    if (GetPriceItemId(priceItems, (double)request.DistancePrice) == priceItems.FirstOrDefault().Id)
                    {

                        price1 = priceItems
                       .Where(b => b.MinDistance <= (double)request.DistancePrice && b.MaxDistance >= (double)request.DistancePrice)
                       .Select(b => (decimal)b.Price)
                       .FirstOrDefault();
                        max1 = priceItems
                                .Where(b => b.MinDistance <= (double)request.DistancePrice && b.MaxDistance >= (double)request.DistancePrice && firstId == priceItems.FirstOrDefault().Id)
                                .Select(b => (decimal)b.MaxDistance)
                                .FirstOrDefault();
                        maxAmount1 = priceItems
                                .Where(b => b.MaxDistance < (double)request.DistancePrice )
                                .Select(b => (decimal)b.MaxAmount)
                                .FirstOrDefault();
                        min1 = priceItems
                                .Where(b => b.MinDistance <= (double)request.DistancePrice && b.MaxDistance >= (double)request.DistancePrice && firstId == priceItems.FirstOrDefault().Id)
                                .Select(b => (decimal)b.MinDistance)
                                .FirstOrDefault();
                        minAmount1 = priceItems
                                .Where(b => b.MinDistance <= (double)request.DistancePrice && b.MaxDistance >= (double)request.DistancePrice && firstId == priceItems.FirstOrDefault().Id)
                                .Select(b => (decimal)b.MinAmount)
                                .FirstOrDefault();
                        if (request.DistancePrice >= min1 && request.DistancePrice <= max1)
                        {
                            distancePrice = priceItems
                                .Where(b => b.MinDistance <= (double)request.DistancePrice && b.MaxDistance >= (double)request.DistancePrice)
                                .Select(b => b.Price)
                                .FirstOrDefault();
                            distancePriceMax1 = priceItems
                                .Where(b => b.MinDistance <= (double)request.DistancePrice && b.MaxDistance >= (double)request.DistancePrice)
                                .Select(b => (decimal)b.MaxDistance)
                                .FirstOrDefault();
                            if (request.DistancePrice <= distancePriceMax1 && request.DistancePrice >= 1)
                            {
                                distancePrice = request.DistancePrice * distancePrice;


                            }
                            else if (request.DistancePrice < 1)
                            {
                                distancePrice = minAmount1;
                            }
                            else if (request.DistancePrice > max1)
                            {
                                distancePrice = maxAmount1;

                            }

                        }

                    }

                    else
                    {
                        max2 = priceItems
                       .Where(b => b.MinDistance <= (double)request.DistancePrice && b.MaxDistance >= (double)request.DistancePrice && secondItem == priceItems.Skip(1).FirstOrDefault().Id)
                       .Select(b => (decimal)b.MaxDistance)
                       .FirstOrDefault();
                        maxAmount2 = priceItems
                               .Where(b => b.MaxDistance < (double)request.DistancePrice )
                               .Select(b => (decimal)b.MaxAmount)
                               .FirstOrDefault();
                        min2 = priceItems
                                .Where(b => b.MinDistance <= (double)request.DistancePrice && b.MaxDistance >= (double)request.DistancePrice && secondItem == priceItems.Skip(1).FirstOrDefault().Id)
                                .Select(b => (decimal)b.MinDistance)
                                .FirstOrDefault();
                        minAmount2 = priceItems
                                .Where(b => b.MinDistance <= (double)request.DistancePrice && b.MaxDistance >= (double)request.DistancePrice && secondItem == priceItems.Skip(1).FirstOrDefault().Id)
                                .Select(b => (decimal)b.MinAmount)
                                .FirstOrDefault();

                        decimal max = (decimal)priceItems.FirstOrDefault().MaxDistance;
                        decimal price = (decimal)priceItems.FirstOrDefault().Price;
                        if (request.DistancePrice >= min2 && request.DistancePrice <= max2)
                        {
                            var distancePrice2 = priceItems
                                .Where(b => b.MinDistance >= (double)min2 && b.MaxDistance <= (double)max2)
                                .Select(b => b.Price)
                                .FirstOrDefault();
                            distancePrice = max * price + (request.DistancePrice - max) * distancePrice2;
                        }
                        else if (request.DistancePrice > max2 && request.DistancePrice <= min2)
                        {
                            distancePrice = minAmount2;
                        }
                        else if (request.DistancePrice > max2)
                        {
                            distancePrice = maxAmount2;
                        }

                    }
                }


            }
            #endregion



            if (package == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy gói hàng", id.ToString());
            }
             var storeId1 = request.StoreId = storeId;
            package.StoreId = storeId1 ?? 0;
            package.Capacity = request.Capacity;
            package.PackageWeight = request.PackageWeight;
            package.PackageWidth = request.PackageWidth;
            package.PackageHeight = request.PackageHeight;
            package.PackageLength = request.PackageLength;

            package.CustomerAddress = request.CustomerAddress;
            package.CustomerPhone = request.CustomerPhone;
            package.CustomerName = request.CustomerName;
            package.CustomerEmail = request.CustomerEmail;
            package.PackagePrice = request.PackagePrice;
            package.DistancePrice = distancePrice;
            package.SubtotalPrice = request.SubtotalPrice;
            package.TotalPrice = distancePrice + request.SubtotalPrice;
            await _unitOfWork.Repository<Package>().Update(package, id);
            await _unitOfWork.CommitAsync();

            var updatedPackageResponse = _mapper.Map<PackageResponse>(package);

            //var updatedPackageResponse = new 
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
                id= id,
                Message = "Xóa gói hàng thành công",
            };
        } 
        public async Task<int> GetTotalPackageCount(int? batchId)
        {
            var count = await _unitOfWork.Repository<Package>()
                .GetAll()
                .Where(f => f.BatchId == batchId || batchId == 0)
                .CountAsync();

            return count;
        }
    }
}
