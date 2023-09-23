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
        public async Task<BrandResponse> DeleteBrand(int id)
        {
            var account = _unitOfWork.Repository<Account>().GetAll().Where(x => x.Id == id && x.Active == true).ToList();
            if (account.Count != 0)
            {
                throw new CrudException(HttpStatusCode.Conflict, "Brand has account active!!!", "");
            }
            else
            {
                var brand = _unitOfWork.Repository<Brand>().GetAll().Where(x => x.Id == id && x.Active == true).FirstOrDefault();
                if (brand != null)
                {
                    brand.Active = false;
                    try
                    {
                        await _unitOfWork.Repository<Brand>().Update(brand, id);
                        await _unitOfWork.CommitAsync();
                        return new BrandResponse
                        {
                            Id = brand.Id,
                            BrandName = brand.BrandName,
                            BrandDescription = brand.BrandDescription,
                            CreatedAt = DateTime.Now,
                            IconUrl = brand.IconUrl,
                            ImageUrl = brand.ImageUrl,
                            Active = brand.Active,
                            AccountId = brand.AccountId
                        };
                    }
                    catch (Exception e)
                    {
                        throw new CrudException(HttpStatusCode.BadRequest, "Delete Brand Error!!!", e.InnerException?.Message);
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<BrandResponse> GetBrandByID(int id)
        {
            var brand = await _unitOfWork.Repository<Brand>().GetAll().Where(x => x.Id == id && x.Active == true).FirstOrDefaultAsync();
            if (brand != null)
            {
                return new BrandResponse
                {
                    Id = brand.Id,
                    BrandName = brand.BrandName,
                    IconUrl = brand.IconUrl,
                    ImageUrl = brand.ImageUrl,
                    Active = brand.Active,
                    AccountId = brand.AccountId
                };
            }
            else
            {
                return null;
            }
        }

        public async Task<List<BrandResponse>> GetBrands(BrandPagingRequest request)
        {
            List<BrandResponse> list = null;
            try
            {
                List<Brand> brands = null;
                if (request.AccountId != 0)
                {
                    brands = await _unitOfWork.Repository<Brand>()
                           .GetAll()
                               .Where(x => x.BrandName.ToLower()
                           .Contains(request.KeySearch.ToLower())
                           && x.Active == true && x.AccountId == request.AccountId).ToListAsync();
                }
                else
                {
                    brands = await _unitOfWork.Repository<Brand>()
                           .GetAll()
                    .Where(x => x.BrandName.ToLower()
                    .Contains(request.KeySearch.ToLower())
                           && x.Active == true).ToListAsync();
                }
                IEnumerable<BrandResponse> rs = brands.Select(x => new BrandResponse
                {
                    Id = x.Id,
                    BrandName = x.BrandName,
                    IconUrl = x.IconUrl,
                    ImageUrl = x.ImageUrl,
                    Active = x.Active,
                    AccountId = x.AccountId
                }).AsEnumerable();

                list = PageHelper<BrandResponse>.Sorting((SortType.SortOrder)request.SortType, rs, request.ColName);
                return list;
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get Brands Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<PagedResults<BrandResponse>> GetBrandsPaging(BrandPagingRequest request)
        {
            List<BrandResponse> list = null;
            try
            {
                List<Brand> brands = null;
                if (request.AccountId != 0)
                {
                    brands = await _unitOfWork.Repository<Brand>()
                           .GetAll()
                               .Where(x => x.BrandName.ToLower()
                           .Contains(request.KeySearch.ToLower())
                           && x.Active == true && x.AccountId == request.AccountId).ToListAsync();
                }
                else
                {
                    brands = await _unitOfWork.Repository<Brand>()
                           .GetAll()
                    .Where(x => x.BrandName.ToLower()
                    .Contains(request.KeySearch.ToLower())
                           && x.Active == true).ToListAsync();
                }
                IEnumerable<BrandResponse> rs = brands.Select(x => new BrandResponse
                {
                    Id = x.Id,
                    BrandName = x.BrandName,
                    IconUrl = x.IconUrl,
                    ImageUrl = x.ImageUrl,
                    Active = x.Active,
                    AccountId = x.AccountId
                }).AsEnumerable();

                list = PageHelper<BrandResponse>.Sorting((SortType.SortOrder)request.SortType, rs, request.ColName);
                var result = PageHelper<BrandResponse>.Paging(list, request.Page, request.PageSize);
                return result;
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get Brands Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<BrandResponse> PostBrand(PostBrandRequest model, int role)
        {
            Brand brand = new Brand
            {
                BrandName = model.BrandName,
                Active = true,

                AccountId = model.AccountId
            };
            brand.ImageUrl = model.ImageUrl ?? "";
            brand.IconUrl = model.IconUrl ?? "";
            brand.AccountId = model.AccountId > 0 ? model.AccountId : 0;
            try
            {
                await _unitOfWork.Repository<Brand>().InsertAsync(brand);
                await _unitOfWork.CommitAsync();
                brand = await _unitOfWork.Repository<Brand>().GetAll().Where(x => x.Id == brand.Id).Include(x => x.Account).SingleOrDefaultAsync();
                return new BrandResponse
                {
                    Id = brand.Id,
                    BrandName = brand.BrandName,
                    IconUrl = brand.IconUrl,
                    ImageUrl = brand.ImageUrl,
                    Active = brand.Active,
                    AccountId = brand.AccountId
                };
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Insert Brand Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<BrandResponse> PutBrand(int id, PostBrandRequest model)
        {
            var brand = _unitOfWork.Repository<Brand>().GetAll().Where(x => x.Id == id && x.Active == true).FirstOrDefault();
            if (brand != null)
            {
                try
                {
                    brand.BrandName = model.BrandName;
                    brand.AccountId = model.AccountId;
                    if (String.IsNullOrEmpty(model.ImageUrl))
                    {
                        brand.ImageUrl = model.ImageUrl;
                    }

                    if (String.IsNullOrEmpty(model.IconUrl))
                    {
                        brand.IconUrl = model.IconUrl;
                    }

                    if (model.AccountId > 0)
                    {
                        brand.AccountId = model.AccountId;
                    }

                    await _unitOfWork.Repository<Brand>().Update(brand, id);
                    await _unitOfWork.CommitAsync();
                    return new BrandResponse
                    {
                        Id = brand.Id,
                        BrandName = brand.BrandName,
                        IconUrl = brand.IconUrl,
                        ImageUrl = brand.ImageUrl,
                        Active = brand.Active,
                        AccountId = brand.AccountId
                    };
                }
                catch (Exception e)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Update Brand Error!!!", e.InnerException?.Message);
                }
            }
            else
            {
                return null;
            }
        }
    }
}

