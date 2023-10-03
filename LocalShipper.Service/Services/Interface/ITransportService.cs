using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Interface
{
    public interface ITransportService
    {
        Task<List<TransportResponse>> GetTransport(int? id, int? typeId, string? licencePlate, string? transportColor,
                                                    string? transportImage, string? transportRegistration, int? pageNumber, int? pageSize);

        //Task<List<TransportResponse>> GetListTransport(int? typeId, string? transportColor);

        Task<int> GetTotalTransportCount();
        Task<TransportResponse> CreateTransport(RegisterTransportRequest request);
        Task<TransportResponse> UpdateTransport(int id, PutTransportRequest transportRequest);
        Task<TransportResponse> DeleteTransport(int id);
    }
}
