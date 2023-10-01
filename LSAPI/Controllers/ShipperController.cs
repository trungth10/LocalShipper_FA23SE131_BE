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

namespace LSAPI.Controllers
{
    [ApiController]
    [Route("api/shippers")]
    //[Authorize(Policy = "Shipper")]
    public class ShipperController : Controller
    {

        private readonly IShipperService _shipperService;

        public ShipperController(IShipperService shipperService)
        {
            _shipperService = shipperService;
        }

       /* [HttpPut("{shipperId:int}/status")]
        public async Task<ActionResult<ShipperResponse>> UpdateShipperStatus(int shipperId, [FromBody] UpdateShipperStatusRequest request)
        {
            try
            {

                // int agentId = Convert.ToInt32(User.FindFirst("UserId")?.Value);
                var response = await _shipperService.UpdateShipperStatus(shipperId, request);


                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Cập nhật trạng thái người giao hàng thất bại: {ex.Message}");
            }
        }*/


        [HttpGet()]
        public async Task<ActionResult<List<TransactionResponse>>> GetShipper(int id, string firstName, string lastName,
                                            string email, string phone,
                                            string address, int transportId, int accountId, int zoneId, int status, string fcmToken, int walletId)
        {
            try
            {
                var rs = await _shipperService.GetShipper(id, firstName, lastName, email, phone, address, transportId, accountId, zoneId, status, fcmToken, walletId);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Không tìm thấy shipper");
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

        [HttpGet("count")]
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


        [HttpPost("register-shipper-information")]
        public async Task<ActionResult<ShipperResponse>> CreateShipper([FromBody] ShipperInformationRequest request)
        {
            try
            {
                var rs = await _shipperService.RegisterShipperInformation(request);
                return Ok(rs);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPut()]
        public async Task<ActionResult<ShipperResponse>> UpdateShipper(int id, [FromBody] PutShipperRequest shipperRequest)
        {
            try
            {

                var response = await _shipperService.UpdateShipper(id, shipperRequest);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Cập nhật thông tin shipper thất bại: {ex.Message}");
            }
        }

        [HttpDelete()]
        public async Task<ActionResult<MessageResponse>> DeleteShipper(int id)
        {
            try
            {

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
