using TeklifPanel.Entity;

namespace TeklifPanelWebUI.ViewModels
{
    public class OfferViewModel
    {
        public string Pdf { get; set; }
        public int OfferNumber { get; set; } //Teklif Numarasi
        public decimal? SellPrice { get; set; }
        public decimal? KDV { get; set; }
        public string DateOfOffer { get; set; }
        public CompanySettingsViewModel CompanySettingsViewModel { get; set; }
        public List<ProductViewModel> ProductsViewModel { get; set; }
        public Company Company { get; set; }
        public Customer Customer { get; set; }
        public CustomerContact CustomerContact { get; set; }
        public List<OfferTable> OfferTables { get; set; }
        public string Deadline { get; set; }
        public string CurrencyType { get; set; }
        public string Currency { get; set; }
    }
}
