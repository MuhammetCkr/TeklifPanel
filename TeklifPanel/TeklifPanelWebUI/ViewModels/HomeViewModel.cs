using TeklifPanel.Entity;

namespace TeklifPanelWebUI.ViewModels
{
    public class HomeViewModel
    {
        public List<Offer> Offers { get; set; }
        public List<Log> Logs { get; set; }
        public List<Customer> CustomerList { get; set; }
        public User User { get; set; }
        public decimal USD { get; set; }
        public decimal EURO { get; set; }
        public decimal GBP { get; set; }
        public decimal TL { get; set; }
        public decimal USDUser { get; set; }
        public decimal EUROUser { get; set; }
        public decimal GBPUser { get; set; }
        public decimal TLUser { get; set; }
    }
}
