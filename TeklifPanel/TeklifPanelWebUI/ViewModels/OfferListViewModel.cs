using TeklifPanel.Entity;

namespace TeklifPanelWebUI.ViewModels
{
    public class OfferListViewModel
    {
        public List<Offer> Offers { get; set; }
        public List<Customer> Customers { get; set; }
        public User User { get; set; }
    }
}
