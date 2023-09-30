using AutoMapper;
using LocalShipper.Data.Models;
using LocalShipper.Data.UnitOfWork;
using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Exceptions;
using LocalShipper.Service.Helpers;
using LocalShipper.Service.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Implement
{
    public class BrandService : IBrandService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public BrandService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
       



      

        public async Task<List<BrandResponse>> GetBrands(int? id, string? brandName, string? brandDescripton, string? iconUrl, string? imageUrl, int? accountId)
        {
            var brands = await _unitOfWork.Repository<Brand>().GetAll()
                                                               .Where(b => id == 0 || b.Id == id)
                                                               .Where(b => string.IsNullOrWhiteSpace(brandName) || b.BrandName.Contains(brandName))
                                                               .Where(b => string.IsNullOrWhiteSpace(brandDescripton) || b.BrandDescription.Contains(brandDescripton))
                                                               
                                                               .Where(b => string.IsNullOrWhiteSpace(iconUrl) || b.IconUrl.Contains(iconUrl))
                                                               .Where(b => string.IsNullOrWhiteSpace(imageUrl) || b.ImageUrl.Contains(imageUrl))
                                                              
                                                               .Where(b => accountId == 0 || b.AccountId == accountId)
                                                              
                                                               .ToListAsync();
            var brandResponses = _mapper.Map<List<Brand>, List<BrandResponse>>(brands);
            return brandResponses;
        }

        
        public async Task<BrandResponse> PostBrand(BrandRequest request)
        {
          
            var existingBrand = await _unitOfWork.Repository<Brand>().FindAsync(b => b.BrandName == request.BrandName);
            if (existingBrand != null)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Brand đã tồn tại", request.BrandName);
            }
            var existingAccountId = await _unitOfWork.Repository<Brand>().FindAsync(b => b.AccountId == request.AccountId);
            if (existingAccountId != null)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "AccountId đã tồn tại", request.AccountId.ToString());
            }

           
            var newBrand = new Brand
            {
               BrandName= request.BrandName,
               BrandDescription= request.BrandDescription,
               CreatedAt= request.CreatedAt,
               IconUrl= request.IconUrl,
               ImageUrl= request.ImageUrl,
               Active=request.Active,
               AccountId= request.AccountId,
            };

           
            await _unitOfWork.Repository<Brand>().InsertAsync(newBrand);
            await _unitOfWork.CommitAsync();

          
            var brandResponse = _mapper.Map<BrandResponse>(newBrand);
            return brandResponse;
        }

        public async Task<BrandResponse> UpdateBrand(int id, BrandRequest brandRequest)
        {
            var brand = await _unitOfWork.Repository<Brand>()
                .GetAll()
                .FirstOrDefaultAsync(b => b.Id == id);

            if (brand == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy thương hiệu", id.ToString());
            }

            brand.BrandName = brandRequest.BrandName;
            brand.BrandDescription = brandRequest.BrandDescription;
            brand.IconUrl = brandRequest.IconUrl;
            brand.ImageUrl = brandRequest.ImageUrl;
            brand.Active = brandRequest.Active;
            brand.AccountId = brandRequest.AccountId;
            brand.CreatedAt = DateTime.Now;
           

            await _unitOfWork.Repository<Brand>().Update(brand, id);
            await _unitOfWork.CommitAsync();

            var updatedBrandResponse = new BrandResponse
            {
                Id = brand.Id,
                BrandName = brand.BrandName,
                BrandDescription = brand.BrandDescription,
                IconUrl = brand.IconUrl,
                ImageUrl = brand.ImageUrl,
                Active = brand.Active,

                AccountId = brand.AccountId,
                CreatedAt = DateTime.Now,
            
            };

            return updatedBrandResponse;
        }
        public async Task<BrandResponse> DeleteBrand(int id)
        {
            var brand = await _unitOfWork.Repository<Brand>()
                .GetAll()
                .FirstOrDefaultAsync(b => b.Id == id);

            if (brand == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy Brand", id.ToString());
            }

           
            brand.Active =false;

          
            await _unitOfWork.Repository<Brand>().Update(brand, id);
            await _unitOfWork.CommitAsync();

          
            var deletedBrandResponse = new BrandResponse
            {
                Id = brand.Id,
                BrandName = brand.BrandName,
                Active= brand.Active,
             
            };

            return deletedBrandResponse;
        }

    }
}

