using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Interface
{
    public interface ITransportTypeService
    {
        Task<List<TransportTypeResponse>> GetTransportType(int? id, string? transportType);
        Task<int> GetTotalTransportTypeCount();
        Task<TransportTypeResponse> CreateTransportType(RegisterTransportTypeRequest request);
        Task<TransportTypeResponse> UpdateTransportType(int id, PutTransportTypeRequest transportTypeRequest);
        Task<MessageResponse> DeleteTransportType(int id);
    }
}
