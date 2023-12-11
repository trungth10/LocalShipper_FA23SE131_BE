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
using static System.Net.WebRequestMethods;

namespace LocalShipper.Service.Services.Implement
{
    public class StoreService : IStoreService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        private IRouteService _routeService;
        public StoreService(IMapper mapper, IUnitOfWork unitOfWork, IRouteService routeService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _routeService = routeService;
        }

        public async Task<List<StoreResponse>> GetStore(int? id, string? storeName, int? status, int? zoneId, int? walletId, int? accountId, int? pageNumber, int? pageSize)
        {

            var stores = _unitOfWork.Repository<Store>().GetAll().Include(b => b.Wallet).Include(b => b.Account).Include(b => b.Zone).Include(b => b.Template)
                                                              .Where(b => id == 0 || b.Id == id)
                                                              .Where(b => status == 0 || b.Status == status)
                                                              .Where(b => zoneId == 0 || b.ZoneId == zoneId)
                                                              .Where(b => walletId == 0 || b.WalletId == walletId)
                                                              .Where(b => accountId == 0 || b.AccountId == accountId)
                                                              .Where(b => string.IsNullOrWhiteSpace(storeName) || b.StoreName.Contains(storeName.Trim()));

            // Xác định giá trị cuối cùng của pageNumber
            pageNumber = pageNumber.HasValue ? Math.Max(1, pageNumber.Value) : 1;
            // Áp dụng phân trang nếu có thông số pageNumber và pageSize
            if (pageNumber.HasValue && pageSize.HasValue)
            {
                stores = stores.Skip((pageNumber.Value - 1) * pageSize.Value)
                                       .Take(pageSize.Value);
            }

            var storeList = await stores.ToListAsync();
            if (storeList == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Cửa hàng không có hoặc không tồn tại", id.ToString());
            }
            var storeResponses = _mapper.Map<List<Store>, List<StoreResponse>>(storeList);

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
           

             string storeAddress = request.StoreAddress;
             var storeCoordinates = await _routeService.ConvertAddress(storeAddress);

            Account account = new Account
            {
                Fullname = request.StoreName,
                Email = request.StoreEmail,
                Active = true,
                Phone = request.StorePhone,
                RoleId = 4,
                Password = request.Password
            };
            await _unitOfWork.Repository<Account>().InsertAsync(account);
            await _unitOfWork.CommitAsync();


            Wallet wallet = new Wallet
            {
                Balance = 0,
            };

            await _unitOfWork.Repository<Wallet>().InsertAsync(wallet);
            await _unitOfWork.CommitAsync();




            var newStore = new Store
            {
                StoreName = account.Fullname,
                StoreAddress = request.StoreAddress,
                StorePhone = account.Phone,
                StoreEmail = account.Email,
                OpenTime = request.OpenTime,
                CloseTime = request.CloseTime,
                StoreDescription = "Cửa hàng hệ thống LocalShipper HCM",
                Status = 1,
                ZoneId = request.ZoneId,
                WalletId = wallet.Id,
                AccountId=account.Id,
                StoreLat = (float)storeCoordinates.Latitude,
                StoreLng = (float)storeCoordinates.Longitude
        };
         

            await _unitOfWork.Repository<Store>().InsertAsync(newStore);
            await _unitOfWork.CommitAsync();

           

            var storeResponse = _mapper.Map<StoreResponse>(newStore);
            return storeResponse;
        }

        public async Task<StoreResponse> UpdateStore(int id, StoreRequestPut storeRequest)
        {
            var store = await _unitOfWork.Repository<Store>()
                .GetAll().Include(b => b.Wallet).Include(b => b.Account).Include(b => b.Zone).Include(b => b.Template)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (store == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy cửa hàng", id.ToString());
            }

            store.StoreName = storeRequest.StoreName.Trim();
            store.StorePhone = storeRequest.StorePhone.Trim();
            store.OpenTime = storeRequest.OpenTime;
            store.CloseTime = storeRequest.CloseTime; 
       

          

            await _unitOfWork.Repository<Store>().Update(store, id);
            await _unitOfWork.CommitAsync();
          
            var updatedStoreResponse = _mapper.Map<StoreResponse>(store);
            return updatedStoreResponse;
        }

        public async Task<StoreResponse> SetTimeDelivery(int id, StoreRequestTime storeRequest)
        {
            var store = await _unitOfWork.Repository<Store>()
                .GetAll().Include(b => b.Wallet).Include(b => b.Account).Include(b => b.Zone).Include(b => b.Template)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (store == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy cửa hàng", id.ToString());
            }

           store.TimeDelivery = storeRequest.TimeDelivery;

            await _unitOfWork.Repository<Store>().Update(store, id);
            await _unitOfWork.CommitAsync();

            var updatedStoreResponse = _mapper.Map<StoreResponse>(store);
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
        public async Task<int> GetTotalStoreCount()
        {
            var count = await _unitOfWork.Repository<Store>()
                .GetAll()
                .CountAsync();

            return count;
        }
    }
}

