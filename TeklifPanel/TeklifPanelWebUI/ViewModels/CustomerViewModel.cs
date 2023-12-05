using TeklifPanel.Entity;

namespace TeklifPanelWebUI.ViewModels
{
    public class CustomerViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TaxNumber { get; set; } // Vergi numarası
        public string TaxOffice { get; set; } // Vergi Dairesi
        public string Note { get; set; }
        public string SpecialNote { get; set; }
        public string Phone { get; set; }
        public decimal? Discount { get; set; } // İskonto
        public int CompanyId { get; set; }
        public string OpenAddress { get; set; }
        public string Email { get; set; }
        public List<string> ContactName { get; set; }
        public List<string> ContactEmail { get; set; }
        public List<string> ContactPhone { get; set; }
        public List<string> Department { get; set; }
        public List<int> ContactId { get; set; }
        public List<CustomerContact> CustomerContacts { get; set; }
        public Company Company { get; set; }
    }
}
