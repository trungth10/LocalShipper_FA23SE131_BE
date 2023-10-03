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
            CreateMap<PackageAction, PackageActionResponse>().ReverseMap();
            CreateMap<Package, PackageResponse>().ReverseMap();
            CreateMap<PackageType, PackageTypeResponse>().ReverseMap();
            CreateMap<Store, StoreResponse>().ReverseMap();
            CreateMap<Template, TemplateResponse>().ReverseMap();
            CreateMap<Batch, BatchResponse>().ReverseMap();
            CreateMap<Account, AccountResponse>().ReverseMap();
            CreateMap<Wallet, WalletResponse>().ReverseMap();
            CreateMap<Zone, ZoneResponse>().ReverseMap();
        }
    }
}
