using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Interface
{
    public interface IHistoryService
    {
        Task<List<HistoryResponse>> GetHistory(int? id, string? action, int? storeId, int? pageNumber, int? pageSize);

        Task<int> GetTotalHistoryCount();
        Task<HistoryResponse> CreateHistory(RegisterHistoryRequest request);
        Task<HistoryResponse> UpdateHistory(int id, PutHistoryRequest historyRequest);
        Task<MessageResponse> DeleteHistory(int id);
    }
}
