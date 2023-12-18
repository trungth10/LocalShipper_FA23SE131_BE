using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using LocalShipper.Service.Services.Interface;
using LocalShipper.Service.Services.Implement;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using LocalShipper.Data.Models;
using System.Text.RegularExpressions;
using Org.BouncyCastle.Asn1.Ocsp;
using LocalShipper.Service.Helpers;

namespace LSAPI.Controllers
{
    [ApiController]
    [Route("api/shippers")]
    public class ShipperController : Controller
    {

        private readonly IShipperService _shipperService;

        public ShipperController(IShipperService shipperService)
        {
            _shipperService = shipperService;
        }

        [Authorize(Roles = Roles.Shipper + "," + Roles.Store, AuthenticationSchemes = "Bearer")]
        [HttpPut("status")]
        public async Task<ActionResult<ShipperResponse>> UpdateShipperStatus(int shipperId, [FromBody] UpdateShipperStatusRequest request)
        {
            try
            {
               
                if (shipperId <= 0)
                {
                    return BadRequest("ShipperId phải là số nguyên dương");
                }
                if (request.status <= 0)
                {
                    return BadRequest("Status chỉ từ 1 đến 4");
                }
                var response = await _shipperService.UpdateShipperStatus(shipperId, request);


                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Cập nhật trạng thái người giao hàng thất bại: {ex.Message}");
            }
        }

        [Authorize(Roles = Roles.Store + "," + Roles.Staff + "," + Roles.Shipper, AuthenticationSchemes = "Bearer")]
        [HttpGet()]
        public async Task<ActionResult<List<ShipperResponse>>> GetShipper(int id, int storeId, string fullName,
                                            string email, string phone,
                                            string address, int transportId, int accountId, int zoneId, int status, string fcmToken, int walletId, int? pageNumber, int? pageSize)
        {
            try
            {
                if (pageNumber.HasValue && pageNumber <= 0)
                {
                    return BadRequest("Số trang phải là số nguyên dương");
                }

                if (pageSize.HasValue && pageSize <= 0)
                {
                    return BadRequest("Số phần tử trong trang phải là số nguyên dương");
                }
                if (id < 0)
                {
                    return BadRequest("Id không hợp lệ");
                }
                var rs = await _shipperService.GetShipper(id, storeId, fullName, email, phone, address, transportId, accountId, zoneId, status, fcmToken, walletId, pageNumber, pageSize);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Không tìm thấy shipper { ex.Message}");
            }
        }

        //[HttpGet("shippers.json")]
        //public async Task<ActionResult<List<ShipperResponse>>> GetAll(int? zoneId)
        //{
        //    try
        //    {
        //        var rs = await _shipperService.GetListShipper(zoneId);
        //        return Ok(rs);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest($"Không tìm thấy shipper");
        //    }
        //}
        [Authorize(Roles = Roles.Store + "," + Roles.Staff + "," + Roles.Shipper, AuthenticationSchemes = "Bearer")]
        [HttpGet("api/shippers/count")]
        public async Task<ActionResult<ShipperResponse>> GetCountShipper()
        {
            try
            {

                var rs = await _shipperService.GetTotalShipperCount();
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xem count thất bại: {ex.Message}");
            }

        }

        [AllowAnonymous]
        [HttpPost("register-shipper-information")]
        public async Task<ActionResult<ShipperResponse>> CreateShipper([FromBody] ShipperInformationRequest request)
        {
            try
            {

                var regex = new Regex("^[a-zA-Z ]+$");
                var regex2 = new Regex("^[0-9]+$");

                if (!regex.IsMatch(request.FullName))
                {
                    return BadRequest("Tên không hợp lệ");
                }

                var regex3 = new Regex(@"^\w+@gmail\.com$");
                if (!regex3.IsMatch(request.EmailShipper))
                {
                    return BadRequest("Email phải có dạng example@gmail.com");
                }
                if (!regex2.IsMatch(request.PhoneShipper))
                {
                    return BadRequest("Số điện thoại không hợp lệ");
                }
               
                if (request.PhoneShipper.Length < 9 || request.PhoneShipper.Length > 11)
                {
                    return BadRequest("Số điện thoại phải có từ 9 đến 11 số");
                }
                if (request.TransportId <= 0)
                {
                    return BadRequest("TransportId phải là số nguyên dương");
                }
                if (request.AccountId <= 0)
                {
                    return BadRequest("AccountId phải là số nguyên dương");
                }
                if (request.ZoneId <= 0)
                {
                    return BadRequest("ZoneId phải là số nguyên dương");
                }
                if (request.Status <= 0)
                {
                    return BadRequest("Status không hợp lệ");
                }
                if (request.WalletId <= 0)
                {
                    return BadRequest("WalletId phải là số nguyên dương");
                }
                var rs = await _shipperService.RegisterShipperInformation(request);
                return Ok(rs);
            }
            catch (Exception ex) 
            { return BadRequest($"Thêm thông tin shipper thất bại: {ex.Message}"); }
        }

        [Authorize(Roles = Roles.Shipper, AuthenticationSchemes = "Bearer")]
        [HttpPut()]
        public async Task<ActionResult<ShipperResponse>> UpdateShipper(int id, [FromBody] PutShipperRequest shipperRequest)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Id phải là số nguyên dương");
                }
                var regex2 = new Regex("^[0-9]+$");
                var regex = new Regex("^[a-zA-Z ]+$");
                if (!regex.IsMatch(shipperRequest.FullName))
                {
                    return BadRequest("Tên không hợp lệ");
                }
                var regex3 = new Regex(@"^\w+@gmail\.com$");
                if (!regex3.IsMatch(shipperRequest.EmailShipper))
                {
                    return BadRequest("Email phải có dạng example@gmail.com");
                }
                if (!regex2.IsMatch(shipperRequest.PhoneShipper))
                {
                    return BadRequest("Số điện thoại không hợp lệ");
                }             
                if (shipperRequest.PhoneShipper.Length < 9 || shipperRequest.PhoneShipper.Length > 11)
                {
                    return BadRequest("Số điện thoại phải có từ 9 đến 11 số");
                }
                if (shipperRequest.TransportId <= 0)
                {
                    return BadRequest("TransportId phải là số nguyên dương");
                }
                if (shipperRequest.AccountId <= 0)
                {
                    return BadRequest("AccountId phải là số nguyên dương");
                }
                if (shipperRequest.ZoneId <= 0)
                {
                    return BadRequest("ZoneId phải là số nguyên dương");
                }
                if (shipperRequest.Status <= 0)
                {
                    return BadRequest("Status không hợp lệ");
                }
                if (shipperRequest.WalletId <= 0)
                {
                    return BadRequest("WalletId phải là số nguyên dương");
                }
                var response = await _shipperService.UpdateShipper(id, shipperRequest);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Cập nhật thông tin shipper thất bại: {ex.Message}");
            }
        }

        [Authorize(Roles = Roles.Admin + "," + Roles.Staff + "," + Roles.Store, AuthenticationSchemes = "Bearer")]
        [HttpDelete()]
        public async Task<ActionResult<MessageResponse>> DeleteShipper(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest("Vui lòng nhập Id");
                }
                if (id < 0)
                {
                    return BadRequest("Id phải là số nguyên dương");
                }
                var response = await _shipperService.DeleteShipper(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xóa shipper thất bại: {ex.Message}");
            }
        }
    }
}
