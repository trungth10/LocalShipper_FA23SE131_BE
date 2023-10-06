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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Implement
{
    public class HistoryService : IHistoryService
    {

        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public HistoryService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;

        }
        //CREATE History
        public async Task<HistoryResponse> CreateHistory(RegisterHistoryRequest request)
        {
            var storeIdExisted = _unitOfWork.Repository<History>().Find(x => x.StoreId == request.StoreId);

            if (storeIdExisted != null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Lịch sử không tồn tại", request.StoreId.ToString());
            }
            History history = new History
            {
                Action = request.Action,
                HistoryDescription = request.HistoryDescription,
                StoreId = request.StoreId,
                CreateAt = request.CreateAt,

            };
            await _unitOfWork.Repository<History>().InsertAsync(history);
            await _unitOfWork.CommitAsync();


            var createdHistoryResponse = new HistoryResponse
            {
                Id = history.Id,
                Action = history.Action,
                HistoryDescription = history.HistoryDescription,
                StoreId = history.StoreId,
                CreateAt = history.CreateAt,
            };
            return createdHistoryResponse;

        }

        //GET 
        public async Task<List<HistoryResponse>> GetHistory(int? id, string? action, int? storeId, int? pageNumber, int? pageSize)
        {

            var histories = _unitOfWork.Repository<History>().GetAll()
            .Include(t => t.Store)
            .Where(t => id == 0 || t.Id == id)
            .Where(t => string.IsNullOrWhiteSpace(action) || t.Action.Contains(action.Trim()))
            .Where(t => storeId == 0 || t.StoreId == storeId);

            // Xác định giá trị cuối cùng của pageNumber
            pageNumber = pageNumber.HasValue ? Math.Max(1, pageNumber.Value) : 1;
            // Áp dụng phân trang nếu có thông số pageNumber và pageSize
            if (pageNumber.HasValue && pageSize.HasValue)
            {
                histories = histories.Skip((pageNumber.Value - 1) * pageSize.Value)
                                       .Take(pageSize.Value);
            }

            var historyList = await histories.ToListAsync();
            if (historyList == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Lịch sử không có hoặc không tồn tại", id.ToString());
            }
            var historyResponses = historyList.Select(history => new HistoryResponse
            {
                Id = history.Id,
                Action = history.Action,
                HistoryDescription = history.HistoryDescription,
                StoreId = history.StoreId,
                CreateAt = history.CreateAt,

                Store = history.Store != null ? new StoreResponse
                {
                    Id = history.Store.Id,
                    StoreName = history.Store.StoreName,
                    StoreAddress = history.Store.StoreAddress,
                    StorePhone = history.Store.StorePhone,
                    StoreEmail = history.Store.StoreEmail,
                    OpenTime = history.Store.OpenTime,
                    CloseTime = history.Store.CloseTime,
                    StoreDescription = history.Store.StoreDescription,
                    Status = history.Store.Status,
                    TemplateId = history.Store.TemplateId,
                    ZoneId = history.Store.ZoneId,
                    WalletId = history.Store.WalletId,
                    AccountId = history.Store.AccountId,
                } : null
            }).ToList();
            return historyResponses;
        }


        //Count History
        public async Task<int> GetTotalHistoryCount()
        {
            var count = await _unitOfWork.Repository<History>()
                .GetAll()
                .CountAsync();

            return count;
        }

        //UPDATE History
        public async Task<HistoryResponse> UpdateHistory(int id, PutHistoryRequest historyRequest)
        {
            var history = await _unitOfWork.Repository<History>()
                .GetAll().Include(a => a.Store)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (history == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy lịch sử", id.ToString());
            }


            history.Action = historyRequest.Action;
            history.HistoryDescription = historyRequest.HistoryDescription;
            history.StoreId = historyRequest.StoreId;
            history.CreateAt = historyRequest.CreateAt;

            // Kiểm tra xem StoreId mới từ historyRequest đã tồn tại trong các bản ghi History khác chưa
            if (await _unitOfWork.Repository<History>()
                .GetAll()
                .AnyAsync(a => a.Id != id && a.StoreId == historyRequest.StoreId))
            {
                throw new CrudException(HttpStatusCode.BadRequest, "StoreId đã tồn tại trong lịch sử khác.", id.ToString());
            }

            await _unitOfWork.Repository<History>().Update(history, id);
            await _unitOfWork.CommitAsync();

            var updatedHistoryResponse = new HistoryResponse
            {
                Id = history.Id,
                Action = historyRequest.Action,
                HistoryDescription = historyRequest.HistoryDescription,
                StoreId = historyRequest.StoreId,
                CreateAt = historyRequest.CreateAt,
                Store = history.Store != null ? new StoreResponse
                {
                    Id = history.Store.Id,
                    StoreName = history.Store.StoreName,
                    StoreAddress = history.Store.StoreAddress,
                    StorePhone = history.Store.StorePhone,
                    StoreEmail = history.Store.StoreEmail,
                    OpenTime = history.Store.OpenTime,
                    CloseTime = history.Store.CloseTime,
                    StoreDescription = history.Store.StoreDescription,
                    Status = history.Store.Status,
                    TemplateId = history.Store.TemplateId,
                    ZoneId = history.Store.ZoneId,
                    WalletId = history.Store.WalletId,
                    AccountId = history.Store.AccountId,
                } : null
            };

            return updatedHistoryResponse;
        }

        //DELETE History
        public async Task<MessageResponse> DeleteHistory(int id)
        {

            var history = await _unitOfWork.Repository<History>().GetAll()
            .FirstOrDefaultAsync(a => a.Id == id);

            if (history == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy lịch sử", id.ToString());
            }

            _unitOfWork.Repository<History>().Delete(history);
            await _unitOfWork.CommitAsync();

            return new MessageResponse
            {
                id = id,
                Message = "Xóa lịch sử thành công",
            };
        }
    }
}

