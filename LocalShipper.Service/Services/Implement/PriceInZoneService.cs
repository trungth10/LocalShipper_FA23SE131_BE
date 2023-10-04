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
    public class PriceInZoneService : IPriceInZoneService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public PriceInZoneService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        //CREATE Price In Zone
        public async Task<PriceInZoneResponse> CreatePriceInZone(RegisterPriceInZoneRequest request)
        {
            PriceInZone priceInZone = new PriceInZone
            {
                PriceId = request.priceId,
                ZoneId = request.zoneId,
            };
            await _unitOfWork.Repository<PriceInZone>().InsertAsync(priceInZone);
            await _unitOfWork.CommitAsync();

            var createdResponse = new PriceInZoneResponse
            {
                Id = priceInZone.Id,
                PriceId = priceInZone.PriceId,
                ZoneId = priceInZone.ZoneId,

            };
            return createdResponse;

        }

        //GET
        public async Task<List<PriceInZoneResponse>> GetPriceInZone(int? id, int? priceId, int? zoneId, int? pageNumber, int? pageSize)
        {
            var priceInZones = _unitOfWork.Repository<PriceInZone>()
                .GetAll()
                .Include(t => t.Price)
                .Include(t => t.Zone)
                .Where(a => (id == null || id == 0) || a.Id == id)
                .Where(a => (priceId == null || priceId == 0) || a.PriceId == priceId)
                .Where(a => (zoneId == null || zoneId == 0) || a.ZoneId == zoneId);


            // Xác định giá trị cuối cùng của pageNumber
            pageNumber = pageNumber.HasValue ? Math.Max(1, pageNumber.Value) : 1;

            // Áp dụng phân trang nếu có thông số pageNumber và pageSize
            if (pageNumber.HasValue && pageSize.HasValue)
            {
                priceInZones = priceInZones.Skip((pageNumber.Value - 1) * pageSize.Value)
                                               .Take(pageSize.Value);
            }

            var priceInZoneList = await priceInZones.ToListAsync();

            if (priceInZoneList == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Danh sách không có hoặc không tồn tại", id.ToString());
            }



            var priceInZoneResponses = priceInZoneList.Select(priceInZone => new PriceInZoneResponse
            {
                Id = priceInZone.Id,
                PriceId = priceInZone.PriceId,
                ZoneId = priceInZone.ZoneId,

                PriceLS = priceInZone.Price != null ? new PriceLSResponse
                {
                    Id = priceInZone.Price.Id,
                    Name = priceInZone.Price.Name,
                    StoreId = priceInZone.Price.StoreId,
                    Hourfilter = priceInZone.Price.Hourfilter,
                    Datefilter = priceInZone.Price.Datefilter,
                    Mode = priceInZone.Price.Mode,
                    Status = priceInZone.Price.Status,
                    Priority = priceInZone.Price.Priority,
                    CreateAt = priceInZone.Price.CreateAt,

                } : null,

                Zone = priceInZone.Zone != null ? new ZoneResponse
                {
                    Id = priceInZone.Zone.Id,
                    ZoneName = priceInZone.Zone.ZoneName,
                    ZoneDescription = priceInZone.Zone.ZoneDescription,
                    Latitude = priceInZone.Zone.Latitude,
                    Longitude = priceInZone.Zone.Longitude,
                    Radius = priceInZone.Zone.Radius,
                    CreatedAt = priceInZone.Zone.CreatedAt,
                    UpdateAt = priceInZone.Zone.UpdateAt,
                    Active = priceInZone.Zone.Active,
                    PriceInZoneId = priceInZone.Zone.PriceInZoneId,
                } : null

            }).ToList();

            return priceInZoneResponses;
        }

        //GET Count
        public async Task<int> GetTotalPriceInZoneCount()
        {
            var count = await _unitOfWork.Repository<PriceInZone>()
                .GetAll()
                .CountAsync();

            return count;
        }

        //UPDATE Price In Zone
        public async Task<PriceInZoneResponse> UpdatePriceInZone(int id, PutPriceInZoneRequest priceInZoneRequest)
        {
            var priceInZone = await _unitOfWork.Repository<PriceInZone>()
                .GetAll()
                .Include(a => a.Zone).Include(a => a.Price)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (priceInZone == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy danh sách bảng giá trong khu vực", id.ToString());
            }

            priceInZone.PriceId = priceInZoneRequest.priceId;
            priceInZone.ZoneId = priceInZoneRequest.zoneId;


            await _unitOfWork.Repository<PriceInZone>().Update(priceInZone, id);
            await _unitOfWork.CommitAsync();

            var updatedPriceInZoneResponse = new PriceInZoneResponse
            {
                Id = priceInZone.Id,
                PriceId = priceInZone.PriceId,
                ZoneId = priceInZone.ZoneId,

                PriceLS = priceInZone.Price != null ? new PriceLSResponse
                {
                    Id = priceInZone.Price.Id,
                    Name = priceInZone.Price.Name,
                    StoreId = priceInZone.Price.StoreId,
                    Hourfilter = priceInZone.Price.Hourfilter,
                    Datefilter = priceInZone.Price.Datefilter,
                    Mode = priceInZone.Price.Mode,
                    Status = priceInZone.Price.Status,
                    Priority = priceInZone.Price.Priority,
                    CreateAt = priceInZone.Price.CreateAt,

                } : null,

                Zone = priceInZone.Zone != null ? new ZoneResponse
                {
                    Id = priceInZone.Zone.Id,
                    ZoneName = priceInZone.Zone.ZoneName,
                    ZoneDescription = priceInZone.Zone.ZoneDescription,
                    Latitude = priceInZone.Zone.Latitude,
                    Longitude = priceInZone.Zone.Longitude,
                    Radius = priceInZone.Zone.Radius,
                    CreatedAt = priceInZone.Zone.CreatedAt,
                    UpdateAt = priceInZone.Zone.UpdateAt,
                    Active = priceInZone.Zone.Active,
                    PriceInZoneId = priceInZone.Zone.PriceInZoneId,
                } : null
            };

            return updatedPriceInZoneResponse;
        }

        //DELETE Price In Zone
        public async Task<MessageResponse> DeletePriceInZone(int id)
        {

            var priceInZone = await _unitOfWork.Repository<PriceInZone>().GetAll()
            .FirstOrDefaultAsync(a => a.Id == id);

            if (priceInZone == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy giá bảng giá trong khu vực", id.ToString());
            }

            _unitOfWork.Repository<PriceInZone>().Delete(priceInZone);
            await _unitOfWork.CommitAsync();

            return new MessageResponse
            {
                id = id,
                Message = "Xóa bảng giá thành công",
            };
        }
    }

}


