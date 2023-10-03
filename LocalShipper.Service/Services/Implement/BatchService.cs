using AutoMapper;
using AutoMapper.Configuration.Conventions;
using LocalShipper.Data.Models;
using LocalShipper.Data.UnitOfWork;
using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Exceptions;
using LocalShipper.Service.Helpers;
using LocalShipper.Service.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace LocalShipper.Service.Services.Implement
{
    public class BatchService : IBatchService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public BatchService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }



        public async Task<List<BatchResponse>> GetBatch(int? id, string? batchName)
        {

            var batchs = await _unitOfWork.Repository<Batch>().GetAll()
                                                              .Where(b => id == 0 || b.Id == id)
                                                              .Where(b => string.IsNullOrWhiteSpace(batchName) || b.BatchName.Contains(batchName))
                                                              .ToListAsync();
            var batchResponses = batchs.Select(batch => new BatchResponse
            {
                Id = batch.Id,
                BatchName = batch.BatchName,
                BatchDescription = batch.BatchDescription,
                CreatedAt = batch.CreatedAt,
                UpdateAt = batch.UpdateAt,
                Status = batch.Status,
               


            }).ToList();
            return batchResponses;
        }

        public async Task<BatchResponse> CreateBatch(BatchRequest request)
        {
            var existingBatch = await _unitOfWork.Repository<Batch>().FindAsync(b => b.BatchName == request.BatchName);
            if (existingBatch != null)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Batch đã tồn tại", request.BatchName);
            }


            var newBatch = new Batch
            {
                BatchName = request.BatchName,
                BatchDescription = request.BatchDescription,
                CreatedAt = DateTime.Now,
                UpdateAt = DateTime.Now,
                Status = request.Status,
            };


            await _unitOfWork.Repository<Batch>().InsertAsync(newBatch);
            await _unitOfWork.CommitAsync();


            var batchResponse = _mapper.Map<BatchResponse>(newBatch);
            return batchResponse;
        }
        public async Task<BatchResponse> UpdateBatch(int id, BatchRequest batchRequest)
        {
            var batch = await _unitOfWork.Repository<Batch>()
                .GetAll()
                .FirstOrDefaultAsync(b => b.Id == id);

            if (batch == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy batch", id.ToString());
            }

            batch.BatchName = batchRequest.BatchName;
            batch.BatchDescription = batchRequest.BatchDescription;
            batch.Status = batchRequest.Status;
            batch.UpdateAt = DateTime.Now;


            await _unitOfWork.Repository<Batch>().Update(batch, id);
            await _unitOfWork.CommitAsync();

            var updatedBatchResponse = new BatchResponse
            {
                Id = batch.Id,
                BatchName = batch.BatchName,
                BatchDescription = batch.BatchDescription,
                CreatedAt = batch.CreatedAt,
                UpdateAt = batch.UpdateAt,
                Status= batch.Status,
               
            };

            return updatedBatchResponse;
        }

        public async Task<BatchResponse> DeleteBatch(int id)
        {
            var batch = await _unitOfWork.Repository<Batch>()
                .GetAll()
                .FirstOrDefaultAsync(b => b.Id == id);

            if (batch == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy batch", id.ToString());
            }


            batch.Status = (int)StatusOfBatch.DELETE;

            await _unitOfWork.Repository<Batch>().Update(batch, id);
            await _unitOfWork.CommitAsync();

            //var deletedBatchResponse = new BatchResponse
            //{
            //    Id = batch.Id,
            //    StoreId = batch.StoreId,
            //    BatchName = batch.BatchName,
            //    BatchDescription = batch.BatchDescription,
            //    CreatedAt = batch.CreatedAt,
            //    UpdateAt = batch.UpdateAt,
            //    Status = batch.Status
            //};
            var deletedBatchResponse = _mapper.Map<BatchResponse>(batch);
            return deletedBatchResponse;
        }

        public async Task<int> GetTotalBatchCount()
        {
            var count = await _unitOfWork.Repository<Batch>()
                .GetAll()
                .CountAsync();

            return count;
        }
    }

}
