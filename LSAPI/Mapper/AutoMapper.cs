using AutoMapper;
using LocalShipper.Data.Models;
using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using Microsoft.AspNetCore.Mvc;
using static System.Collections.Specialized.BitVector32;

namespace LSAPI.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Shipper, ShipperResponse>().ReverseMap();
            CreateMap<Order, OrderResponse>().ReverseMap();
        }
    }
}
