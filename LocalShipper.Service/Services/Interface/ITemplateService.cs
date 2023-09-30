using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Interface
{
    public interface ITemplateService
    {
        Task<List<TemplateResponse>> GetTemplate(int? id, string? templateName);
        Task<TemplateResponse> CreateTemplate(TemplateRequest request);
        Task<TemplateResponse> UpdateTemplate(int id, TemplateRequest templateRequest);

        Task<MessageResponse> DeleteTemplate(int id);
    }
}
