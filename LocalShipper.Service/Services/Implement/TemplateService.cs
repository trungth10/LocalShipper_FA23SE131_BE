using AutoMapper;
using LocalShipper.Data.Models;
using LocalShipper.Data.UnitOfWork;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<List<TemplateResponse>> GetTemplate(int? id, string? templateName)
        {

            var templates = await _unitOfWork.Repository<Template>().GetAll()
                                                              .Where(b => id == 0 || b.Id == id)

                                                              .Where(b => string.IsNullOrWhiteSpace(templateName) || b.TemplateName.Contains(templateName))
                                                              .ToListAsync();
            var templateRespnses = _mapper.Map<List<Template>, List<TemplateResponse>>(templates);
            return templateRespnses;
        }
    }
}
