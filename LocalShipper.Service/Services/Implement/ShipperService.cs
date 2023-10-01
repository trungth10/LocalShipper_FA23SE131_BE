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
using System.Threading.Tasks;
using LocalShipper.Service.Helpers;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Security.Cryptography.Xml;

namespace LocalShipper.Service.Services.Implement
{
    public class ShipperService : IShipperService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public ShipperService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        //UPDATE Shipper Status
        public async Task<ShipperResponse> UpdateShipperStatus(int shipperId, UpdateShipperStatusRequest request)
        {
            try
            {
                var shipper = _unitOfWork.Repository<Shipper>().Find(x => x.Id == shipperId);

                if (shipper == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy người giao hàng", shipperId.ToString());
                }

                shipper.Status = (int)request.status;

                await _unitOfWork.Repository<Shipper>().Update(shipper, shipperId);
                await _unitOfWork.CommitAsync();


                return _mapper.Map<Shipper, ShipperResponse>(shipper);
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Cập nhật trạng thái người giao hàng thất bại", ex.InnerException?.Message);
            }
        }


        //CREATE Shipper
        public async Task<ShipperResponse> RegisterShipperInformation(ShipperInformationRequest request)
        {
            //Check Email trùng
            var emailExisted = await _unitOfWork.Repository<Shipper>().FindAsync(x => x.EmailShipper == request.EmailShipper);
            if (emailExisted != null)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Email đã tồn tại", request.EmailShipper);
            }
            //Check Phone trùng
            var phoneExisted = await _unitOfWork.Repository<Shipper>().FindAsync(x => x.PhoneShipper == request.PhoneShipper);
            if (phoneExisted != null)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Số điện thoại đã tồn tại", request.EmailShipper);
            }

            //Check transportId trùng
            var transportIdExisted = await _unitOfWork.Repository<Shipper>().FindAsync(x => x.TransportId == request.TransportId);
            if (transportIdExisted != null)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "TransportId đã tồn tại", request.TransportId.ToString());
            }

            //Check accountId trùng
            var accountIdExisted = await _unitOfWork.Repository<Shipper>().FindAsync(x => x.AccountId == request.AccountId);
            if (accountIdExisted != null)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "AccountId đã tồn tại", request.AccountId.ToString());
            }

            //Check walletId trùng
            var walletIdExisted = await _unitOfWork.Repository<Shipper>().FindAsync(x => x.WalletId == request.WalletId);
            if (walletIdExisted != null)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "WalletId đã tồn tại", request.WalletId.ToString());
            }



            Shipper shipper = new Shipper
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                EmailShipper = request.EmailShipper,
                PhoneShipper = request.PhoneShipper,
                AddressShipper = request.AddressShipper,
                TransportId = request.TransportId,
                AccountId = request.AccountId,
                ZoneId = request.ZoneId,
                Status = request.Status,
                Fcmtoken = request.Fcmtoken,
                WalletId = request.WalletId
            };
            await _unitOfWork.Repository<Shipper>().InsertAsync(shipper);
            await _unitOfWork.CommitAsync();

            var createdShipperResponse = new ShipperResponse
            {
                Id = shipper.Id,
                FirstName = request.FirstName,
                LastName = request.LastName,
                EmailShipper = request.EmailShipper,
                PhoneShipper = request.PhoneShipper,
                AddressShipper = request.AddressShipper,
                TransportId = request.TransportId,
                AccountId = request.AccountId,
                ZoneId = request.ZoneId,
                Status = (ShipperStatusEnum)request.Status,
                Fcmtoken = request.Fcmtoken,
                WalletId = request.WalletId
            };
            return createdShipperResponse;
        }

        //GET
        public async Task<List<ShipperResponse>> GetShipper(int? id, string? firstName, string? lastName, string? email, string? phone, 
            string? address, int? transportId, int? accountId, int? zoneId, int? status, string? fcmToken, int? walletId)
        {

            var shippers = await _unitOfWork.Repository<Shipper>().GetAll()
                                                              .Where(t => id == 0 || t.Id == id)
                                                              .Where(t => string.IsNullOrWhiteSpace(firstName) || t.FirstName.Contains(firstName))
                                                              .Where(t => string.IsNullOrWhiteSpace(lastName) || t.LastName.Contains(lastName))
                                                              .Where(t => string.IsNullOrWhiteSpace(email) || t.EmailShipper.Contains(email))
                                                              .Where(t => string.IsNullOrWhiteSpace(phone) || t.PhoneShipper.Contains(phone))
                                                              .Where(t => string.IsNullOrWhiteSpace(address) || t.AddressShipper.Contains(address))
                                                              .Where(t => transportId == 0 || t.TransportId == transportId)
                                                              .Where(t => accountId == 0 || t.AccountId == accountId)
                                                              .Where(t => zoneId == 0 || t.ZoneId == zoneId)
                                                              .Where(t => status == 0 || t.Status == status)
                                                              .Where(t => string.IsNullOrWhiteSpace(fcmToken) || t.Fcmtoken.Contains(fcmToken))
                                                              .Where(t => walletId == 0 || t.WalletId == walletId)
                                                              .ToListAsync();
            var shipperResponses = shippers.Select(shipper => new ShipperResponse
            {
                Id = shipper.Id,
                FirstName = shipper.FirstName,
                LastName = shipper.LastName,
                EmailShipper = shipper.EmailShipper,
                PhoneShipper = shipper.PhoneShipper,
                AddressShipper = shipper.AddressShipper,
                TransportId = shipper.TransportId,
                AccountId = shipper.AccountId,
                ZoneId = shipper.ZoneId,
                Fcmtoken = shipper.Fcmtoken,
                Status = (ShipperStatusEnum)shipper.Status,
                WalletId = shipper.WalletId,

            }).ToList();
            return shipperResponses;
        }

        //GET List
        //public async Task<List<ShipperResponse>> GetListShipper(int? zoneId = null)
        //{
        //    IQueryable<Shipper> query = _unitOfWork.Repository<Shipper>().GetAll().Include(o => o.Zone);

        //    if (zoneId.HasValue)
        //    {
        //        query = query.Where(a => a.ZoneId == zoneId);
        //    }


        //    var shippers = await query.ToListAsync();


        //    if (shippers.Count == 0)
        //    {
        //        throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy shipper", shippers.ToString());
        //    }

        //    var shipperResponses = shippers.Select(shipper => new ShipperResponse
        //    {
        //        Id = shipper.Id,
        //        FirstName = shipper.FirstName,
        //        LastName = shipper.LastName,
        //        EmailShipper = shipper.EmailShipper,
        //        PhoneShipper = shipper.PhoneShipper,
        //        AddressShipper = shipper.AddressShipper,
        //        TransportId = shipper.TransportId,
        //        AccountId = shipper.AccountId,
        //        ZoneId = shipper.ZoneId,
        //        Fcmtoken = shipper.Fcmtoken,
        //        Status = (ShipperStatusEnum)shipper.Status,
        //        WalletId = shipper.WalletId,


        //        Zone = shipper.Zone != null ? new ZoneResponse
        //        {
        //            Id = shipper.Zone.Id,
        //            ZoneName = shipper.Zone.ZoneName,
        //            ZoneDescription = shipper.Zone.ZoneDescription,
        //            Latitude = shipper.Zone.Latitude,
        //            Longitude = shipper.Zone.Longitude,
        //            Radius = shipper.Zone.Radius,
        //            CreatedAt = shipper.Zone.CreatedAt,
        //            UpdateAt = shipper.Zone.UpdateAt,
        //        } : null
        //    }).ToList();

        //    return shipperResponses;
        //}

        //GET Count
        public async Task<int> GetTotalShipperCount()
        {
            var count = await _unitOfWork.Repository<Shipper>()
                .GetAll()
                .CountAsync();

            return count;
        }

        ////GET All
        //public async Task<List<ShipperResponse>> GetAll()
        //{
        //    var shippers = await _unitOfWork.Repository<Shipper>().GetAll().ToListAsync();

        //    var shipperResponses = shippers.Select(shipper => new ShipperResponse
        //    {
        //        Id = shipper.Id,
        //        FirstName = shipper.FirstName,
        //        LastName = shipper.LastName,
        //        EmailShipper = shipper.EmailShipper,
        //        PhoneShipper = shipper.PhoneShipper,
        //        TransportId = shipper.TransportId,
        //        ZoneId = shipper.ZoneId,
        //        Status = ShipperStatusEnum.Offline,
        //    }).ToList();
        //    return shipperResponses;
        //}


        //UPDATE Shipper
        public async Task<ShipperResponse> UpdateShipper(int id, PutShipperRequest shipperRequest)
        {
            var shipper = await _unitOfWork.Repository<Shipper>()
                .GetAll()
                .Include(o => o.Account)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (shipper == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy shipper", id.ToString());
            }
            // Check RoleId ở Account có = 5 hay không
            if (shipper.Account != null && shipper.Account.RoleId == 5)
            {
                shipper.FirstName = shipperRequest.FirstName;
                shipper.LastName = shipperRequest.LastName;
                shipper.EmailShipper = shipperRequest.EmailShipper;
                shipper.PhoneShipper = shipperRequest.PhoneShipper;
                shipper.AddressShipper = shipperRequest.AddressShipper;
                shipper.TransportId = shipperRequest.TransportId;
                shipper.AccountId = shipperRequest.AccountId;
                shipper.ZoneId = shipperRequest.ZoneId;
                shipper.Fcmtoken = shipperRequest.Fcmtoken;
                shipper.WalletId = shipperRequest.WalletId;

                //Check Email trùng
                var emailExisted = await _unitOfWork.Repository<Shipper>()
                    .GetAll()
                    .FirstOrDefaultAsync(a => a.EmailShipper == shipperRequest.EmailShipper && a.Id != id);
                if (emailExisted != null)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Email đã tồn tại cho một tài khoản khác.", id.ToString());
                }
                //Check Phone trùng
                var phoneExisted = await _unitOfWork.Repository<Shipper>()
                    .GetAll()
                    .FirstOrDefaultAsync(a => a.PhoneShipper == shipperRequest.PhoneShipper && a.Id != id);
                if (phoneExisted != null)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Số điện thoại đã tồn tại cho một tài khoản khác.", id.ToString());
                }

                //Check transportId trùng
                var transportIdExisted = await _unitOfWork.Repository<Shipper>()
                    .GetAll()
                    .FirstOrDefaultAsync(a => a.TransportId == shipperRequest.TransportId && a.Id != id);
                if (transportIdExisted != null)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Transport Id đã tồn tại cho một tài khoản khác.", id.ToString());
                }

                //Check accountId trùng
                var accountIdExisted = await _unitOfWork.Repository<Shipper>()
                    .GetAll()
                    .FirstOrDefaultAsync(a => a.AccountId == shipperRequest.AccountId && a.Id != id);
                if (accountIdExisted != null)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Account Id đã tồn tại cho một tài khoản khác.", id.ToString());
                }

                //Check walletId trùng
                var walletIdExisted = await _unitOfWork.Repository<Shipper>()
                    .GetAll()
                    .FirstOrDefaultAsync(a => a.WalletId == shipperRequest.WalletId && a.Id != id);
                if (walletIdExisted != null)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Wallet Id đã tồn tại cho một tài khoản khác.", id.ToString());
                }


                await _unitOfWork.Repository<Shipper>().Update(shipper, id);
                await _unitOfWork.CommitAsync();

                var updatedShipperResponse = new ShipperResponse
                {
                    Id = shipper.Id,
                    FirstName = shipper.FirstName,
                    LastName = shipper.LastName,
                    EmailShipper = shipper.EmailShipper,
                    PhoneShipper = shipper.PhoneShipper,
                    AddressShipper = shipper.AddressShipper,
                    TransportId = shipper.TransportId,
                    AccountId = shipper.AccountId,
                    ZoneId = shipper.ZoneId,
                    Status = (ShipperStatusEnum)shipper.Status,
                    Fcmtoken = shipper.Fcmtoken,
                    WalletId = shipper.WalletId,


                    Account = shipper.Account != null ? new AccountResponse
                    {
                        Id = shipper.Account.Id,
                        Fullname = shipper.Account.Fullname,
                        Phone = shipper.Account.Phone,
                        Email = shipper.Account.Email,
                        RoleId = shipper.Account.RoleId,
                        Active = shipper.Account.Active,
                        FcmToken = shipper.Account.FcmToken,
                        CreateDate = shipper.Account.CreateDate,
                        ImageUrl = shipper.Account.ImageUrl,
                        Password = shipper.Account.Password,
                    } : null
                };

                return updatedShipperResponse;
            }
            else
            {
                throw new CrudException(HttpStatusCode.Forbidden, "Cập nhật Shipper thất bại", id.ToString());
            }
        }


        //DELETE Shipper
        public async Task<MessageResponse> DeleteShipper(int id)
        {

            var shipper = await _unitOfWork.Repository<Shipper>().GetAll()
            .Include(o => o.Account)
            .FirstOrDefaultAsync(a => a.Id == id);

            if (shipper == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy shipper", id.ToString());
            }

            _unitOfWork.Repository<Shipper>().Delete(shipper);
            await _unitOfWork.CommitAsync();

            return new MessageResponse
            {
                Message = "Xóa shipper thành công",
            };
        }
    }
}




