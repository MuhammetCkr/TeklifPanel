using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeklifPanel.Entity
{
    public class Offer : BaseEntity
    {
        public Offer()
        {
            ProductOffers = new List<ProductOffer>();
        }
        public DateTime DateOfOffer { get; set; }
        public DateTime DateOfForwarded { get; set; } // İletildi/teslime edildi tarihi
        public DateTime DateOfRead { get; set; } // Okundu tarihi
        public bool IsForwarded { get; set; } // İletildi mi?
        public bool IsRead { get; set; } // Okundu mu?
        public string Pdf { get; set; }
        public int OfferNumber { get; set; } //Teklif Numarasi
        public Customer Customer { get; set; }
        public int CustomerId { get; set; }
        public CustomerContact CustomerContact { get; set; }
        public int CustomerContactId { get; set; }
        public List<ProductOffer> ProductOffers { get; set; }
        public User User { get; set; }
        public string UserId { get; set; }
        public decimal? Discount { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsApproved { get; set; }
        public string Note { get; set; }
        public bool IsSend { get; set; }
        public bool IsDeleted { get; set; }
        public string CurrenyType { get; set; }
        public string Currency { get; set; }

    }
}
