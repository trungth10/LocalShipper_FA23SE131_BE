using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Response
{
    public class WalletTransactionResponse
    {
        public int Id { get; set; }
        public string TransactionType { get; set; }
        public int FromWalletId { get; set; }
        public int ToWalletId { get; set; }
        public decimal Amount { get; set; }
        public DateTime? TransactionTime { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? Active { get; set; }


        public WalletResponse FromWallet { get; set; }
        public WalletResponse ToWallet { get; set; }
        public OrderResponse Order { get; set; }

    }
}
