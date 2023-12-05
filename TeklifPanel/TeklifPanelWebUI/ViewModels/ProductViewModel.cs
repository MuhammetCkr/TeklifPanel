using TeklifPanel.Entity;

namespace TeklifPanelWebUI.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public decimal? BuyPrice { get; set; }
        public decimal? SellPrice { get; set; }
        public int Stock { get; set; }
        public int Amount { get; set; }
        public bool IsSeleted { get; set; }
        public decimal? Discount { get; set; }
        public string Detail { get; set; }
        public string Url { get; set; }
        public bool IsActive { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public decimal KDV { get; set; }
        public List<ProductOffer> ProductOffers { get; set; }
        public List<ProductImage> ProductImages { get; set; }
        public List<string> Images { get; set; }
        public User User { get; set; }
        public int CompanyId { get; set; }
        public string Unit { get; set; }
        public string Currency { get; set; }
        public List<decimal> OfferDisCount { get; set; }
        public byte[] QRCode { get; set; }
        public string Deadline { get; set; }

    }
}
