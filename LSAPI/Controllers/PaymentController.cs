using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Services.Implement;
using LocalShipper.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using LocalShipper.Service.DTOs.Request;
using LocalShipper.Data.Models;
using LocalShipper.Service.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace LSAPI.Controllers
{
    [ApiController]
    [Route("api/payments")]
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet()]
        public async Task<ActionResult<PaymentResponse>> GetPayment(int id, string paymentMethod, int status, string paymentCode
            , string paymentImage, int packageId, int? pageNumber, int? pageSize)
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
                var rs = await _paymentService.GetPayment(id, paymentMethod, status, paymentCode, paymentImage, packageId, pageNumber, pageSize);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xem thất bại: {ex.Message}");
            }
        }



        [HttpGet("api/payments/count")]
        public async Task<ActionResult<PaymentResponse>> GetCountPayment()
        {
            try
            {
                var rs = await _paymentService.GetTotalPaymentCount();
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xem count thất bại: {ex.Message}");
            }

        }

        [HttpPost()]
        public async Task<ActionResult<PaymentResponse>> CreatePayment([FromBody] PaymentRequest request)
        {
            try
            {
                if (request.PackageId <= 0)
                {
                    return BadRequest("PackageId phải là số nguyên dương");
                }
                var rs = await _paymentService.CreatePayment(request);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Tạo thất bại: {ex.Message}");
            }
        }

        [HttpPut()]
        public async Task<ActionResult<PaymentResponse>> UpdatePayment(int id,[FromBody] PutPaymentRequest request)
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
                if (request.Status <= 0)
                {
                    return BadRequest("Status không hợp lệ");
                }
                if (request.PackageId <= 0)
                {
                    return BadRequest("PackageId phải là số nguyên dương");
                }
                var rs = await _paymentService.UpdatePayment(id, request);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Cập nhật thất bại: {ex.Message}");
            }
        }

        [HttpDelete()]
        public async Task<ActionResult<PaymentResponse>> DeletePayment(int id)
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
                var rs = await _paymentService.DeletePayment(id);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xóa thất bại: {ex.Message}");
            }
        }
    }
}
