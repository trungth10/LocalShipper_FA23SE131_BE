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
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Implement
{
    public class TransactionService : ITransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public TransactionService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;

        }

        //CREATE Transaction
        public async Task<TransactionResponse> CreateTransaction(RegisterTransactionRequest request)
        {
            var orderExisted = _unitOfWork.Repository<Transaction>().Find(x => x.OrderId == request.OrderId);

            if (orderExisted != null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Giao dịch không tồn tại", request.OrderId.ToString());
            }
            Transaction transaction = new Transaction
            {
                TransactionMethod = request.TransactionMethod,
                OrderId = request.OrderId,
                WalletId = request.WalletId,
                Amount = request.Amount,
                TransactionTime = request.TransactionTime,
                TransactionDescription = request.TransactionDescription,
                CreatedAt = request.CreatedAt
            };
            await _unitOfWork.Repository<Transaction>().InsertAsync(transaction);
            await _unitOfWork.CommitAsync();

            var createdTransactionResponse = new TransactionResponse
            {
                Id = transaction.Id,
                TransactionMethod = transaction.TransactionMethod,
                OrderId= transaction.OrderId,
                WalletId= transaction.WalletId,
                Amount = transaction.Amount,
                TransactionTime = transaction.TransactionTime,
                TransactionDescription = transaction.TransactionDescription,
                CreatedAt = transaction.CreatedAt,
            };
            return createdTransactionResponse;

        }

        //GET 
        public async Task<List<TransactionResponse>> GetTransaction(int? id, string? transactionMethod, int? orderId, int? walletId)
        {

            var transactions = await _unitOfWork.Repository<Transaction>().GetAll()
                                                              .Where(t => id == 0 || t.Id == id)
                                                              .Where(t => string.IsNullOrWhiteSpace(transactionMethod) || t.TransactionMethod.Contains(transactionMethod))
                                                              .Where(t => orderId == 0 || t.OrderId == orderId)
                                                              .Where(t => walletId == 0 || t.WalletId == walletId)
                                                              .ToListAsync();
            var transactionReponses = transactions.Select(transaction => new TransactionResponse
            {
                Id = transaction.Id,
                TransactionMethod = transaction.TransactionMethod,
                OrderId = transaction.OrderId,
                WalletId = transaction.WalletId,
                Amount = transaction.Amount,
                TransactionTime = transaction.TransactionTime,
                TransactionDescription = transaction.TransactionDescription,
                CreatedAt = transaction.CreatedAt,

            }).ToList();
            return transactionReponses;
        }

        //GET List
        //public async Task<List<TransactionResponse>> GetListTransaction(int? walletId = null, string? transactionMethod = null)
        //{
        //    IQueryable<Transaction> query = _unitOfWork.Repository<Transaction>().GetAll();

        //    if (walletId.HasValue)
        //    {
        //        query = query.Where(a => a.WalletId == walletId);
        //    }
        //    if (!string.IsNullOrEmpty(transactionMethod))
        //    {
        //        query = query.Where(a => a.TransactionMethod == transactionMethod);
        //    }

        //    var transactions = await query.ToListAsync();
        //    var transactionsResponse = transactions.Select(transactions => new TransactionResponse
        //    {
        //        Id = transactions.Id,
        //        TransactionMethod = transactions.TransactionMethod,
        //        OrderId = transactions.OrderId,
        //        WalletId = transactions.WalletId,
        //        Amount = transactions.Amount,
        //        TransactionTime = transactions.TransactionTime,
        //        TransactionDescription = transactions.TransactionDescription,
        //        CreatedAt = transactions.CreatedAt,

        //        Order = transactions.Order != null ? new OrderResponse
        //        {
        //            Id = transactions.Order.Id,
        //            status = transactions.Order.Status,
        //            storeId = transactions.Order.StoreId,
        //            batchId = transactions.Order.BatchId,
        //            shipperId = transactions.Order.ShipperId,
        //            trackingNumber = transactions.Order.TrackingNumber,
        //            createTime = transactions.Order.CreateTime,
        //            orderTime = transactions.Order.OrderTime,
        //            acceptTime = transactions.Order.AcceptTime,
        //            pickupTime = transactions.Order.PickupTime,
        //            cancelTime = transactions.Order.CancelTime,
        //            cancelReason = transactions.Order.CancelReason,
        //            completeTime = transactions.Order.CompleteTime,
        //            distancePrice = transactions.Order.DistancePrice,
        //            subTotalprice = transactions.Order.SubtotalPrice,
        //            totalPrice = transactions.Order.TotalPrice,
        //            other = transactions.Order.Other,
        //        } : null
        //    }).ToList();
        //    return transactionsResponse;

        //}



        //GET Count
        public async Task<int> GetTotalTransactionCount()
        {
            var count = await _unitOfWork.Repository<Transaction>()
                .GetAll()
                .CountAsync();

            return count;
        }

        //UPDATE Transaction
        public async Task<TransactionResponse> UpdateTransaction(int id, PutTransactionRequest transactionRequest)
        {
            var transaction = await _unitOfWork.Repository<Transaction>()
                .GetAll()
                .Include(o => o.Order)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (transaction == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy giao dịch", id.ToString());
            }

            transaction.TransactionMethod = transactionRequest.TransactionMethod;
            transaction.OrderId = transactionRequest.OrderId;
            transaction.WalletId = transactionRequest.WalletId;
            transaction.Amount = transactionRequest.Amount;
            transaction.TransactionDescription = transactionRequest.TransactionDescription;



            await _unitOfWork.Repository<Transaction>().Update(transaction, id);
            await _unitOfWork.CommitAsync();

            var updatedTransactionResponse = new TransactionResponse
            {
                Id = transaction.Id,
                TransactionMethod = transaction.TransactionMethod,
                OrderId = transaction.OrderId,
                WalletId = transaction.WalletId,
                Amount = transaction.Amount,
                TransactionTime = transaction.TransactionTime,
                TransactionDescription = transaction.TransactionDescription,
                CreatedAt = transaction.CreatedAt,

                Order = transaction.Order != null ? new OrderResponse
                {
                    Id = transaction.Order.Id,
                    status = transaction.Order.Status,
                    storeId = transaction.Order.StoreId,
                    batchId = transaction.Order.BatchId,
                    shipperId = (int)transaction.Order.ShipperId,
                    trackingNumber = transaction.Order.TrackingNumber,
                    createTime = transaction.Order.CreateTime,
                    orderTime = transaction.Order.OrderTime,
                    acceptTime = transaction.Order.AcceptTime,
                    pickupTime = transaction.Order.PickupTime,
                    cancelTime = transaction.Order.CancelTime,
                    cancelReason = transaction.Order.CancelReason,
                    completeTime = transaction.Order.CompleteTime,
                    distancePrice = transaction.Order.DistancePrice,
                    subTotalprice = transaction.Order.SubtotalPrice,
                    totalPrice = transaction.Order.TotalPrice,
                    other = transaction.Order.Other,
                } : null
            };

            return updatedTransactionResponse;
        }

        //DELETE Transaction
        public async Task<MessageResponse> DeleteTransaction(int id)
        {

            var transaction = await _unitOfWork.Repository<Transaction>().GetAll()
            .Include(o => o.Order)
            .FirstOrDefaultAsync(a => a.Id == id);

            if (transaction == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy giao dịch", id.ToString());
            }

            _unitOfWork.Repository<Transaction>().Delete(transaction);
            await _unitOfWork.CommitAsync();

            return new MessageResponse
            {
                Message = "Xóa giao dịch thành công",
            };
        }
    }
}



