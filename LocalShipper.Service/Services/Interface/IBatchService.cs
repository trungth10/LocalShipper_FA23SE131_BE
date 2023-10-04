using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Interface
{
    public interface IBatchService
    {
        Task<List<BatchResponse>> GetBatch(int? id, string? batchName, int? pageNumber, int? pageSize);
        Task<BatchResponse> CreateBatch(BatchRequest request);
        Task<BatchResponse> UpdateBatch(int id, BatchRequest batchRequest);

        Task<BatchResponse> DeleteBatch(int id);

        Task<int> GetTotalBatchCount();
    }
}
