using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeklifPanel.Entity
{
    public class Customer : BaseEntity
    {
        public Customer()
        {
            CustomerContacts = new List<CustomerContact>();
            Offers = new List<Offer>();
        }
        public string Name { get; set; }
        public int TaxNumber { get; set; } // Vergi numarası
        public string TaxOffice { get; set; } // Vergi Dairesi
        public string Note { get; set; }
        public string SpecialNote { get; set; }
        public string Phone { get; set; }
        public decimal? Discount { get; set; } // İskonto
        public List<CustomerContact> CustomerContacts { get; set; }
        public List<Offer> Offers { get; set; }
        public string OpenAddress { get; set; }
        public string Email { get; set; }
    }
}
