using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Interface
{
    public interface IRatingService
    {
        Task<List<RatingResponse>> GetRatingByShipperId(int shipperId);
        Task<decimal> GetAverageRatingByShipperId(int shipperId);
        Task<List<RatingResponse>> GetRating(int? id, int? shipperId, int? ratingValue, int? byStoreId);


        Task<int> GetTotalRatingCount();
        Task<RatingResponse> CreateRating(RegisterRatingRequest request);
        Task<RatingResponse> UpdateRating(int id, PutRatingRequest ratingRequest);
        Task<MessageResponse> DeleteRating(int id);

    }
}
