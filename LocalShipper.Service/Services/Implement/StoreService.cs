using AutoMapper;
using LocalShipper.Data.Models;
using LocalShipper.Data.UnitOfWork;
using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Exceptions;
using LocalShipper.Service.Helpers;
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
        public async Task<StoreResponse> CreateStore(StoreRequest request)
        {
            var existingStoreName = await _unitOfWork.Repository<Store>().FindAsync(b => b.StoreName == request.StoreName);
            if (existingStoreName != null)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "StoreName đã tồn tại", request.StoreName);
            } 
            var existingStoreAdd = await _unitOfWork.Repository<Store>().FindAsync(b => b.StoreAddress == request.StoreAddress);
            if (existingStoreAdd != null)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "StoreAddress đã tồn tại", request.StoreAddress);
            } 
            var existingStoreEmail = await _unitOfWork.Repository<Store>().FindAsync(b => b.StoreEmail == request.StoreEmail);
            if (existingStoreEmail != null)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "StoreEmail đã tồn tại", request.StoreEmail);
            }
            var existingStoreAccountId = await _unitOfWork.Repository<Store>().FindAsync(b => b.AccountId == request.AccountId);
            if (existingStoreEmail != null)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "AccountId đã tồn tại", request.AccountId.ToString());
            }
            var newStore = new Store
            {
                StoreName = request.StoreName,
                StoreAddress = request.StoreAddress,
                StorePhone = request.StorePhone,
                StoreEmail = request.StoreEmail,
                OpenTime = request.OpenTime,
                CloseTime = request.CloseTime,
                StoreDescription = request.StoreDescription,
                Status = request.Status ?? 0, 
                BrandId = request.BrandId,
                TemplateId = request.TemplateId,
                ZoneId = request.ZoneId,
                WalletId = request.WalletId,
                AccountId = request.AccountId,
            };

          
            await _unitOfWork.Repository<Store>().InsertAsync(newStore);
            await _unitOfWork.CommitAsync();

           
            var storeResponse = _mapper.Map<StoreResponse>(newStore);
            return storeResponse;
        }

        public async Task<StoreResponse> UpdateStore(int id, StoreRequest storeRequest)
        {
            var store = await _unitOfWork.Repository<Store>()
                .GetAll()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (store == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy cửa hàng", id.ToString());
            }

            store.StoreName = storeRequest.StoreName;
            store.StoreAddress = storeRequest.StoreAddress;
            store.StorePhone = storeRequest.StorePhone;
            store.StoreEmail = storeRequest.StoreEmail;
            store.OpenTime = storeRequest.OpenTime;
            store.CloseTime = storeRequest.CloseTime;
            store.StoreDescription = storeRequest.StoreDescription;
            store.Status = storeRequest.Status ?? 0; 
            store.BrandId = storeRequest.BrandId;
            store.TemplateId = storeRequest.TemplateId;
            store.ZoneId = storeRequest.ZoneId;
            store.WalletId = storeRequest.WalletId;
            store.AccountId = storeRequest.AccountId;
            

          

            await _unitOfWork.Repository<Store>().Update(store, id);
            await _unitOfWork.CommitAsync();

            var updatedStoreResponse = new StoreResponse
            {
                Id = store.Id,
                StoreName = store.StoreName,
                StoreAddress = store.StoreAddress,
                StorePhone = store.StorePhone,
                StoreEmail = store.StoreEmail,
                OpenTime = store.OpenTime,
                CloseTime = store.CloseTime,
                StoreDescription = store.StoreDescription,
                Status = store.Status,
                BrandId = store.BrandId,
                TemplateId = store.TemplateId,
                ZoneId = store.ZoneId,
                WalletId = store.WalletId,
                AccountId= store.AccountId,
                
              
            };

            return updatedStoreResponse;
        }
        public async Task<MessageResponse> DeleteStore(int id)
        {
            var stores = await _unitOfWork.Repository<Store>().GetAll().FindAsync(id);

            if (stores == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy cửa hàng", id.ToString());
            }


            stores.Status = (int)StoreStatusEnum.DELETE;

            await _unitOfWork.Repository<Store>().Update(stores, id);
            await _unitOfWork.CommitAsync();


            

            return new MessageResponse
            {
                Message = "Đã xóa",
            };
        }

    }
}

