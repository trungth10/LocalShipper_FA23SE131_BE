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
    public class StoreService : IStoreService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public StoreService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<StoreResponse>> GetStore(int? id, string? storeName, int? status, int? brandId, int? zoneId, int? walletId, int? accountId)
        {

            var stores = await _unitOfWork.Repository<Store>().GetAll()
                                                              .Where(b => id == 0 || b.Id == id)
                                                              .Where(b => status ==0 || b.Status == status)
                                                              .Where(b => brandId ==0 || b.BrandId == brandId)
                                                              .Where(b => zoneId ==0 || b.ZoneId == zoneId)
                                                              .Where(b => walletId ==0 || b.WalletId == walletId)
                                                              .Where(b => accountId ==0 || b.AccountId == accountId)
                                                              .Where(b => string.IsNullOrWhiteSpace(storeName) || b.StoreName.Contains(storeName))
                                                              .ToListAsync();
            var storeResponses = _mapper.Map<List<Store>, List<StoreResponse>>(stores);

            return storeResponses;
        }
    }
}

