using AutoMapper;
using AutoMapper.Configuration.Conventions;
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
using System.Text;
using System.Threading.Tasks;

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


        public async Task<List<BatchResponse>> GetBatch(int? id, int? storeId, string? batchName)
        {

            var batchs = await _unitOfWork.Repository<Batch>().GetAll()
                                                              .Where(b => id == 0 || b.Id == id)
                                                              .Where(b => storeId == 0 || b.StoreId == storeId)
                                                              .Where(b => string.IsNullOrWhiteSpace(batchName) || b.BatchName.Contains(batchName))
                                                              .ToListAsync();
            var batchResponses = batchs.Select(batch => new BatchResponse
            {
                Id = batch.Id,
                StoreId = batch.StoreId,
                BatchName = batch.BatchName,
                BatchDescription = batch.BatchDescription,
                CreatedAt = batch.CreatedAt,
                UpdateAt = batch.UpdateAt,
            }).ToList();
            return batchResponses;
        }

    }

}
