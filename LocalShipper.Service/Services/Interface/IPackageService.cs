﻿using LocalShipper.Service.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Interface
{
    public interface IPackageService
    {
        Task<List<PackageResponse>> GetPackage(int? batchId, int? id, int? status, int? actionId, int? typeId, string? customerName);
    }
}
