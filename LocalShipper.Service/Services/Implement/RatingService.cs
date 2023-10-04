using AutoMapper;
using LocalShipper.Data.Models;
using LocalShipper.Data.UnitOfWork;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Services.Interface;
using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.Exceptions;
using System.Net;
using Org.BouncyCastle.Asn1.Ocsp;

namespace LocalShipper.Service.Services.Implement
{
    public class RatingService : IRatingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public RatingService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<List<RatingResponse>> GetRatingByShipperId(int shipperId)
        {
            // Sử dụng ToListAsync() để chuyển đổi IQueryable thành danh sách bất đồng bộ
            var ratings = await _unitOfWork.Repository<Rating>()
                .GetAll()
                .Include(o => o.ByStore).Include(p => p.Shipper)
                .Where(f => f.ShipperId == shipperId)
                .ToListAsync();

            var ratingResponse = ratings.Select(rating => new RatingResponse
            {
                Id = rating.Id,
                ShipperId = rating.ShipperId,
                ShipperName = rating.Shipper.FullName,
                RatingValue = rating.RatingValue,
                Comment = rating.Comment,
                RatingTime = rating.RatingTime,
                NameStore = rating.ByStore.StoreName,
                Shipper = rating.Shipper != null ? new ShipperResponse
                {
                    Id = rating.Shipper.Id,
                    FullName = rating.Shipper.FullName,
                    EmailShipper = rating.Shipper.EmailShipper,
                    PhoneShipper = rating.Shipper.PhoneShipper,
                    AddressShipper = rating.Shipper.AddressShipper,
                    TransportId = rating.Shipper.TransportId,
                    AccountId = rating.Shipper.AccountId,
                    ZoneId = rating.Shipper.ZoneId,
                    Status = (Helpers.ShipperStatusEnum)rating.Shipper.Status,
                    Fcmtoken = rating.Shipper.Fcmtoken,
                    WalletId = rating.Shipper.WalletId,
                } : null,
                Store = rating.ByStore != null ? new StoreResponse
                {
                    Id = rating.ByStore.Id,
                    StoreName = rating.ByStore.StoreName,
                    StoreAddress = rating.ByStore.StoreAddress,
                    StorePhone = rating.ByStore.StorePhone,
                    StoreEmail = rating.ByStore.StoreEmail,
                    OpenTime = rating.ByStore.OpenTime,
                    CloseTime = rating.ByStore.CloseTime,
                    StoreDescription = rating.ByStore.StoreDescription,
                    Status = rating.ByStore.Status,
                    TemplateId = rating.ByStore.TemplateId,
                    ZoneId = rating.ByStore.ZoneId,
                    WalletId = rating.ByStore.WalletId,
                    AccountId = rating.ByStore.AccountId,
                } : null
            }).ToList();

            return ratingResponse;
        }
        public async Task<decimal> GetAverageRatingByShipperId(int shipperId)
        {
            var ratings = await _unitOfWork.Repository<Rating>()
                .GetAll()
                .Where(r => r.ShipperId == shipperId)
                .ToListAsync();

            if (ratings.Count == 0)
            {
               
                return 0;
            }

            decimal averageRating =(decimal)ratings.Average(r => r.RatingValue);
            return averageRating;
        }



        //CREATE Rating
        public async Task<RatingResponse> CreateRating(RegisterRatingRequest request)
        {
            if (request.RatingValue < 1 || request.RatingValue > 5)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Đánh giá thất bại. Vui lòng nhập lại từ 1 đến 5.", ToString());
            }
            var shipperExisted = _unitOfWork.Repository<Rating>().Find(x => x.ShipperId == request.ShipperId);

            if (shipperExisted != null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Shipper Id không tồn tại", request.ShipperId.ToString());
            }

            Rating rating = new Rating
            {
                ShipperId = request.ShipperId,
                RatingValue = request.RatingValue,
                Comment = request.Comment,
                ByStoreId = request.ByStoreId,
                RatingTime = request.RatingTime
            };
            
            await _unitOfWork.Repository<Rating>().InsertAsync(rating);
            await _unitOfWork.CommitAsync();

