using AutoMapper;
using LocalShipper.Data.Models;
using LocalShipper.Data.UnitOfWork;
using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Exceptions;
using LocalShipper.Service.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Implement
{
    public class PriceService : IPriceLSService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public PriceService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;

        }

        //CREATE Price
        public async Task<PriceLSResponse> CreatePrice(RegisterPriceRequest request, int accountId)
        {
            var account = await _unitOfWork.Repository<Account>()
                .GetAll()
                .FirstOrDefaultAsync(a => a.Id == accountId);

            if (account.RoleId == 2)
            {
                if (request.StoreId != null)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Không được nhập StoreId khi Role là Staff", accountId.ToString());
                }
                else
                {
                    request.StoreId = null; // Force StoreId to be null
                }
            }
            else if (account.RoleId != 4)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Hãy nhập StoreId", accountId.ToString());
            }
            else if (account.RoleId == 4 && request.StoreId == null)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "StoreId không thể để trống khi RoleId là 4", accountId.ToString());
            }

            PriceL price = new PriceL
            {
                Name = request.Name,
                StoreId = request.StoreId,
                Hourfilter = request.Hourfilter,
                Datefilter = request.Datefilter,
                Mode = request.Mode,
                Status = request.Status,
                Priority = request.Priority,
                CreateAt = request.CreateAt
            };

            await _unitOfWork.Repository<PriceL>().InsertAsync(price);
            await _unitOfWork.CommitAsync();

            var createdPriceResponse = new PriceLSResponse
            {
                Id = price.Id,
                Name = price.Name,
                StoreId = price.StoreId,
                Hourfilter = price.Hourfilter,
                Datefilter = price.Datefilter,
                Mode = price.Mode,
                Status = price.Status,
                Priority = price.Priority,
                CreateAt = price.CreateAt
            };

            return createdPriceResponse;
        }

        //GET Price
        public async Task<List<PriceLSResponse>> GetPrice(int? id, string name, int? storeId, int? hourFilter, int? dateFilter, int? mode, int? status, int? priority, int? pageNumber, int? pageSize)
        {
            var prices = _unitOfWork.Repository<PriceL>()
                .GetAll()
                .Include(t => t.Store)
                .Where(a => (id == null || id == 0) || a.Id == id)
                .Where(a => string.IsNullOrWhiteSpace(name) || a.Name.Contains(name))
                .Where(a => (storeId == null || storeId == 0) || a.StoreId == storeId)
                .Where(a => (hourFilter == null || hourFilter == 0) || a.Hourfilter == hourFilter)
                .Where(a => (dateFilter == null || dateFilter == 0) || a.Datefilter == dateFilter)
                .Where(a => (mode == null || mode == 0) || a.Mode == mode)
                .Where(a => status == null || a.Status == status || a.Status == null)
                .Where(a => (priority == null || priority == 0) || a.Priority == priority);


            // Xác định giá trị cuối cùng của pageNumber
            pageNumber = pageNumber.HasValue ? Math.Max(1, pageNumber.Value) : 1;

            // Áp dụng phân trang nếu có thông số pageNumber và pageSize
            if (pageNumber.HasValue && pageSize.HasValue)
            {
                prices = prices.Skip((pageNumber.Value - 1) * pageSize.Value)
                                               .Take(pageSize.Value);
            }

            var priceList = await prices.ToListAsync();

            if (priceList == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Bảng giá không có hoặc không tồn tại", id.ToString());
            }

            var priceResponses = priceList.Select(price => new PriceLSResponse
            {
                Id = price.Id,
                Name = price.Name,
                StoreId = price.StoreId,
                Hourfilter = price.Hourfilter,
                Datefilter = price.Datefilter,
                Mode = price.Mode,
                Status = price.Status,
                Priority = price.Priority,
                CreateAt = price.CreateAt,

                Store = price.Store != null ? new StoreResponse
                {
                    Id = price.Store.Id,
                    StoreName = price.Store.StoreName,
                    StoreAddress = price.Store.StoreAddress,
                    StorePhone = price.Store.StorePhone,
                    StoreEmail = price.Store.StoreEmail,
                    OpenTime = price.Store.OpenTime,
                    CloseTime = price.Store.CloseTime,
                    StoreDescription = price.Store.StoreDescription,
                    Status = price.Store.Status,
                    TemplateId = price.Store.TemplateId,
                    ZoneId = price.Store.ZoneId,
                    WalletId = price.Store.WalletId,
                    AccountId = price.Store.AccountId,

                } : null
            }).ToList();

            return priceResponses;
        }

        //COUNT Total Price

        public async Task<int> GetTotalPriceCount()
        {
            var count = await _unitOfWork.Repository<PriceL>()
                .GetAll()
                .CountAsync();

            return count;
        }



        //UPDATE Price

        public async Task<PriceLSResponse> UpdatePrice(int id, PutPriceRequest priceRequest, int accountId)
        {
            var price = await _unitOfWork.Repository<PriceL>()
                .GetAll().Include(a => a.Store)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (price == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy giá", id.ToString());
            }

            var account = await _unitOfWork.Repository<Account>()
                .GetAll()
                .FirstOrDefaultAsync(a => a.Id == accountId);

            if (account.RoleId == 2)
            {
                if (priceRequest.StoreId != null)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Không thể chỉnh sửa giá này", id.ToString());
                }
            }
            else if (account.RoleId == 4)
            {
                if (priceRequest.StoreId == null)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Không thể chỉnh sửa giá này", id.ToString());
                }
                else if (priceRequest.StoreId == 0)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Không thể chỉnh sửa StoreId thành null", id.ToString());
                }
            }
            else
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Không có quyền chỉnh sửa giá", id.ToString());
            }

            price.Name = priceRequest.Name;
            price.StoreId = priceRequest.StoreId;
            price.Hourfilter = priceRequest.Hourfilter;
            price.Datefilter = priceRequest.Datefilter;
            price.Mode = priceRequest.Mode;
            price.Status = priceRequest.Status;
            price.Priority = priceRequest.Priority;
            price.CreateAt = priceRequest.CreateAt;

            await _unitOfWork.Repository<PriceL>().Update(price, id);
            await _unitOfWork.CommitAsync();

            var updatedPriceResponse = new PriceLSResponse
            {
                Id = price.Id,
                Name = price.Name,
                StoreId = price.StoreId,
                Hourfilter = price.Hourfilter,
                Datefilter = price.Datefilter,
                Mode = price.Mode,
                Status = price.Status,
                Priority = price.Priority,
                CreateAt = price.CreateAt,

                Store = price.Store != null ? new StoreResponse
                {
                    Id = price.Store.Id,
                    StoreName = price.Store.StoreName,
                    StoreAddress = price.Store.StoreAddress,
                    StorePhone = price.Store.StorePhone,
                    StoreEmail = price.Store.StoreEmail,
                    OpenTime = price.Store.OpenTime,
                    CloseTime = price.Store.CloseTime,
                    StoreDescription = price.Store.StoreDescription,
                    Status = price.Store.Status,
                    TemplateId = price.Store.TemplateId,
                    ZoneId = price.Store.ZoneId,
                    WalletId = price.Store.WalletId,
                    AccountId = price.Store.AccountId,
                } : null
            };

            return updatedPriceResponse;
        }



        //DELETE Price
        public async Task<MessageResponse> DeletePrice(int id)
        {

            var price = await _unitOfWork.Repository<PriceL>().GetAll()
            .FirstOrDefaultAsync(a => a.Id == id);

            if (price == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy giá", id.ToString());
            }

            _unitOfWork.Repository<PriceL>().Delete(price);
            await _unitOfWork.CommitAsync();

            return new MessageResponse
            {
                id = id,
                Message = "Xóa giá thành công",
            };
        }
    }

}



