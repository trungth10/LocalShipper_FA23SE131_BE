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
    public class ShipperService : IShipperService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public ShipperService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;         
        }
        public async Task<ShipperResponse> UpdateShipperStatus(int shipperId, UpdateShipperStatusRequest request)
        {
            try
            {
                var shipper = _unitOfWork.Repository<Shipper>().Find(x => x.Id == shipperId);         

                if (shipper == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy người giao hàng", shipperId.ToString());
                }    
                
                shipper.Status = (int) request.status;

                await _unitOfWork.Repository<Shipper>().Update(shipper, shipperId);
                await _unitOfWork.CommitAsync();

                
                return _mapper.Map<Shipper, ShipperResponse>(shipper);
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Cập nhật trạng thái người giao hàng thất bại", ex.InnerException?.Message);
            }
        }
    }
}
