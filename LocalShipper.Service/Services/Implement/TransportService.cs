using AutoMapper;
using LocalShipper.Data.Models;
using LocalShipper.Data.UnitOfWork;
using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Implement
{
    public class TransportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public TransportService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<TransportResponse> AddTransport(TransportRequest request)
        {
            var licencePlate = _unitOfWork.Repository<Transport>().Find(x => x.LicencePlate == request.licence_plate);

            if (licencePlate != null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Xe đã đã tồn tại", request.licence_plate.ToString());
            }

            Transport transport = new Transport
            {
                TypeId = request.typeId,
                LicencePlate = request.licence_plate,
                TransportColor = request.color,
                TransportImage = request.transport_image,
                TransportRegistration = request.transport_registration
            };
            await _unitOfWork.Repository<Transport>().InsertAsync(transport);
            await _unitOfWork.CommitAsync();

            var transportNew = await _unitOfWork.Repository<Transport>().GetAll().Include(o => o.Type).FirstOrDefaultAsync();
            var transportResponse = new TransportResponse
            {
                Id = transportNew.Id,
                LicencePlate = transportNew.LicencePlate,
                TransportColor = transportNew.TransportColor,
                TransportImage = transportNew.TransportImage,
                TransportRegistration = transportNew.TransportRegistration
            };
            if(transportNew.Type != null)
            {
                transportResponse.Type = new TransportTypeResponse
                {
                    Id = transportNew.Type.Id,
                    TransportType1 = transportNew.Type.TransportType1,
                    CreateAt = transportNew.Type.CreateAt
                };
            }
            return transportResponse;
        }
    }
}
