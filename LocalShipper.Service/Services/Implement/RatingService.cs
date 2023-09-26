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
                ShipperName = rating.Shipper.LastName + " " + rating.Shipper.FirstName,
                RatingValue = rating.RatingValue,
                Comment = rating.Comment,
                RatingTime = rating.RatingTime,
                NameStore = rating.ByStore.StoreName
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
    }
}
