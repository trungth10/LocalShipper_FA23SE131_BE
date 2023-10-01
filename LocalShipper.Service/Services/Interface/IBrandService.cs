using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Interface
{
    public interface IBrandService
    {
        Task<List<BrandResponse>> GetBrands(int? id, string? brandName, string? iconUrl, string? imageUrl, int? accountId);
        Task<BrandResponse> PostBrand(BrandRequest request);
        Task<BrandResponse> UpdateBrand(int id, BrandRequest brandRequest);
        Task<BrandResponse> DeleteBrand(int id);
        Task<int> GetTotalBrandCount();


    }
}