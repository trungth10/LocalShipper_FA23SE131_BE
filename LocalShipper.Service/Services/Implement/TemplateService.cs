using AutoMapper;
using LocalShipper.Data.Models;
using LocalShipper.Data.UnitOfWork;
using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Exceptions;
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
    public class TemplateService : ITemplateService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public TemplateService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<TemplateResponse>> GetTemplate(int? id, string? templateName, int? pageNumber, int? pageSize)
        {

            var templates = _unitOfWork.Repository<Template>().GetAll()
                                                              .Where(b => id == 0 || b.Id == id)

                                                              .Where(b => string.IsNullOrWhiteSpace(templateName) || b.TemplateName.Contains(templateName.Trim()));
            // Xác định giá trị cuối cùng của pageNumber
            pageNumber = pageNumber.HasValue ? Math.Max(1, pageNumber.Value) : 1;
            // Áp dụng phân trang nếu có thông số pageNumber và pageSize
            if (pageNumber.HasValue && pageSize.HasValue)
            {
                templates = templates.Skip((pageNumber.Value - 1) * pageSize.Value)
                                       .Take(pageSize.Value);
            }

            var templateList = await templates.ToListAsync();
            if (templateList == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Template không có hoặc không tồn tại", id.ToString());
            }
            var templateRespnses = _mapper.Map<List<Template>, List<TemplateResponse>>(templateList);
            return templateRespnses;
        }
        public async Task<TemplateResponse> CreateTemplate(TemplateRequest request)
        {

            var existingTemplate = await _unitOfWork.Repository<Template>().FindAsync(b => b.TemplateName == request.TemplateName);
            if (existingTemplate != null)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "TemplateName đã tồn tại", request.TemplateName);
            }

            var newTemplate = new Template
            {
                TemplateName= request.TemplateName.Trim(),
                ImageUrl= request.ImageUrl.Trim(),
                CreateAt= DateTime.UtcNow,
                Deleted = request.Deleted,
            };

            await _unitOfWork.Repository<Template>().InsertAsync(newTemplate);
            await _unitOfWork.CommitAsync();


            var TemplateResponses = _mapper.Map<TemplateResponse>(newTemplate);
            return TemplateResponses;
        }

        public async Task<TemplateResponse> UpdateTemplate(int id, TemplateRequest templateRequest)
        {
            var template = await _unitOfWork.Repository<Template>()
                .GetAll()
                .FirstOrDefaultAsync(t => t.Id == id);

            if (template == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy Template", id.ToString());
            }

            // Cập nhật thông tin Template
            template.TemplateName = templateRequest.TemplateName.Trim();
            template.ImageUrl = templateRequest.ImageUrl.Trim();
            template.CreateAt = DateTime.UtcNow;

            // Lưu thay đổi vào cơ sở dữ liệu
            await _unitOfWork.Repository<Template>().Update(template, id);
            await _unitOfWork.CommitAsync();

            // Tạo và trả về đối tượng TemplateResponse
            var updatedTemplateResponse = new TemplateResponse
            {
                Id = template.Id,
                TemplateName = template.TemplateName,
                ImageUrl= template.ImageUrl,
                CreateAt= DateTime.UtcNow,
                
            };

            return updatedTemplateResponse;
        }
        public async Task<MessageResponse> DeleteTemplate(int id)
        {
            var template = await _unitOfWork.Repository<Template>()
                .GetAll()
                .FirstOrDefaultAsync(b => b.Id == id);

            if (template == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy Template", id.ToString());
            }


            template.Deleted = true;


            await _unitOfWork.Repository<Template>().Update(template, id);
            await _unitOfWork.CommitAsync();


        
       
            return new MessageResponse
            {
                Message = "Đã xóa",
            };
        }
        public async Task<int> GetTotalTemplateCount()
        {
            var count = await _unitOfWork.Repository<Template>()
                .GetAll()
                .CountAsync();

            return count;
        }
    }
}