            var createdRatingResponse = new RatingResponse
            {
                Id = rating.Id,
                ShipperId = rating.ShipperId,
                RatingValue = rating.RatingValue,
                Comment = rating.Comment,
                ByStoreId = rating.ByStoreId,
                RatingTime = rating.RatingTime,
            };
            return createdRatingResponse;

        }

        //GET 
        public async Task<List<RatingResponse>> GetRating(int? id, int? shipperId, int? ratingValue, int? byStoreId, int? pageNumber, int? pageSize)
        {

            var ratings = _unitOfWork.Repository<Rating>().GetAll().Include(t => t.Shipper).Include(t => t.ByStore)
                    .Where(t => id == 0 || t.Id == id)
                    .Where(t => shipperId == 0 || t.ShipperId == shipperId)
                    .Where(t => ratingValue == 0 || t.RatingValue == ratingValue)
                    .Where(t => byStoreId == 0 || t.ByStoreId == byStoreId);


            // Xác định giá trị cuối cùng của pageNumber
            pageNumber = pageNumber.HasValue ? Math.Max(1, pageNumber.Value) : 1;
            // Áp dụng phân trang nếu có thông số pageNumber và pageSize
            if (pageNumber.HasValue && pageSize.HasValue)
            {
                ratings = ratings.Skip((pageNumber.Value - 1) * pageSize.Value)
                                       .Take(pageSize.Value);
            }

            var ratingList = await ratings.ToListAsync();
            if (ratingList == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Đánh giá không có hoặc không tồn tại", id.ToString());
            }
            var ratingResponses = ratingList.Select(rating => new RatingResponse
            {
                Id = rating.Id,
                ShipperId = rating.ShipperId,
                RatingValue = rating.RatingValue,
                Comment = rating.Comment,
                ByStoreId = rating.ByStoreId,
                RatingTime = rating.RatingTime,
                Shipper = rating.Shipper != null ? new ShipperResponse
                {
                    Id = rating.Shipper.Id,
                    FullName = rating.Shipper.FullName,
                    EmailShipper = rating.Shipper.EmailShipper,
                    PhoneShipper = rating.Shipper.PhoneShipper,
                    AddressShipper = rating.Shipper.AddressShipper,
                    TransportId = rating.Shipper.TransportId,
                    AccountId = rating.Shipper.AccountId,
                    ZoneId = rating.Shipper.ZoneId,
                } : null,
                Store = rating.ByStore != null ? new StoreResponse
                {
                    Id = rating.ByStore.Id,
                    StoreName = rating.ByStore.StoreName,
                    StoreAddress = rating.ByStore.StoreAddress,
                    StorePhone = rating.ByStore.StorePhone,
                    StoreEmail = rating.ByStore.StoreEmail,
                    OpenTime = rating.ByStore.OpenTime,
                    CloseTime = rating.ByStore.CloseTime,
                    StoreDescription = rating.ByStore.StoreDescription,
                    Status = rating.ByStore.Status,
                    TemplateId = rating.ByStore.TemplateId,
                    ZoneId = rating.ByStore.ZoneId,
                    WalletId = rating.ByStore.WalletId,
                    AccountId = rating.ByStore.AccountId,
                } : null
            }).ToList();
            return ratingResponses;
        }

        //GET Count
        public async Task<int> GetTotalRatingCount()
        {
            var count = await _unitOfWork.Repository<Rating>()
                .GetAll()
                .CountAsync();

            return count;
        }

        //UPDATE Rating
        public async Task<RatingResponse> UpdateRating(int id, PutRatingRequest ratingRequest)
        {
            var rating = await _unitOfWork.Repository<Rating>()
                .GetAll()
                .Include(o => o.Shipper).Include(o => o.ByStore)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (rating == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy đánh giá", id.ToString());
            }
            if (ratingRequest.RatingValue < 1 || ratingRequest.RatingValue > 5)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Đánh giá không hợp lệ. Vui lòng nhập lại từ 1 đến 5.", ToString());
            }
            rating.ShipperId = ratingRequest.ShipperId;
            rating.RatingValue = ratingRequest.RatingValue;
            rating.Comment = ratingRequest.Comment;
            rating.ByStoreId = ratingRequest.ByStoreId;
            rating.RatingTime = ratingRequest.RatingTime;



            await _unitOfWork.Repository<Rating>().Update(rating, id);
            await _unitOfWork.CommitAsync();

            var updatedRatingResponse = new RatingResponse
            {
                Id = rating.Id,
                ShipperId = rating.ShipperId,
                RatingValue = rating.RatingValue,
                Comment = rating.Comment,
                ByStoreId = rating.ByStoreId,
                RatingTime = rating.RatingTime,

                Shipper = rating.Shipper != null ? new ShipperResponse
                {
                    Id = rating.Shipper.Id,
                    FullName = rating.Shipper.FullName,
                    EmailShipper = rating.Shipper.EmailShipper,
                    PhoneShipper = rating.Shipper.PhoneShipper,
                    AddressShipper = rating.Shipper.AddressShipper,
                    TransportId = rating.Shipper.TransportId,
                    AccountId = rating.Shipper.AccountId,
                    ZoneId = rating.Shipper.ZoneId,
                } : null,
                Store = rating.ByStore != null ? new StoreResponse
                {
                    Id = rating.ByStore.Id,
                    StoreName = rating.ByStore.StoreName,
                    StoreAddress = rating.ByStore.StoreAddress,
                    StorePhone = rating.ByStore.StorePhone,
                    StoreEmail = rating.ByStore.StoreEmail,
                    OpenTime = rating.ByStore.OpenTime,
                    CloseTime = rating.ByStore.CloseTime,
                    StoreDescription = rating.ByStore.StoreDescription,
                    Status = rating.ByStore.Status,
                    TemplateId = rating.ByStore.TemplateId,
                    ZoneId = rating.ByStore.ZoneId,
                    WalletId = rating.ByStore.WalletId,
                    AccountId = rating.ByStore.AccountId,
                } : null
            };

            return updatedRatingResponse;
        }

        //DELETE Rating
        public async Task<MessageResponse> DeleteRating(int id)
        {

            var rating = await _unitOfWork.Repository<Rating>().GetAll()
            .FirstOrDefaultAsync(a => a.Id == id);

            if (rating == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy đánh giá", id.ToString());
            }

            _unitOfWork.Repository<Rating>().Delete(rating);
            await _unitOfWork.CommitAsync();

            return new MessageResponse
            {
                id = id,
                Message = "Xóa đánh giá thành công",
            };
        }
    }
}



