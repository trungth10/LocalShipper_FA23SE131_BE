﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTO.Request
{
    public class PagingRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string KeySearch { get; set; } = "";
        public string SearchBy { get; set; } = "";
        public SortOrder SortType { get; set; } = SortOrder.Descending;
        public string ColName { get; set; } = "Id";
    }
}
