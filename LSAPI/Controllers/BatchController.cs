using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Services.Implement;
using LocalShipper.Service.Services.Interface;
using MailKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LSAPI.Controllers
{
    [ApiController]
    [Route("api/batchs")]
    //[Authorize(Policy = "Shipper")]
    public class BatchController : Controller
    {
        private readonly IBatchService _batchService;
        public BatchController(IBatchService batchService)
        {
            _batchService = batchService;
        }

        [HttpGet()]
        public async Task<ActionResult<List<BatchResponse>>> GetBatch(int id, string batchName, int? pageNumber, int? pageSize)
        {
            try
            {
                if (pageNumber.HasValue && pageNumber < 0)
                {
                    return BadRequest("pageNumber phải là số dương");
                }

                if (pageSize.HasValue && pageSize < 0)
                {
                    return BadRequest("pageSize phải là số dương");
                }
                if (id < 0 )
                {
                    return BadRequest("id không hợp lệ");
                }




                var rs = await _batchService.GetBatch(id, batchName, pageNumber, pageSize);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xem Batch thất bại: {ex.Message}");
            }

        }

        [HttpPost()]
        public async Task<ActionResult<BatchResponse>> PostBatch(BatchRequest request)
        {
            try
            {
                if(request.StoreId <= 0 )
                {
                    return BadRequest("storeId phải là 1 số dương");
                }

                var regex = new Regex("^[a-zA-Z0-9 ]+$"); 
                if (!regex.IsMatch(request.BatchName))
                {
                    return BadRequest("batchName không được chứa ký tự đặc biệt");
                }
                if (!regex.IsMatch(request.BatchDescription))
                {
                    return BadRequest("batchDescription không được chứa ký tự đặc biệt");
                }
                if (request.Status <= 0)
                {
                    return BadRequest("status phải phải là 1 số dương");
                }
                var rs = await _batchService.CreateBatch(request);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"thêm Batch thất bại: {ex.Message}");
            }

        }


        [HttpPut()]
        public async Task<ActionResult<BatchResponse>> PutBatch(int id, BatchRequest batchRequest)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest("làm ơn hãy nhập id");
                }
                if (id <= 0)
                {
                    return BadRequest("id phải là số dương");
                }
                if (batchRequest.StoreId <= 0)
                {
                    return BadRequest("storeId phải là 1 số dương");
                }

                var regex = new Regex("^[a-zA-Z0-9 ]+$");
                if (!regex.IsMatch(batchRequest.BatchName))
                {
                    return BadRequest("batchName không được chứa ký tự đặc biệt");
                }
                if (!regex.IsMatch(batchRequest.BatchDescription))
                {
                    return BadRequest("batchDescription không được chứa ký tự đặc biệt");
                }
                if(batchRequest.Status <= 0)
                {
                    return BadRequest("status phải phải là 1 số dương");
                }
                var rs = await _batchService.UpdateBatch(id, batchRequest);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"cập nhật Batch thất bại: {ex.Message}");
            }

        }

        [HttpDelete()]
        public async Task<ActionResult<BatchResponse>> DeleteBatch(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest("làm ơn hãy nhập id");
                }
                if (id <= 0) 
                {
                    return BadRequest("id phải là số dương");
                }
                var rs = await _batchService.DeleteBatch(id);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"xóa Batch thất bại: {ex.Message}");
            }

        }



        [HttpGet("count")]
        public async Task<ActionResult<BatchResponse>> GetCountBatch()
        {
            try
            {

                var rs = await _batchService.GetTotalBatchCount();
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xem count thất bại: {ex.Message}");
            }

        }
    }
}
