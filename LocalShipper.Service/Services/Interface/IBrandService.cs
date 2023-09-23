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
        Task<List<BrandResponse>> GetBrands(BrandPagingRequest request);
        Task<BrandResponse> PostBrand(PostBrandRequest model, int role);
        Task<BrandResponse> PutBrand(int id, PostBrandRequest model);
        Task<BrandResponse> DeleteBrand(int id);
        Task<BrandResponse> GetBrandByID(int id);
    }
}