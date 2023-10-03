using AutoMapper;
using LocalShipper.Data.Models;
using LocalShipper.Data.UnitOfWork;
using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Exceptions;
using LocalShipper.Service.Helpers;
using LocalShipper.Service.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Implement
{
    public class WalletService : IWalletService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public WalletService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;            
        }

        //Get Wallet  
        public async Task<List<WalletResponse>> GetWallet(int? id, decimal? balance, int? pageNumber, int? pageSize)
        {

            var wallets = _unitOfWork.Repository<Wallet>().GetAll()
                                                              .Where(a => id == 0 || a.Id == id)
                                                              .Where(a => balance == 0 || a.Balance == balance);
            // Xác định giá trị cuối cùng của pageNumber
            pageNumber = pageNumber.HasValue ? Math.Max(1, pageNumber.Value) : 1;
            // Áp dụng phân trang nếu có thông số pageNumber và pageSize
            if (pageNumber.HasValue && pageSize.HasValue)
            {
                wallets = wallets.Skip((pageNumber.Value - 1) * pageSize.Value)
                                       .Take(pageSize.Value);
            }

            var walletList = await wallets.ToListAsync();
            if (walletList == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Ví không có hoặc không tồn tại", id.ToString());
            }
            var walletResponses = walletList.Select(wallets => new WalletResponse
            {
                Id = wallets.Id,
                Balance = wallets.Balance,
                CreatedAt = wallets.CreatedAt,
                UpdatedAt = wallets.UpdatedAt
            }).ToList();
            return walletResponses;
        }


        //GET Count
        public async Task<int> GetTotalWalletCount()
        {
            var count = await _unitOfWork.Repository<Wallet>()
                .GetAll()
                .CountAsync();

            return count;
        }

        //CREATE Wallet
        public async Task<WalletResponse> CreateWallet(WalletRequest request)
        {
            Wallet wallet = new Wallet
            {
                Balance = request.Balance,
            };
            await _unitOfWork.Repository<Wallet>().InsertAsync(wallet);
            await _unitOfWork.CommitAsync();


            var walletResponse = new WalletResponse
            {
               Id= wallet.Id,
               Balance = wallet.Balance,
               CreatedAt = wallet.CreatedAt,
               UpdatedAt = wallet.UpdatedAt
            };
            return walletResponse;
        }

        //UPDATE Wallet
        public async Task<WalletResponse> UpdateWallet(int id, WalletRequest request)
        {
            var wallet = await _unitOfWork.Repository<Wallet>()
                .GetAll()              
                .FirstOrDefaultAsync(a => a.Id == id);

            if (wallet == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy ví", id.ToString());
            }

            wallet.Balance = request.Balance;
            wallet.UpdatedAt = DateTime.Now;

            await _unitOfWork.Repository<Wallet>().Update(wallet, id);
            await _unitOfWork.CommitAsync();           

            var walletResponse = new WalletResponse
            {
                Id = wallet.Id,
                Balance = wallet.Balance,
                CreatedAt= wallet.CreatedAt,
                UpdatedAt = wallet.UpdatedAt
            };
            return walletResponse;
        }
    }
}
