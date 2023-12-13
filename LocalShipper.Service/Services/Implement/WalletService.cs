using AutoMapper;
using LocalShipper.Data.Models;
using LocalShipper.Data.UnitOfWork;
using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Exceptions;
using LocalShipper.Service.Helpers;
using LocalShipper.Service.Helpers.Momo;
using LocalShipper.Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Implement
{
    public class WalletService : IWalletService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly IOptions<MomoOptionModel> _options;
        private IAccountService _accountService;


        public WalletService(IMapper mapper, IUnitOfWork unitOfWork, IOptions<MomoOptionModel> options, IAccountService accountService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _options = options;
           _accountService = accountService;
        }

        //Get Wallet  
        public async Task<List<WalletResponse>> GetWallet(int? id, decimal? balance,int? shipperId, int? type, int? pageNumber, int? pageSize)
        {

            var wallets = _unitOfWork.Repository<Wallet>().GetAll()
                                                              .Where(a => id == 0 || a.Id == id)
                                                              .Where(a => balance == 0 || a.Balance == balance)
                                                              .Where(a => shipperId == 0 || a.ShipperId == shipperId)
                                                              .Where(a => type ==0 || a.Type == type);
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
                UpdatedAt = wallets.UpdatedAt,
                ShipperId = wallets.ShipperId,
                Type = wallets.Type,
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
                Type= request.Type,
                ShipperId = request.ShipperId
            };

            if (wallet.Type == 1 && wallet.Balance < 100000)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Số dư của loại ví Type 1 phải là ít nhất 100,000", "");
            }
            else if (wallet.Type == 3 && wallet.Balance < 300000)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Số dư của loại ví Type 3 phải là ít nhất 300,000", "");
            }
            else if (wallet.Type == 2 && wallet.Balance <= 0)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Số dư của loại ví Type 2 phải là số dương", "");
            }
            await _unitOfWork.Repository<Wallet>().InsertAsync(wallet);
            await _unitOfWork.CommitAsync();


            var walletResponse = new WalletResponse
            {
               Id= wallet.Id,
               Balance = wallet.Balance,
               CreatedAt = wallet.CreatedAt,
               UpdatedAt = wallet.UpdatedAt,
               ShipperId = wallet.ShipperId,
               Type= wallet.Type,
               
            };
            return walletResponse;
        }

        //UPDATE Wallet
        public async Task<WalletResponse> UpdateWallet(string email, decimal balance, string? OTP, int type)
        {
            var account = await _unitOfWork.Repository<Account>()
                .GetAll()
                .FirstOrDefaultAsync(a => a.Email == email);

            var walletQuery =  _unitOfWork.Repository<Wallet>()
                .GetAll().Include(a => a.Shipper);

            Wallet wallet = null;
            var shippers = await _unitOfWork.Repository<Shipper>()
                .GetAll()
                .FirstOrDefaultAsync(a => a.EmailShipper == email);
            var stores = await _unitOfWork.Repository<Store>()
                .GetAll()
                .FirstOrDefaultAsync(a => a.StoreEmail == email);
            if (account.RoleId == 5)
            {
                wallet = await walletQuery.FirstOrDefaultAsync(a => a.ShipperId == shippers.Id && a.Type == 1);
            }

            if (account.RoleId == 4)
            {
                wallet = await walletQuery.FirstOrDefaultAsync(a => a.Id == stores.WalletId);
            }

            var shipper = await _unitOfWork.Repository<Shipper>()
               .GetAll()
               .FirstOrDefaultAsync(a => a.Id == wallet.ShipperId);

           

            if (wallet == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy ví", email);
            }
            if (type == 1)
            {             
                    wallet.Balance = wallet.Balance + balance;
                    wallet.UpdatedAt = DateTime.Now;                    
            }
            if (type == 2)
            {
                
                if (OTP.Equals(shipper.Fcmtoken))
                {
                    if (balance > wallet.Balance)
                    {
                        throw new CrudException(HttpStatusCode.NotFound, "Không đủ số dư để thực hiện giao dịch", email);
                    }
                    wallet.Balance = wallet.Balance - balance;
                    wallet.UpdatedAt = DateTime.Now;
                }
                else
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Nhập sai mã OTP", shipper.Id.ToString());
                }
            }

            await _unitOfWork.Repository<Wallet>().Update(wallet, wallet.Id);
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

        public async Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(WalletTransactionPayment model)
        {
            model.Id = DateTime.UtcNow.Ticks.ToString();
            model.Description = "Khách hàng: " + model.Name + ". Nội dung: " + model.Description;
            var rawData =
                $"partnerCode={_options.Value.PartnerCode}&accessKey={_options.Value.AccessKey}&requestId={model.Id}&amount={model.Amount}&orderId={model.Id}&orderInfo={model.Description}&returnUrl={_options.Value.ReturnUrl}&notifyUrl={_options.Value.NotifyUrl}&extraData=";

            var signature = ComputeHmacSha256(rawData, _options.Value.SecretKey);

            var client = new RestClient(_options.Value.MomoApiUrl);
            var request = new RestRequest() { Method = Method.Post };
            request.AddHeader("Content-Type", "application/json; charset=UTF-8");

            // Create an object representing the request data
            var requestData = new
            {
                accessKey = _options.Value.AccessKey,
                partnerCode = _options.Value.PartnerCode,
                requestType = _options.Value.RequestType,
                notifyUrl = _options.Value.NotifyUrl,
                returnUrl = _options.Value.ReturnUrl,
                orderId = model.Id,
                amount = model.Amount.ToString(),
                orderInfo = model.Description,
                requestId = model.Id,
                extraData = "",
                signature = signature
            };

            request.AddParameter("application/json", JsonConvert.SerializeObject(requestData), ParameterType.RequestBody);

            var response = await client.ExecuteAsync(request);

            return JsonConvert.DeserializeObject<MomoCreatePaymentResponseModel>(response.Content);
        }

        public MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection)
        {
            var amount = collection.First(s => s.Key == "amount").Value;
            var orderInfo = collection.First(s => s.Key == "orderInfo").Value;
            var orderId = collection.First(s => s.Key == "orderId").Value;
            return new MomoExecuteResponseModel()
            {
                Amount = amount,
                OrderId = orderId,
                OrderInfo = orderInfo
            };
        }

        private string ComputeHmacSha256(string message, string secretKey)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (secretKey == null)
            {
                throw new ArgumentNullException(nameof(secretKey));
            }

            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            byte[] hashBytes;

            using (var hmac = new HMACSHA256(keyBytes))
            {
                hashBytes = hmac.ComputeHash(messageBytes);
            }

            var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            return hashString;
        }

    }
}
