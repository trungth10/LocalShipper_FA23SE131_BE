using System;
using System.Collections.Generic;

namespace LocalShipper.Data.Models
{
    public partial class History
    {
        public int Id { get; set; }
        public string Action { get; set; } = null!;
        public string? HistoryDescription { get; set; }
        public int StoreId { get; set; }
        public DateTime? CreateAt { get; set; }

        public virtual Store Store { get; set; } = null!;
    }
}
