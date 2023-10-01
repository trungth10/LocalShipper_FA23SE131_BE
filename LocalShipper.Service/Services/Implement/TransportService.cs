using AutoMapper;
using LocalShipper.Data.Models;
using LocalShipper.Data.UnitOfWork;
using LocalShipper.Service.DTOs.Request;
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
using System.Security.Principal;
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

        //CREATE Transport
        public async Task<TransportResponse> CreateTransport(RegisterTransportRequest request)
        {
            var licencePlateExisted = _unitOfWork.Repository<Transport>().Find(x => x.LicencePlate == request.LicencePlate);

            if (licencePlateExisted != null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Biển số xe không tồn tại", request.LicencePlate.ToString());
            }
            Transport transport = new Transport
            {
                TypeId = request.TypeId,
                LicencePlate = request.LicencePlate,
                TransportColor = request.TransportColor,
                TransportImage = request.TransportImage,
                TransportRegistration = request.TransportRegistration,
                Active = request.Active
            };
            await _unitOfWork.Repository<Transport>().InsertAsync(transport);
            await _unitOfWork.CommitAsync();

            var createdTransportResponse = new TransportResponse
            {
                Id = transport.Id,
                TypeId = transport.TypeId,
                LicencePlate = transport.LicencePlate,
                TransportColor = transport.TransportColor,
                TransportImage = transport.TransportImage,
                TransportRegistration = transport.TransportRegistration,
                Active = (bool)transport.Active,
            };
            return createdTransportResponse;

        }
        //GET
        public async Task<List<TransportResponse>> GetTransport(int? id, int? typeId, string? licencePlate, string? transportColor,
                                                string? transportImage, string? transportRegistration)
        {

            var transports = await _unitOfWork.Repository<Transport>().GetAll()
                                                              .Where(t => id == 0 || t.Id == id)
                                                              .Where(t => id == 0 || t.TypeId == typeId)
                                                              .Where(t => string.IsNullOrWhiteSpace(licencePlate) || t.LicencePlate.Contains(licencePlate))
                                                              .Where(t => string.IsNullOrWhiteSpace(transportColor) || t.TransportColor.Contains(transportColor))
                                                              .Where(t => string.IsNullOrWhiteSpace(transportImage) || t.TransportImage.Contains(transportImage))
                                                              .Where(t => string.IsNullOrWhiteSpace(transportRegistration) || t.TransportRegistration.Contains(transportRegistration))
                                                              
                                                              .ToListAsync();
            var transportResponses = transports.Select(transport => new TransportResponse
            {
                Id = transport.Id,
                TypeId = transport.TypeId,
                LicencePlate = transport.LicencePlate,
                TransportColor = transport.TransportColor,
                TransportImage = transport.TransportImage,
                TransportRegistration = transport.TransportRegistration,
                Active= (bool)transport.Active,

            }).ToList();
            return transportResponses;
        }



        //GET Single
        //public async Task<TransportResponse> GetTransport(int id, string licencePlate)
        //{

        //    var transports = await _unitOfWork.Repository<Transport>().GetAll()
        //        .Include(t => t.Type)
        //        .Where(t => t.Id == id
        //        || t.LicencePlate == licencePlate)
        //        .FirstOrDefaultAsync();

        //    var transportResponse = new TransportResponse
        //    {
        //        Id = transports.Id,
        //        TypeId = transports.TypeId,
        //        LicencePlate = transports.LicencePlate,
        //        TransportColor = transports.TransportColor,
        //        TransportImage = transports.TransportImage,
        //        TransportRegistration = transports.TransportRegistration
        //    };

        //    if (transports.Type != null)
        //    {
        //        transportResponse.TransportType = new TransportTypeResponse
        //        {
        //            Id = transports.Type.Id,
        //            TransportType1 = transports.Type.TransportType1,
        //            CreateAt = transports.Type.CreateAt
        //        };
        //    }
        //    return transportResponse;
        //}




        //GET List
        //public async Task<List<TransportResponse>> GetListTransport(int? typeId = null, string? transportColor = null)
        //{
        //    IQueryable<Transport> query = _unitOfWork.Repository<Transport>().GetAll();


        //    if (typeId.HasValue)
        //    {
        //        query = query.Where(a => a.TypeId == typeId);
        //    }
        //    if (!string.IsNullOrEmpty(transportColor))
        //    {
        //        query = query.Where(a => a.TransportColor == transportColor);
        //    }

        //    var transports = await query.ToListAsync();

        //    var transportsResponse = transports.Select(transports => new TransportResponse
        //    {
        //        Id = transports.Id,
        //        TypeId = transports.TypeId,
        //        LicencePlate = transports.LicencePlate,
        //        TransportColor = transports.TransportColor,
        //        TransportImage = transports.TransportImage,
        //        TransportRegistration = transports.TransportRegistration,

        //        TransportType = transports.Type != null ? new TransportTypeResponse
        //        {
        //            Id = transports.Type.Id,
        //            TransportType1 = transports.Type.TransportType1,
        //            CreateAt = transports.Type.CreateAt,

        //        } : null

        //    }).ToList();
        //    return transportsResponse;
        //}



        //GET Count
        public async Task<int> GetTotalTransportCount()
        {
            var count = await _unitOfWork.Repository<Transport>()
                .GetAll()
                .CountAsync();

            return count;
        }

        //UPDATE Transport
        public async Task<TransportResponse> UpdateTransport(int id, PutTransportRequest transportRequest)
        {


            var transport = await _unitOfWork.Repository<Transport>()
                .GetAll()
                .Include(o => o.Type)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (transport == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy phương tiện", id.ToString());
            }

            transport.TypeId = transportRequest.TypeId;
            transport.LicencePlate = transportRequest.LicencePlate;
            transport.TransportColor = transportRequest.TransportColor;
            transport.TransportImage = transportRequest.TransportImage;
            transport.TransportRegistration = transportRequest.TransportRegistration;
            transport.Active = transportRequest.Active;
            // Kiểm tra xem có phương tiện khác sử dụng LicencePlate mới không
            var existingTransportWithSameLicencePlate = await _unitOfWork.Repository<Transport>()
                .GetAll()
                .FirstOrDefaultAsync(a => a.LicencePlate == transportRequest.LicencePlate && a.Id != id);

            if (existingTransportWithSameLicencePlate != null)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Biển số xe đã tồn tại cho một phương tiện khác.", id.ToString());
            }

            await _unitOfWork.Repository<Transport>().Update(transport, id);
            await _unitOfWork.CommitAsync();

            var updatedTransportResponse = new TransportResponse
            {
                Id = transport.Id,
                TypeId = transportRequest.TypeId,
                LicencePlate = transportRequest.LicencePlate,
                TransportColor = transportRequest.TransportColor,
                TransportImage = transportRequest.TransportImage,
                TransportRegistration = transportRequest.TransportRegistration,
                Active = transportRequest.Active,

                TransportType = transport.Type != null ? new TransportTypeResponse
                {
                    Id = transport.Type.Id,
                    TransportType1 = transport.Type.TransportType1,
                } : null
            };

            return updatedTransportResponse;
        }

        //DELETE Transport
        public async Task<MessageResponse> DeleteTransport(int id)
        {

            var transport = await _unitOfWork.Repository<Transport>().GetAll()
            .Include(o => o.Type)
            .FirstOrDefaultAsync(a => a.Id == id);

            if (transport == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy phương tiện", id.ToString());
            }

            transport.Active = false; // Đặt giá trị Active thành false


            await _unitOfWork.CommitAsync(); // Lưu thay đổi vào cơ sở dữ liệu

            return new MessageResponse
            {
                Message = "Vô hiệu hóa phương tiện thành công",
            };
        }
    }
}