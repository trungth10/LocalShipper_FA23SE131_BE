using AutoMapper;
using LocalShipper.Data.Models;
using LocalShipper.Data.UnitOfWork;
using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Exceptions;
using LocalShipper.Service.Helpers;
using LocalShipper.Service.Services.Interface;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using static System.Net.WebRequestMethods;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;
using MailKit.Search;
using System.IO;
using Firebase.Auth;
using Microsoft.AspNetCore.Hosting;
using System.Threading;
using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Client;

namespace LocalShipper.Service.Services.Implement
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _env;
        private static string apiKey = "AIzaSyDClgoRpinIEXy6Ho_Upk2mwoKtSc3Kl2M";
        private static string bucket = "localshipper-fc4a5.appspot.com";
        private static string authEmail = "tranbacong311@gmail.com";
        private static string authPassword = "lai1nguoinua";


        public AccountService(IMapper mapper, IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager, IEmailService emailService, IWebHostEnvironment env)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _emailService = emailService;
            _env = env;
        }

     



        //Create Account Shipper
        public async Task<AccountResponse> RegisterShipperAccount(RegisterRequest request)
        {          

            var emailExisted = _unitOfWork.Repository<Account>().Find(x => x.Email == request.Email);

            if (emailExisted != null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Email đã tồn tại", request.Email.ToString());
            }
            //string otp = GenerateOTP();
            // CreatePasswordHash(request.Password, out string passwordHash, salt);
          
            Account account = new Account
            {
                Fullname = request.FullName,
                Email = request.Email,
                Active = false,
                Phone = request.Phone,
              
                // FcmToken = otp,
                RoleId = request.RoleId,
                Password = request.Password,
            };

            await _unitOfWork.Repository<Account>().InsertAsync(account);
            await _unitOfWork.CommitAsync();


            //await SendOTPEmail(request.Email, otp);

            return new AccountResponse
            {
                Id = account.Id,
                Fullname = account.Fullname,
                Phone = account.Phone,
                Email = account.Email,
                RoleId = account.RoleId,
                Active = account.Active,
                FcmToken = account.FcmToken,
                CreateDate = account.CreateDate,
                ImageUrl = account.ImageUrl,
            };
        }
        public async Task<string> UploadImageToFirebase(int accountId, IFormFile image)
        {

            try
            {

                var account = await _unitOfWork.Repository<Account>().GetAll().FirstOrDefaultAsync(a => a.Id == accountId);

                if (account == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Tài khoản không tồn tại", accountId.ToString());
                }

             
                if (image == null || image.Length == 0)
                {
                    throw new Exception("Không có tệp tin nào được chọn hoặc tệp tin rỗng.");
                }

             
                string fileName = Path.GetFileName(image.FileName);
                string folderName = "avatars"; 
                if (_env != null)
                {
                    string path = Path.Combine(_env.WebRootPath, $"images/{folderName}");
                

                   
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                   
                    using (var fileStream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                    {
                        await image.CopyToAsync(fileStream);
                    }
                }
                else
                {
                    Console.WriteLine("Error: HostingEnvironment is null.");
                  
                }


              
                string imagePath = $"{folderName}/{fileName}";

               
                var auth = new FirebaseAuthProvider(new FirebaseConfig(apiKey));
                var authResult = await auth.SignInWithEmailAndPasswordAsync(authEmail, authPassword);

               
                var cancellation = new CancellationTokenSource();
                var firebaseStorage = new FirebaseStorage(
                    bucket,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(authResult.FirebaseToken),
                        ThrowOnCancel = true
                    });

                var uploadTask = firebaseStorage.Child("images").Child(accountId.ToString()).Child(fileName).PutAsync(image.OpenReadStream(), cancellation.Token);
                var imageUrl = await uploadTask;

                
                account.ImageUrl = imageUrl;

                await _unitOfWork.Repository<Account>().Update(account, accountId);
                await _unitOfWork.CommitAsync();
 

                return imageUrl;
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Error uploading image: {ex.Message}");
                throw;
            }
        }
        
        /*private void CreatePasswordHash(string password, out string passwordHash, byte[] salt)
        {
            using (var hmac = new HMACSHA512(salt))
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = hmac.ComputeHash(passwordBytes);

                passwordHash = Convert.ToBase64String(hashBytes);
            }
        }
        private byte[] GenerateSalt(int length = 16)
        {
            byte[] salt = new byte[length];

            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            return salt;
        }*/
        public async Task<bool> SendOTPAgain (string email)
        {
            var account = _unitOfWork.Repository<Account>().Find(x => x.Email == email);

            if (account == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Email không tồn tại", email.ToString());        
            }
            
            string otp = GenerateOTP();
            account.FcmToken = otp;
            _unitOfWork.Repository<Account>().Update(account, account.Id);
            await _unitOfWork.CommitAsync();
            await SendOTPEmail(email, otp);
            return true;
        }

        public string GenerateOTP()
        {
            Random random = new Random();
            int otp = random.Next(100000, 999999);
            return otp.ToString();
        }

        public async Task SendOTPEmail(string email, string otp)
        {
            string subject = "Xác thực tài khoản";
            string content = $"Mã OTP của bạn là: {otp}";

            var message = new Message(new List<string> { email }, subject, content);
            _emailService.SendEmail(message);
        }


        

        public async Task<bool> VerifyOTP(string email, string otp)
        {
            var account = _unitOfWork.Repository<Account>().Find(x => x.Email == email);
            if (account.FcmToken != otp)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Mã xác thực không chính xác", email.ToString());
            }
            if (account != null)
            {
                account.Active = true;
                _unitOfWork.Repository<Account>().Update(account, account.Id);
                await _unitOfWork.CommitAsync();
                return true;
            }

            return false;
        }

        //Get Account  
        public async Task<List<AccountResponse>> GetAccount(int? id, string? phone, string? email, int? role, string? fcm_token, int? pageNumber, int? pageSize)
        {

            var accounts = _unitOfWork.Repository<Account>().GetAll()
                                                              .Include(o => o.Role)
                                                              .Where(a => id == 0 || a.Id == id)
                                                              .Where(a => string.IsNullOrWhiteSpace(phone) || a.Phone.Contains(phone.Trim()))
                                                              .Where(a => string.IsNullOrWhiteSpace(email) || a.Email.Contains(email.Trim()))
                                                              .Where(a => role == 0 || a.RoleId == role)
                                                              .Where(a => string.IsNullOrWhiteSpace(fcm_token) || a.FcmToken.Contains(fcm_token.Trim()));

            // Xác định giá trị cuối cùng của pageNumber
            pageNumber = pageNumber.HasValue ? Math.Max(1, pageNumber.Value) : 1;
            // Áp dụng phân trang nếu có thông số pageNumber và pageSize
            if (pageNumber.HasValue && pageSize.HasValue)
            {
                accounts = accounts.Skip((pageNumber.Value - 1) * pageSize.Value)
                                       .Take(pageSize.Value);
            }

            var accountList = await accounts.ToListAsync();
            if (accountList == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Tài khoản không có hoặc không tồn tại", id.ToString());
            }

            var accountResponses = accountList.Select(account => new AccountResponse
            {
                Id = account.Id,
                Fullname = account.Fullname,
                Phone = account.Phone,
                Email = account.Email,
                RoleId = account.RoleId,
                Active = account.Active,
                FcmToken = account.FcmToken,
                CreateDate = account.CreateDate,
                ImageUrl = account.ImageUrl,
                Password = account.Password,
                Role = account.Role != null ? new RoleResponse
                {
                    Id = account.Role.Id,
                    Name = account.Role.Name,
                } : null
            }).ToList();
            return accountResponses;
        }
         

        //GET Count
        public async Task<int> GetTotalAccountCount()
        {
            var count = await _unitOfWork.Repository<Account>()
                .GetAll()
                .CountAsync();

            return count;
        }

        //UPDATE Account
        public async Task<AccountResponse> UpdateAccount(int id, AccountRequest accountRequest)
        {
            var accounts = await _unitOfWork.Repository<Account>()
                .GetAll()
                .Include(o => o.Role)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (accounts == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy tài khoản", id.ToString());
            }

                accounts.Fullname = accountRequest.FullName;
                accounts.Email = accountRequest.Email;                     
                accounts.Phone = accountRequest.Phone;            
                accounts.Password = accountRequest.Password;        
                accounts.RoleId = (int)accountRequest.RoleId;           
                accounts.ImageUrl = accountRequest.ImageUrl;                
                accounts.FcmToken = accountRequest.Fcm_token;  
                accounts.Active = accountRequest.Active;

            await _unitOfWork.Repository<Account>().Update(accounts, id);
            await _unitOfWork.CommitAsync();

            var account = await _unitOfWork.Repository<Account>()
                 .GetAll()
                 .Include(o => o.Role)
                 .FirstOrDefaultAsync(a => a.Id == id);



            var updatedAccountResponse = new AccountResponse
            {
                Id = account.Id,
                Fullname = account.Fullname,
                Phone = account.Phone,
                Email = account.Email,
                RoleId = account.RoleId,
                Active = account.Active,
                FcmToken = account.FcmToken,
                CreateDate = account.CreateDate,
                ImageUrl = account.ImageUrl,
                Password = account.Password,
                Role = account.Role != null ? new RoleResponse
                {
                    Id = account.Role.Id,
                    Name = account.Role.Name,
                } : null
            };

            return updatedAccountResponse;
        }

        //DELETE Account
        public async Task<AccountResponse> DeleteAccount(int id)
        {

            var account = await _unitOfWork.Repository<Account>()
            .GetAll()
            .Include(o => o.Role)
            .FirstOrDefaultAsync(a => a.Id == id);

            if (account == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy tài khoản", id.ToString());
            }

            if (account.Active == false)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Tài khoản đã bị xóa rồi", id.ToString());
            }

            account.Active = false;

            await _unitOfWork.Repository<Account>().Update(account, id);
            await _unitOfWork.CommitAsync();

            var accountResponse = new AccountResponse
            {
                Id = account.Id,
                Fullname = account.Fullname,
                Phone = account.Phone,
                Email = account.Email,
                RoleId = account.RoleId,
                Active = account.Active,
                FcmToken = account.FcmToken,
                CreateDate = account.CreateDate,
                ImageUrl = account.ImageUrl,
                Password = account.Password,
                Role = account.Role != null ? new RoleResponse
                {
                    Id = account.Role.Id,
                    Name = account.Role.Name,
                } : null
            };

            return accountResponse;
        }


        //Store Add Shipper
        public async Task<AccountResponse> RegisterShipperPrivate(int storeId,RegisterRequest request)
        {
            var emailExisted = _unitOfWork.Repository<Account>().Find(x => x.Email == request.Email && x.RoleId == 5);
            var store = _unitOfWork.Repository<Store>().Find(x => x.Id == storeId);

            

            if (emailExisted != null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Email đã tồn tại", request.Email.ToString());
            }
            string otp = GenerateOTP();
           // CreatePasswordHash(request.Password, out string passwordHash);



            Account account = new Account
            {
                Fullname = request.FullName,
                Email = request.Email,
                Active = true,
                Phone = request.Phone,
                FcmToken = otp,
                RoleId = 5,
                Password = request.Password
            };
            await _unitOfWork.Repository<Account>().InsertAsync(account);
            await _unitOfWork.CommitAsync();

            Shipper shipper = new Shipper
            {
                FullName = account.Fullname,
                EmailShipper = account.Email,
                PhoneShipper = account.Phone,
                AccountId = account.Id,
                ZoneId = store.ZoneId,
                Status = (int)ShipperStatusEnum.Offline,
                StoreId = storeId,
                Type = (int)ShipperTypeEnum.PRIVATE,
            };

            await _unitOfWork.Repository<Shipper>().InsertAsync(shipper);
            await _unitOfWork.CommitAsync();

            await SendAccountShipper(request.Email, request.Password);

            Wallet wallet1 = new Wallet
            {
                Balance = 0,
                Type = (int)WalletTypeEnum.VICHINH,
                ShipperId = shipper.Id
            };
            await _unitOfWork.Repository<Wallet>().InsertAsync(wallet1);
            Wallet wallet2 = new Wallet
            {
                Balance = 0,
                Type = (int)WalletTypeEnum.VITHUHO,
                ShipperId = shipper.Id
            };
            await _unitOfWork.Repository<Wallet>().InsertAsync(wallet2);
            Wallet wallet3 = new Wallet
            {
                Balance = 0,
                Type = (int)WalletTypeEnum.VIKICHHOAT,
                ShipperId = shipper.Id
            };
            await _unitOfWork.Repository<Wallet>().InsertAsync(wallet3);
            await _unitOfWork.CommitAsync();

            return new AccountResponse
            {
                Id = account.Id,
                Fullname = account.Fullname,
                Phone = account.Phone,
                Email = account.Email,
                RoleId = account.RoleId,
                Active = account.Active,
                FcmToken = account.FcmToken,
                CreateDate = account.CreateDate,
                ImageUrl = account.ImageUrl,

            };

        }


        //Store Add Shipper
        public async Task<AccountResponse> ActiveShipperFromStaff(int accountId, int zoneId)
        {
            var accounts = await _unitOfWork.Repository<Account>()
                .GetAll()
                .Include(o => o.Role)
                .FirstOrDefaultAsync(a => a.Id == accountId);



            if (accounts == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Tài khoản không tồn tại", accountId.ToString());
            }
            //string otp = GenerateOTP();
            // CreatePasswordHash(request.Password, out string passwordHash);

            accounts.Active = true;

            Shipper shipper = new Shipper
            {
                FullName = accounts.Fullname,
                EmailShipper = accounts.Email,
                PhoneShipper = accounts.Phone,
                AccountId = accounts.Id,
                ZoneId = zoneId,
                Status = (int)ShipperStatusEnum.Offline,
                Type = (int)ShipperTypeEnum.PUBLIC,
            };


            await _unitOfWork.Repository<Account>().Update(accounts, accountId);
            await _unitOfWork.CommitAsync();

            await _unitOfWork.Repository<Shipper>().InsertAsync(shipper);
            await _unitOfWork.CommitAsync();

            //await SendAccountShipper(request.Email, request.Password);

            Wallet wallet1 = new Wallet
            {
                Balance = 0,
                Type = (int)WalletTypeEnum.VICHINH,
                ShipperId = shipper.Id
            };
            await _unitOfWork.Repository<Wallet>().InsertAsync(wallet1);
            Wallet wallet2 = new Wallet
            {
                Balance = 0,
                Type = (int)WalletTypeEnum.VITHUHO,
                ShipperId = shipper.Id
            };
            await _unitOfWork.Repository<Wallet>().InsertAsync(wallet2);
            Wallet wallet3 = new Wallet
            {
                Balance = 500000,
                Type = (int)WalletTypeEnum.VIKICHHOAT,
                ShipperId = shipper.Id
            };
            await _unitOfWork.Repository<Wallet>().InsertAsync(wallet3);
            await _unitOfWork.CommitAsync();

            return new AccountResponse
            {
                Id = accounts.Id,
                Fullname = accounts.Fullname,
                Phone = accounts.Phone,
                Email = accounts.Email,
                RoleId = accounts.RoleId,
                Active = accounts.Active,
                FcmToken = accounts.FcmToken,
                CreateDate = accounts.CreateDate,
                ImageUrl = accounts.ImageUrl,

            };

        }

        public async Task SendAccountShipper(string email, string password)
        {
            string subject = "Tài khoản LocalShipper";
            string content = $"Welcome to LocalShipper. Login email: {email}. Password: {password}. Please download the app:";

            var message = new Message(new List<string> { email }, subject, content);
            _emailService.SendEmail(message);
        }

        public async Task SendTrackingOrder(string email, string trackingNumber, int orderId)
        {
            string link = $"https://localshipper.vercel.app/tracking-order?orderId={orderId}";
            string subject = $"Đơn hàng {trackingNumber} - LocalShipper";
            string content = $"Cảm ơn bạn đã sử dụng LocalShipper, Bấm vào link để theo dõi đơn hàng {link}";

            var message = new Message(new List<string> { email }, subject, content);
            _emailService.SendEmail(message);
        }

        public async Task SendNotificationToStore(string email, string trackingNumber)
        {
            string subject = $"Đơn hàng {trackingNumber} - LocalShipper";
            string content = $"Cảm ơn bạn đã sử dụng LocalShipper, Đơn hàng {trackingNumber} - Đã vượt quá thời gian nhận và giao hàng. Liên hệ shipper hoặc báo cáo về hệ thống LocalShipper theo số điện thoại: 076***4855";

            var message = new Message(new List<string> { email }, subject, content);
            _emailService.SendEmail(message);
        }

        public async Task SendNotificationToStoreWaiting(string email, string trackingNumber)
        {
            string subject = $"Đơn hàng {trackingNumber} - LocalShipper";
            string content = $"Cảm ơn bạn đã sử dụng LocalShipper, Đơn hàng {trackingNumber} - Shipper không nhận đơn hoặc không có Shipper nào đang rảnh.";

            var message = new Message(new List<string> { email }, subject, content);
            _emailService.SendEmail(message);
        }

        public async Task<string> SendOTPWallet(string email)
        {
            var shipper = await _unitOfWork.Repository<Shipper>()
               .GetAll()
               .FirstOrDefaultAsync(a => a.EmailShipper == email);

            Random random = new Random();
            int otp = random.Next(100000, 999999);

            shipper.Fcmtoken =  otp.ToString();
            await _unitOfWork.Repository<Shipper>().Update(shipper, shipper.Id);
            await _unitOfWork.CommitAsync();

            string subject = "Xác thực lệnh rút/nạp tiền - LocalShipper";
            string content = $"Mã OTP của bạn là: {otp}";
            var message = new Message(new List<string> { email }, subject, content);
            _emailService.SendEmail(message);
            return "Gửi Email thành công"; 
        }


        //FORGOT PASSWORD
        public async Task<AccountResponse> SendMailForgotPassword(string email)
        {
            var account = _unitOfWork.Repository<Account>().Find(x => x.Email == email);

            if (account == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Tài khoản không tồn tại", email);
            }
           

            Random random = new Random();
            int otp = random.Next(1000, 9999);

            account.FcmToken = otp.ToString();
            await _unitOfWork.Repository<Account>().Update(account, account.Id);
            await _unitOfWork.CommitAsync();

            string subject = "Local Shipper - Forgot Password";
            string content = $"Mã số đặt lại mật khẩu của bạn là: {otp}";

            var message = new Message(new List<string> { email }, subject, content);
            _emailService.SendEmail(message);

            return new AccountResponse
            {
                Id = account.Id,
                Fullname = account.Fullname,
                Phone = account.Phone,
                Email = account.Email,
                RoleId = account.RoleId,
                Active = account.Active,
                FcmToken = account.FcmToken,
                CreateDate = account.CreateDate,
                ImageUrl = account.ImageUrl,
            };
        }

        public async Task<bool> VerifyForgotPassword(string email, string otp)
        {
            var account = _unitOfWork.Repository<Account>().Find(x => x.Email == email);
            if (account.FcmToken != otp)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Mã xác thực không chính xác", email);
            }
            if (account != null)
            { 
                return true;
            }

            return false;
        }

        public async Task<string> ChangePassword(int userId, string currentPassword, string newPassword)
        {
            var user = await _unitOfWork.Repository<Account>().GetAll().Where(b => b.Id == userId).FirstOrDefaultAsync();

            if (user == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Tài khoản không tồn tại", userId.ToString());
            }

            if (user.Password != currentPassword)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Mật khẩu hiện tại không đúng", userId.ToString());
            }

            // Kiểm tra rằng `newPassword` đáp ứng các yêu cầu về mật khẩu của bạn (độ dài, ký tự đặc biệt, v.v.)

            user.Password = newPassword;
            await _unitOfWork.Repository<Account>().Update(user, user.Id);
            await _unitOfWork.CommitAsync();

            return "Mật khẩu đã được thay đổi thành công.";
        }

        public async Task<string> ChangePasswordOfForget(string email, string newPassword)
        {
            var user = await _unitOfWork.Repository<Account>().GetAll().Where(b => b.Email == email.Trim()).FirstOrDefaultAsync();

            if (user == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Tài khoản không tồn tại", email.ToString());
            }      

            user.Password = newPassword;
            await _unitOfWork.Repository<Account>().Update(user, user.Id);
            await _unitOfWork.CommitAsync();

            return "Mật khẩu đã được thay đổi thành công.";
        }



    }
}
