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
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Implement
{
    public class WalletTransactionService : IWalletTransactionService
    {

        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public WalletTransactionService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        //GET WalletTransaction
        public async Task<List<WalletTransactionResponse>> GetWalletTrans(int? id, string? transactionType, int? fromWallet, int? toWallet,
            decimal? amount, int? pageNumber, int? pageSize)
        {

            var walletTrans = _unitOfWork.Repository<WalletTransaction>().GetAll()
                                                              .Include(w => w.FromWallet)
                                                              .Include(w => w.ToWallet)
                                                              .Where(w => id == 0 || w.Id == id)
                                                              .Where(w => string.IsNullOrWhiteSpace(transactionType) || w.TransactionType.Contains(transactionType.Trim()))
                                                              .Where(w => fromWallet == 0 || w.FromWalletId == fromWallet)
                                                              .Where(w => toWallet == 0 || w.ToWalletId == toWallet)
                                                              .Where(w => amount == 0 || w.Amount == amount);
            // Xác định giá trị cuối cùng của pageNumber
            pageNumber = pageNumber.HasValue ? Math.Max(1, pageNumber.Value) : 1;
            // Áp dụng phân trang nếu có thông số pageNumber và pageSize
            if (pageNumber.HasValue && pageSize.HasValue)
            {
                walletTrans = walletTrans.Skip((pageNumber.Value - 1) * pageSize.Value)
                                       .Take(pageSize.Value);
            }

            var walletTransList = await walletTrans.ToListAsync();
            if (walletTransList == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Giao dịch trên ví không có hoặc không tồn tại", id.ToString());
            }

            var walletTransResponses = walletTransList.Select(walletTransaction => new WalletTransactionResponse
            {
                Id = walletTransaction.Id,
                TransactionType = walletTransaction.TransactionType,
                FromWalletId = walletTransaction.FromWalletId,
                ToWalletId = walletTransaction.ToWalletId,
                Amount = walletTransaction.Amount,
                TransactionTime = walletTransaction.TransactionTime,
                Description = walletTransaction.Description,
                CreatedAt = walletTransaction.CreatedAt,

                FromWallet = walletTransaction.FromWallet != null ? new WalletResponse
                {
                    Id = walletTransaction.FromWallet.Id,
                    Balance = walletTransaction.FromWallet.Balance,
                    CreatedAt = walletTransaction.FromWallet.CreatedAt,
                    UpdatedAt = walletTransaction.FromWallet.UpdatedAt
                } : null,
                ToWallet = walletTransaction.ToWallet != null ? new WalletResponse
                {
                    Id = walletTransaction.ToWallet.Id,
                    Balance = walletTransaction.ToWallet.Balance,
                    CreatedAt = walletTransaction.ToWallet.CreatedAt,
                    UpdatedAt = walletTransaction.ToWallet.UpdatedAt
                } : null
               
            }).ToList();
            return walletTransResponses;
        }
    
        
        //GET count
        public async Task<int> GetTotalWalletTransCount()
        {
            var count = await _unitOfWork.Repository<WalletTransaction>()
                .GetAll()
                .CountAsync();

            return count;
        }

        //CREATE Wallet Transaction
        public async Task<WalletTransactionResponse> CreateWalletTrans(WalletTransactionRequest request)
        {

            var fronWalletCheck = await _unitOfWork.Repository<Wallet>().FindAsync(x => x.Id == request.FromWalletId);
            if (fronWalletCheck == null)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Không tìm thấy ví gửi", request.FromWalletId.ToString());
            }
            var toWalletCheck = await _unitOfWork.Repository<Wallet>().FindAsync(x => x.Id == request.ToWalletId);
            if (toWalletCheck == null)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Không tìm thấy ví nhận", request.ToWalletId.ToString());
            }

            WalletTransaction walletTrans = new WalletTransaction
            {
                TransactionType = request.TransactionType,
                FromWalletId = request.FromWalletId,
                ToWalletId = request.ToWalletId,
                Amount = request.Amount,
                Description = request.Description,               
            };

            //Update ví gửi
            var fromWallet = await _unitOfWork.Repository<Wallet>()
               .GetAll()
               .FirstOrDefaultAsync(a => a.Id == request.FromWalletId);

            if (fromWallet.Balance < request.Amount)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Không đủ tiền để thực hiện giao dịch", request.ToWalletId.ToString());
            }

            fromWallet.Balance = fromWallet.Balance - request.Amount;
            fromWallet.UpdatedAt = DateTime.Now;

            await _unitOfWork.Repository<Wallet>().Update(fromWallet, request.FromWalletId);
            await _unitOfWork.CommitAsync();
            //Update ví nhận
            var toWallet = await _unitOfWork.Repository<Wallet>()
               .GetAll()
               .FirstOrDefaultAsync(a => a.Id == request.ToWalletId);

            toWallet.Balance = toWallet.Balance + request.Amount;
            toWallet.UpdatedAt = DateTime.Now;

            await _unitOfWork.Repository<Wallet>().Update(toWallet, request.ToWalletId);
            await _unitOfWork.CommitAsync();


            await _unitOfWork.Repository<WalletTransaction>().InsertAsync(walletTrans);
            await _unitOfWork.CommitAsync();


            var walletTransResponse = new WalletTransactionResponse
            {
                Id = walletTrans.Id,
                TransactionType = walletTrans.TransactionType,
                FromWalletId = walletTrans.FromWalletId,
                ToWalletId = walletTrans.ToWalletId,
                Amount = walletTrans.Amount,
                TransactionTime = walletTrans.TransactionTime,
                Description = walletTrans.Description,
                CreatedAt = walletTrans.CreatedAt,

                FromWallet = walletTrans.FromWallet != null ? new WalletResponse
                {
                    Id = walletTrans.FromWallet.Id,
                    Balance = walletTrans.FromWallet.Balance,
                    CreatedAt = walletTrans.FromWallet.CreatedAt,
                    UpdatedAt = walletTrans.FromWallet.UpdatedAt
                } : null,
                ToWallet = walletTrans.ToWallet != null ? new WalletResponse
                {
                    Id = walletTrans.ToWallet.Id,
                    Balance = walletTrans.ToWallet.Balance,
                    CreatedAt = walletTrans.ToWallet.CreatedAt,
                    UpdatedAt = walletTrans.ToWallet.UpdatedAt
                } : null
            };
            return walletTransResponse;
        }

        //UPDATE WalletTransaction
        public async Task<WalletTransactionResponse> UpdateWalletTrans(int id, WalletTransactionRequest request)
        {
            var walletTrans = await _unitOfWork.Repository<WalletTransaction>()
                .GetAll()
                .Include(o => o.FromWallet)
                .Include(o => o.ToWallet)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (walletTrans == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy giao dịch", id.ToString());
            }

            walletTrans.TransactionType = request.TransactionType;
            walletTrans.Description = request.Description;

            await _unitOfWork.Repository<WalletTransaction>().Update(walletTrans, id);
            await _unitOfWork.CommitAsync();

            var walletTransResponse = new WalletTransactionResponse
            {
                Id = walletTrans.Id,
                TransactionType = walletTrans.TransactionType,
                FromWalletId = walletTrans.FromWalletId,
                ToWalletId = walletTrans.ToWalletId,
                Amount = walletTrans.Amount,
                TransactionTime = walletTrans.TransactionTime,
                Description = walletTrans.Description,
                CreatedAt = walletTrans.CreatedAt,

                FromWallet = walletTrans.FromWallet != null ? new WalletResponse
                {
                    Id = walletTrans.FromWallet.Id,
                    Balance = walletTrans.FromWallet.Balance,
                    CreatedAt = walletTrans.FromWallet.CreatedAt,
                    UpdatedAt = walletTrans.FromWallet.UpdatedAt
                } : null,
                ToWallet = walletTrans.ToWallet != null ? new WalletResponse
                {
                    Id = walletTrans.ToWallet.Id,
                    Balance = walletTrans.ToWallet.Balance,
                    CreatedAt = walletTrans.ToWallet.CreatedAt,
                    UpdatedAt = walletTrans.ToWallet.UpdatedAt
                } : null
            };
            return walletTransResponse;
        }

        //DELETE WalletTransaction

        public async Task<MessageResponse> DeleteWalletTrans(int id)
        {
            var walletTrans = await _unitOfWork.Repository<WalletTransaction>().GetAll()
            .Include(o => o.FromWallet)
            .Include(o => o.ToWallet)
            .FirstOrDefaultAsync(a => a.Id == id);

            if (walletTrans == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy giao dịch", id.ToString());
            }

            _unitOfWork.Repository<WalletTransaction>().Delete(walletTrans);
            await _unitOfWork.CommitAsync();

            return new MessageResponse
            {
                Message = "Xóa giao dịch thành công",
            };
        }
    }
}
