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
            decimal? amount, string? description, int? orderId,int? active, int? pageNumber, int? pageSize)
        {

            var walletTrans = _unitOfWork.Repository<WalletTransaction>().GetAll()
                                                              .Include(w => w.FromWallet)
                                                              .Include(w => w.ToWallet)
                                                              .Include(w => w.Order)
                                                              .Where(w => id == 0 || w.Id == id)
                                                              .Where(w => string.IsNullOrWhiteSpace(transactionType) || w.TransactionType.Contains(transactionType.Trim()))
                                                              .Where(w => fromWallet == 0 || w.FromWalletId == fromWallet)
                                                              .Where(w => toWallet == 0 || w.ToWalletId == toWallet)
                                                              .Where(w => amount == 0 || w.Amount == amount)
                                                              .Where(w => string.IsNullOrWhiteSpace(description) || w.Description.Contains(description.Trim()))
                                                              .Where(w => orderId == 0 || w.OrderId == orderId)
                                                              .Where(w => active == 0 || w.Active == active)
                                                              ;
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
            var walletTransResponses = new List<WalletTransactionResponse>();
            foreach (var item in walletTransList)
            {
                var transReponse = _mapper.Map<WalletTransactionResponse>(item);
                if (item.FromWallet.ShipperId == null)
                {
                    var storeSender = await _unitOfWork.Repository<Store>()
                  .GetAll()
                  .FirstOrDefaultAsync(a => a.WalletId == item.FromWalletId);
                    transReponse.Sender = storeSender.StoreName;
                }
                if (item.ToWallet.ShipperId == null)
                {
                    var storeReceiver = await _unitOfWork.Repository<Store>()
                 .GetAll()
                 .FirstOrDefaultAsync(a => a.WalletId == item.ToWalletId);
                    transReponse.Receiver = storeReceiver.StoreName;
                }
                if (item.ToWallet.ShipperId != null)
                {
                    var shipperReceiver = await _unitOfWork.Repository<Wallet>()
                 .GetAll()
                 .Include(a => a.Shipper)
                 .FirstOrDefaultAsync(a => a.Id == item.ToWalletId);
                    transReponse.Receiver = shipperReceiver.Shipper.FullName;
                }
                if (item.FromWallet.ShipperId != null)
                {
                    var shipperSender = await _unitOfWork.Repository<Wallet>()
                .GetAll()
                .Include(a => a.Shipper)
                .FirstOrDefaultAsync(a => a.Id == item.FromWalletId);
                    transReponse.Sender = shipperSender.Shipper.FullName;
                }

                walletTransResponses.Add(transReponse);
            }
            
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
            var fromWalletCheck = await _unitOfWork.Repository<Wallet>().FindAsync(x => x.Id == request.FromWalletId);
            if (fromWalletCheck == null)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Không tìm thấy ví gửi", request.FromWalletId.ToString());
            }

            var toWalletCheck = await _unitOfWork.Repository<Wallet>().FindAsync(x => x.Id == request.ToWalletId);
            if (toWalletCheck == null)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Không tìm thấy ví nhận", request.ToWalletId.ToString());
            }


            if (((fromWalletCheck.Type == 1 && (toWalletCheck.Type == 2 || toWalletCheck.Type == 3)) ||
               ((fromWalletCheck.Type == 2 || fromWalletCheck.Type == 3) && toWalletCheck.Type == 1))
               &&!(fromWalletCheck.Type == 3 && toWalletCheck.Type == 2)
               &&!(fromWalletCheck.Type == 2 && toWalletCheck.Type == 3))
            {
                if (fromWalletCheck.Type == 1 && fromWalletCheck.Balance <= 100000)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "không thể chuyển nếu ví này còn dưới 100.000", "");
                }
                if (fromWalletCheck.Type == 3 && (DateTime.UtcNow - fromWalletCheck.CreatedAt.GetValueOrDefault()).Days < 30)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "ví này phải có thời gian là 30 ngày kể từ lúc kích hoạt mới để được chuyển", "");
                }

                WalletTransaction walletTrans = new WalletTransaction
                {
                  
                    FromWalletId = request.FromWalletId,
                    ToWalletId = request.ToWalletId,
                    Amount = request.Amount,                   
                    Active = 1
                };

                if (fromWalletCheck.Type == (int)WalletTypeEnum.VICHINH && toWalletCheck.Type == (int)WalletTypeEnum.VITHUHO)
                {
                    walletTrans.TransactionType = "Chuyển tiền";
                    walletTrans.Description = "Chuyển tiền từ Ví chính sang Ví thu hộ";
                }
                if (fromWalletCheck.Type == (int)WalletTypeEnum.VICHINH && toWalletCheck.Type == (int)WalletTypeEnum.VIKICHHOAT)
                {
                    
                    walletTrans.TransactionType = "Chuyển tiền";
                    walletTrans.Description = "Chuyển tiền từ Ví chính sang Ví kích hoạt";
                }
              
                if (fromWalletCheck.Type == (int)WalletTypeEnum.VITHUHO && toWalletCheck.Type == (int)WalletTypeEnum.VICHINH)
                {
                    walletTrans.TransactionType = "Chuyển tiền";
                    walletTrans.Description = "Chuyển tiền từ Ví thu hộ sang Ví Chính";
                }
                if (fromWalletCheck.Type == (int)WalletTypeEnum.VIKICHHOAT && toWalletCheck.Type == (int)WalletTypeEnum.VITHUHO)
                {                  
                    walletTrans.TransactionType = "Chuyển tiền";
                    walletTrans.Description = "Chuyển tiền từ Ví kích hoạt sang Ví thu hộ";
                }


                // Update ví gửi
                var fromWallet = await _unitOfWork.Repository<Wallet>()
                   .GetAll()
                   .FirstOrDefaultAsync(a => a.Id == request.FromWalletId);

                if (fromWallet.Balance < request.Amount)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Không đủ tiền để thực hiện giao dịch", request.ToWalletId.ToString());
                }

                fromWallet.Balance -= request.Amount;
                fromWallet.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.Repository<Wallet>().Update(fromWallet, request.FromWalletId);
                await _unitOfWork.CommitAsync();

                // Update ví nhận
                var toWallet = await _unitOfWork.Repository<Wallet>()
                   .GetAll()
                   .FirstOrDefaultAsync(a => a.Id == request.ToWalletId);
                
                toWallet.Balance += request.Amount;
                toWallet.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.Repository<Wallet>().Update(toWallet, request.ToWalletId);
                await _unitOfWork.CommitAsync();

                await _unitOfWork.Repository<WalletTransaction>().InsertAsync(walletTrans);
                await _unitOfWork.CommitAsync();

                var walletTransResponses = _mapper.Map<WalletTransactionResponse>(walletTrans);
                return walletTransResponses;
            }
            else
            {
                throw new CrudException(HttpStatusCode.BadRequest, "loại ví giao dịch không phù hợp", "");
            }
        }


        //UPDATE WalletTransaction
        public async Task<WalletTransactionResponse> UpdateWalletTrans(int id, WalletTransactionRequestUpdate request)
        {
            var walletTrans = await _unitOfWork.Repository<WalletTransaction>()
                .GetAll()
                .Include(o => o.FromWallet)
                .Include(o => o.ToWallet)
                .Include(o => o.Order)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (walletTrans == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy giao dịch", id.ToString());
            }

            walletTrans.TransactionType = request.TransactionType;
            walletTrans.Description = request.Description;
            walletTrans.Active = request.Active;

            await _unitOfWork.Repository<WalletTransaction>().Update(walletTrans, id);
            await _unitOfWork.CommitAsync();

            var walletTransResponses = _mapper.Map<WalletTransactionResponse>(walletTrans);
            return walletTransResponses;
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
