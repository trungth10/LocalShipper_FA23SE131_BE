using System;
using System.Collections.Generic;

namespace LocalShipper.Data.Models
{
    public partial class TransportType
    {
        public TransportType()
        {
            Transports = new HashSet<Transport>();
        }

        public int Id { get; set; }
        public string TransportType1 { get; set; } = null!;
        public DateTime? CreateAt { get; set; }

        public virtual ICollection<Transport> Transports { get; set; }
    }
}
