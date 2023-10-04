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
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Implement
{
    public class PriceItemService : IPriceItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public PriceItemService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        //CREATE Price Item
        public async Task<PriceItemResponse> CreatePriceItem(RegisterPriceItemRequest request)
        {
            PriceItem priceItem = new PriceItem
            {
                MinDistance = request.MinDistance,
                MaxDistance = request.MaxDistance,
                MinAmount = request.MinAmount,
                MaxAmount = request.MaxAmount,
                Price = request.Price,
                ApplyFrom = request.ApplyFrom,
                ApplyTo = request.ApplyTo,
                PriceId = request.PriceId,
            };
            await _unitOfWork.Repository<PriceItem>().InsertAsync(priceItem);
            await _unitOfWork.CommitAsync();

            var createdResponse = new PriceItemResponse
            {
                Id = priceItem.Id,
                MinDistance = priceItem.MinDistance,
                MaxDistance = priceItem.MaxDistance,
                MinAmount = priceItem.MinAmount,
                MaxAmount = priceItem.MaxAmount,
                Price = priceItem.Price,
                ApplyFrom = priceItem.ApplyFrom,
                ApplyTo = priceItem.ApplyTo,
                PriceId = priceItem.PriceId,
            };
            return createdResponse;

        }

        //GET
        public async Task<List<PriceItemResponse>> GetPriceItem(int? id, decimal? minAmount,
            decimal? maxAmount, decimal? price, int? pageNumber, int? pageSize)
        {
            var priceItems = _unitOfWork.Repository<PriceItem>()
                .GetAll()
                .Include(t => t.PriceNavigation)
                .Where(a => (id == null || id == 0) || a.Id == id)
                .Where(a => (minAmount == null || minAmount == 0) || a.MinAmount == minAmount)
                .Where(a => (maxAmount == null || maxAmount == 0) || a.MaxAmount == maxAmount)
                .Where(a => (price == null || price == 0) || a.Price == price);


            // Xác định giá trị cuối cùng của pageNumber
            pageNumber = pageNumber.HasValue ? Math.Max(1, pageNumber.Value) : 1;

            // Áp dụng phân trang nếu có thông số pageNumber và pageSize
            if (pageNumber.HasValue && pageSize.HasValue)
            {
                priceItems = priceItems.Skip((pageNumber.Value - 1) * pageSize.Value)
                                               .Take(pageSize.Value);
            }

            var priceItemList = await priceItems.ToListAsync();

            if (priceItemList == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Giá item không có hoặc không tồn tại", id.ToString());
            }

            var priceItemResponses = priceItemList.Select(priceItem => new PriceItemResponse
            {
                Id = priceItem.Id,
                MinDistance = priceItem.MinDistance,
                MaxDistance = priceItem.MaxDistance,
                MinAmount = priceItem.MinAmount,
                MaxAmount = priceItem.MaxAmount,
                Price = priceItem.Price,
                ApplyFrom = priceItem.ApplyFrom,
                ApplyTo = priceItem.ApplyTo,
                PriceId = priceItem.PriceId,

                PriceLS = priceItem.PriceNavigation != null ? new PriceLSResponse
                {
                    Id = priceItem.PriceNavigation.Id,
                    Name = priceItem.PriceNavigation.Name,
                    StoreId = priceItem.PriceNavigation.StoreId,
                    Hourfilter = priceItem.PriceNavigation.Hourfilter,
                    Datefilter = priceItem.PriceNavigation.Datefilter,
                    Mode = priceItem.PriceNavigation.Mode,
                    Status = priceItem.PriceNavigation.Status,
                    Priority = priceItem.PriceNavigation.Priority,
                    CreateAt = priceItem.PriceNavigation.CreateAt,

                } : null
            }).ToList();

            return priceItemResponses;
        }

        //GET Count
        public async Task<int> GetTotalPriceItemCount()
        {
            var count = await _unitOfWork.Repository<PriceItem>()
                .GetAll()
                .CountAsync();

            return count;
        }

        //UPDATE Price Item
        public async Task<PriceItemResponse> UpdatePriceItem(int id, PutPriceItemRequest priceItemRequest)
        {
            var priceItem = await _unitOfWork.Repository<PriceItem>()
                .GetAll()
                .Include(a => a.PriceNavigation)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (priceItem == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy giá item", id.ToString());
            }

            priceItem.MinDistance = priceItemRequest.MinDistance;
            priceItem.MaxDistance = priceItemRequest.MaxDistance;
            priceItem.MinAmount = priceItemRequest.MinAmount;
            priceItem.MaxAmount = priceItemRequest.MaxAmount;
            priceItem.Price = priceItemRequest.Price;
            priceItem.ApplyFrom = priceItemRequest.ApplyFrom;
            priceItem.ApplyTo = priceItemRequest.ApplyTo;
            priceItem.PriceId = priceItemRequest.PriceId;

            await _unitOfWork.Repository<PriceItem>().Update(priceItem, id);
            await _unitOfWork.CommitAsync();

            var updatedPriceItemResponse = new PriceItemResponse
            {
                Id = priceItem.Id,
                MinDistance = priceItem.MinDistance,
                MaxDistance = priceItem.MaxDistance,
                MinAmount = priceItem.MinAmount,
                MaxAmount = priceItem.MaxAmount,
                Price = priceItem.Price,
                ApplyFrom = priceItem.ApplyFrom,
                ApplyTo = priceItem.ApplyTo,
                PriceId = priceItem.PriceId,

                PriceLS = priceItem.PriceNavigation != null ? new PriceLSResponse
                {
                    Id = priceItem.PriceNavigation.Id,
                    Name = priceItem.PriceNavigation.Name,
                    StoreId = priceItem.PriceNavigation.StoreId,
                    Hourfilter = priceItem.PriceNavigation.Hourfilter,
                    Datefilter = priceItem.PriceNavigation.Datefilter,
                    Mode = priceItem.PriceNavigation.Mode,
                    Status = priceItem.PriceNavigation.Status,
                    Priority = priceItem.PriceNavigation.Priority,
                    CreateAt = priceItem.PriceNavigation.CreateAt,

                } : null
            };

            return updatedPriceItemResponse;
        }

        //DELETE Price Item
        public async Task<MessageResponse> DeletePriceItem(int id)
        {

            var priceItem = await _unitOfWork.Repository<PriceItem>().GetAll()
            .FirstOrDefaultAsync(a => a.Id == id);

            if (priceItem == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy giá item", id.ToString());
            }

            _unitOfWork.Repository<PriceItem>().Delete(priceItem);
            await _unitOfWork.CommitAsync();

            return new MessageResponse
            {
                id = id,
                Message = "Xóa giá item thành công",
            };
        }
    }

}


