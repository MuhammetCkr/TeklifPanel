using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeklifPanel.Entity
{
    public class ProductOffer
    {
        public Product Product { get; set; }
        public int ProductId { get; set; }
        public Offer Offer { get; set; }
        public int OfferId { get; set; }
        public decimal Discount { get; set; } // İskonto
        public decimal ProductSellPrice { get; set; }
        public string Deadline { get; set; } // Teslim Tarihi
        public int Amount { get; set; }
        public string Currency { get; set; }

    }
}
