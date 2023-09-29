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
using TransportType = LocalShipper.Data.Models.TransportType;

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
        //CREATE TransportType
        public async Task<TransportTypeResponse> CreateTransportType(RegisterTransportTypeRequest request)
        {
            TransportType transportType = new TransportType
            {
                TransportType1 = request.TransportType1,
                CreateAt = request.CreateAt
            };
            await _unitOfWork.Repository<TransportType>().InsertAsync(transportType);
            await _unitOfWork.CommitAsync();

            var createdTransportTypeResponse = new TransportTypeResponse
            {
                Id = transportType.Id,
                TransportType1 = transportType.TransportType1,
                CreateAt = transportType.CreateAt
            };
            return createdTransportTypeResponse;

        }

        //GET
        public async Task<List<TransportTypeResponse>> GetTransportType(int? id, string? transportType)
        {

            var transportTypes = await _unitOfWork.Repository<TransportType>().GetAll()
                                                              .Where(b => id == 0 || b.Id == id)
                                                              .Where(b => string.IsNullOrWhiteSpace(transportType) || b.TransportType1.Contains(transportType))
                                                              .ToListAsync();
            var transportTypeResponses = transportTypes.Select(transportType => new TransportTypeResponse
            {
                Id = transportType.Id,
                TransportType1 = transportType.TransportType1,
                CreateAt = transportType.CreateAt,
            }).ToList();
            return transportTypeResponses;
        }

        //GET Count
        public async Task<int> GetTotalTransportTypeCount()
        {
            var count = await _unitOfWork.Repository<TransportType>()
                .GetAll()
                .CountAsync();

            return count;
        }

        //UPDATE Transport Type
        public async Task<TransportTypeResponse> UpdateTransportType(int id, PutTransportTypeRequest transportTypeRequest)
        {
            var transportType = await _unitOfWork.Repository<TransportType>()
                .GetAll()
                .FirstOrDefaultAsync(a => a.Id == id);

            if (transportType == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy phương tiện", id.ToString());
            }

            transportType.TransportType1 = transportTypeRequest.TransportType;
            transportType.CreateAt = transportTypeRequest.CreateAt;



            await _unitOfWork.Repository<TransportType>().Update(transportType, id);
            await _unitOfWork.CommitAsync();

            var updatedTransportResponse = new TransportTypeResponse
            {
                Id = transportType.Id,
                TransportType1 = transportType.TransportType1,
                CreateAt = transportType.CreateAt,
            };

            return updatedTransportResponse;
        }

        //DELETE TransportType
        public async Task<MessageResponse> DeleteTransportType(int id)
        {

            var transportType = await _unitOfWork.Repository<TransportType>().GetAll()
            .FirstOrDefaultAsync(a => a.Id == id);

            if (transportType == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy phương tiện", id.ToString());
            }

            _unitOfWork.Repository<TransportType>().Delete(transportType);
            await _unitOfWork.CommitAsync();

            return new MessageResponse
            {
                Message = "Xóa phương tiện thành công",
            };
        }
    }

}


