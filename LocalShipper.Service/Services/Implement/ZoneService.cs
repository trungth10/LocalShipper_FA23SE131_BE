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
    public class ZoneService : IZoneService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public ZoneService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }


        //Get Zone   
        public async Task<List<ZoneResponse>> GetZones(int? id, string? zoneName, decimal? latitude, decimal? longtitude, decimal? radius, int? pageNumber, int? pageSize)
        {

            var zones = _unitOfWork.Repository<Zone>().GetAll()
                                                              .Where(a => id == 0 || a.Id == id)
                                                              .Where(a => string.IsNullOrWhiteSpace(zoneName) || a.ZoneName.Contains(zoneName.Trim()))
                                                              .Where(a => latitude == 0 || a.Latitude == latitude)
                                                              .Where(a => longtitude == 0 || a.Longitude == longtitude)
                                                              .Where(a => radius == 0 || a.Radius == radius);

            // Xác định giá trị cuối cùng của pageNumber
            pageNumber = pageNumber.HasValue ? Math.Max(1, pageNumber.Value) : 1;
            // Áp dụng phân trang nếu có thông số pageNumber và pageSize
            if (pageNumber.HasValue && pageSize.HasValue)
            {
                zones = zones.Skip((pageNumber.Value - 1) * pageSize.Value)
                                       .Take(pageSize.Value);
            }

            var zoneList = await zones.ToListAsync();
            if (zoneList == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Khu vực không có hoặc không tồn tại", id.ToString());
            }
            var zoneResponse = zoneList.Select(zones => new ZoneResponse
            {
               Id = zones.Id,
               ZoneName = zones.ZoneName,
               ZoneDescription = zones.ZoneDescription,
               Latitude = zones.Latitude,
               Longitude = zones.Longitude,
               Radius = zones.Radius,
               CreatedAt = zones.CreatedAt,
               UpdateAt = zones.UpdateAt,
               Active = zones.Active
            }).ToList();
            return zoneResponse;
        }
        
        //Get count
        public async Task<int> GetTotalZoneCount()
        {
            var count = await _unitOfWork.Repository<Zone>()
                .GetAll()               
                .CountAsync();
            return count;
        }

        //CREATE Zone
        public async Task<ZoneResponse> CreateZone(ZoneRequest request)
        {
            Zone zone = new Zone
            {
                ZoneName = request.ZoneName,
                ZoneDescription = request.ZoneDescription,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Radius = request.Radius,
                Active = true
            };
            await _unitOfWork.Repository<Zone>().InsertAsync(zone);
            await _unitOfWork.CommitAsync();


            var zoneResponse = new ZoneResponse
            {
                Id = zone.Id,
                ZoneName = zone.ZoneName,
                ZoneDescription = zone.ZoneDescription,
                Latitude = zone.Latitude,
                Longitude = zone.Longitude,
                Radius = zone.Radius,
                CreatedAt = zone.CreatedAt,
                UpdateAt = zone.UpdateAt,
                Active = zone.Active
            };
            return zoneResponse;
        }

        //UPDATE Zone
        public async Task<ZoneResponse> UpdateZone(int id, ZoneRequest request)
        {
            var zone = await _unitOfWork.Repository<Zone>()
                .GetAll()
                .FirstOrDefaultAsync(a => a.Id == id);

            if (zone == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy khu vực", id.ToString());
            }

            zone.ZoneName = request.ZoneName;
            zone.ZoneDescription = request.ZoneDescription;
            zone.Latitude = request.Latitude;
            zone.Longitude = request.Longitude;
            zone.Radius = request.Radius;
            zone.UpdateAt = DateTime.Now;

            await _unitOfWork.Repository<Zone>().Update(zone, id);
            await _unitOfWork.CommitAsync();

            var zoneResponse = new ZoneResponse
            {
                Id = zone.Id,
                ZoneName = zone.ZoneName,
                ZoneDescription = zone.ZoneDescription,
                Latitude = zone.Latitude,
                Longitude = zone.Longitude,
                Radius = zone.Radius,
                CreatedAt = zone.CreatedAt,
                UpdateAt = zone.UpdateAt,
                Active = zone.Active
            };
            return zoneResponse;
        }

        //Delete Zone
        public async Task<ZoneResponse> DeleteZone(int id)
        {
            var zone = await _unitOfWork.Repository<Zone>()
                .GetAll()
                .FirstOrDefaultAsync(a => a.Id == id);

            if (zone == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy khu vực", id.ToString());
            }

            zone.Active = false;

            await _unitOfWork.Repository<Zone>().Update(zone, id);
            await _unitOfWork.CommitAsync();

            var zoneResponse = new ZoneResponse
            {
                Id = zone.Id,
                ZoneName = zone.ZoneName,
                ZoneDescription = zone.ZoneDescription,
                Latitude = zone.Latitude,
                Longitude = zone.Longitude,
                Radius = zone.Radius,
                CreatedAt = zone.CreatedAt,
                UpdateAt = zone.UpdateAt,
                Active = zone.Active
            };
            return zoneResponse;
        }
    }
}
