using LocalShipper.Data.Models;
using LocalShipper.Service.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Response
{
    public class AccountInfoShippperResponse
    {
        public string Role { get; set; }
        public int AccountId { get; set; }
        public string Fullname { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
        public int ShipperId { get; set; }
        public string AddressShipper { get; set; }
        public int? TransportId { get; set; }
        public int? ZoneId { get; set; }
        public int? WalletId { get; set; }
        public bool? Active { get; set; } = null;
        public string FcmToken { get; set; }
        public DateTime? CreateDate { get; set; } = null;
        public string ImageUrl { get; set; }
        public string Password { get; set; }
       
       
    }
    public class AccountInfoStoreResponse
    {
        public string Role { get; set; }
        public int AccountId { get; set; }
        public string Fullname { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public string StoreAddress { get; set; }
        public TimeSpan? OpenTime { get; set; }
        public TimeSpan? CloseTime { get; set; }
        public string StoreDescription { get; set; }
        public int? Status { get; set; }
        public int? TemplateId { get; set; }
        public int? ZoneId { get; set; }
        public int WalletId { get; set; }
        public string FcmToken { get; set; }
        public DateTime? CreateDate { get; set; } = null;
        public string ImageUrl { get; set; }
        public string Password { get; set; }


    }
    public class AccountInfoResponse 
    
    {
        public string Role { get; set; }
        public int Id { get; set; }
        public string Fullname { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
        public bool? Active { get; set; } = null;
        public string FcmToken { get; set; }
        public DateTime? CreateDate { get; set; } = null;
        public string ImageUrl { get; set; }
        public string Password { get; set; }
       
    }


}
