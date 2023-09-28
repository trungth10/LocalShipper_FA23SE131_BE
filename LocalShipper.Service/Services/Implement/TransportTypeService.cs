using AutoMapper;
using LocalShipper.Data.Models;
using LocalShipper.Data.UnitOfWork;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Implement
{
    public class TransportTypeService : ITransportTypeService
    {

        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public TransportTypeService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<TransportTypeResponse>> GetAll() 
        {
            var transportTypes = await _unitOfWork.Repository<TransportType>().GetAll().ToListAsync();

            var transportTypeResponses = _mapper.Map<List<TransportTypeResponse>>(transportTypes);
            return transportTypeResponses;

        }

    }
}
