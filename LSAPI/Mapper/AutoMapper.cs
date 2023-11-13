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
            CreateMap<Shipper, ShipperResponse>(); 
            CreateMap<Order, OrderResponse>().ReverseMap();
            CreateMap<Order, OrderCreateResponse>().ReverseMap();
            CreateMap<Order, OrderWithShipperResponse>().ReverseMap();

            CreateMap<PackageAction, PackageActionResponse>().ReverseMap();
            CreateMap<PackageType, PackageTypeResponse>().ReverseMap();
            CreateMap<Store, StoreResponse>().ReverseMap();
            CreateMap<Template, TemplateResponse>().ReverseMap();
            CreateMap<Account, AccountResponse>().ReverseMap();
            CreateMap<Wallet, WalletResponse>().ReverseMap();
            CreateMap<Zone, ZoneResponse>().ReverseMap();
            CreateMap<Account, AccountInfoResponse>().ReverseMap();

            CreateMap<WalletTransaction, WalletTransactionResponse>().ReverseMap();
            CreateMap<RouteEdge, RouteEdgeResponse>().ReverseMap();
            CreateMap<RouteEdge, RouteEdgeWithShipperResponse>().ReverseMap();
        }
    }
}
