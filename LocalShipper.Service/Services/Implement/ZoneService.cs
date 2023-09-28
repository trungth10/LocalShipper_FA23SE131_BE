using AutoMapper;
using LocalShipper.Data.Models;
using LocalShipper.Data.UnitOfWork;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Helpers;
using LocalShipper.Service.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
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
        public async Task<List<ZoneResponse>> GetAllZone()
        {
            var zones = await _unitOfWork.Repository<Zone>().GetAll().ToListAsync();
            var zoneResponses = _mapper.Map<List<ZoneResponse>>(zones);
            return zoneResponses;
        }
    }
}
