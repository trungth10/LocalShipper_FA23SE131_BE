using AutoMapper;
using LocalShipper.Data.Models;
using LocalShipper.Data.UnitOfWork;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Exceptions;
using LocalShipper.Service.Helpers;
using LocalShipper.Service.Services.Interface;
using MailKit.Search;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Implement
{
    public class TransportService : ITransportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public TransportService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;

        }


        //Get Single
        public async Task<TransportResponse> GetTransport(int id, string licencePlate)
        {

            var transport = await _unitOfWork.Repository<Transport>().GetAll()
                .Include(t => t.Type)
                .Where(t => t.Id == id
                || t.LicencePlate == licencePlate)
                .FirstOrDefaultAsync();

            var transportResponse = new TransportResponse
            {
                Id = transport.Id,
                TypeId = transport.TypeId,
                LicencePlate = transport.LicencePlate,
                TransportColor = transport.TransportColor,
                TransportImage = transport.TransportImage,
                TransportRegistration = transport.TransportRegistration
            };

            if (transport.Type != null)
            {
                transportResponse.TransportType = new TransportTypeResponse
                {
                    Id = transport.Type.Id,
                    TransportType1 = transport.Type.TransportType1,
                    CreateAt = transport.Type.CreateAt
                };
            }
            return transportResponse;
        }


        //Get List
        public async Task<List<TransportResponse>> GetListTransport(int? typeId = null, string? transportColor = null)
        {
            IQueryable<Transport> query = _unitOfWork.Repository<Transport>().GetAll();


            if (typeId.HasValue)
            {
                query = query.Where(a => a.TypeId == typeId);
            }
            if (!string.IsNullOrEmpty(transportColor))
            {
                query = query.Where(a => a.TransportColor == transportColor);
            }

            var transports = await query.ToListAsync();

            var transportsResponse = transports.Select(transports => new TransportResponse
            {
                Id = transports.Id,
                TypeId = transports.TypeId,
                LicencePlate = transports.LicencePlate,
                TransportColor = transports.TransportColor,
                TransportImage = transports.TransportImage,
                TransportRegistration = transports.TransportRegistration
            }).ToList();
            return transportsResponse;

        }
    }
}
