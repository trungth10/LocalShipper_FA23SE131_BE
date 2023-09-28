using AutoMapper;
using LocalShipper.Data.Models;
using LocalShipper.Data.UnitOfWork;
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

        //Get Single
        public async Task<TransactionResponse> GetTransaction(int id, int orderId)
        {

            var transaction = await _unitOfWork.Repository<Transaction>().GetAll()
                .Include(t => t.Order)
                .Where(t => t.Id == id
                || t.OrderId == orderId)
                .FirstOrDefaultAsync();

            var transactionResponse = new TransactionResponse
            {
                Id = transaction.Id,
                TransactionMethod = transaction.TransactionMethod,
                OrderId = transaction.OrderId,
                WalletId = transaction.WalletId,
                Amount = transaction.Amount,
                TransactionTime = transaction.TransactionTime,
                TransactionDescription = transaction.TransactionDescription,
                CreatedAt = transaction.CreatedAt

            };

            if (transaction.Order != null)
            {
                transactionResponse.Order = new OrderResponse
                {
                    Id = transaction.Order.Id,
                    storeId = transaction.Order.StoreId,
                    status = transaction.Order.Status,
                    batchId = transaction.Order.BatchId,
                    shipperId = transaction.Order.ShipperId,
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
                    other = transaction.Order.Other
                };
            }
            return transactionResponse;
        }


        //Get List
        public async Task<List<TransactionResponse>> GetListTransaction(int? walletId = null, string? transactionMethod = null)
        {
            IQueryable<Transaction> query = _unitOfWork.Repository<Transaction>().GetAll();

            if (walletId.HasValue) 
            {
                query = query.Where(a => a.WalletId == walletId);
            }
            if (!string.IsNullOrEmpty(transactionMethod))
            {
                query = query.Where(a => a.TransactionMethod == transactionMethod);
            }

            var transactions = await query.ToListAsync();
            var transactionsResponse = transactions.Select(transactions => new TransactionResponse
            {
                Id = transactions.Id,
                TransactionMethod = transactions.TransactionMethod,
                OrderId = transactions.OrderId,
                WalletId = transactions.WalletId,
                Amount = transactions.Amount,
                TransactionTime = transactions.TransactionTime,
                TransactionDescription = transactions.TransactionDescription,
                CreatedAt = transactions.CreatedAt
            }).ToList();
            return transactionsResponse;

        }
    }
}



