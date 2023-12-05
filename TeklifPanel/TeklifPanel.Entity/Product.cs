using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeklifPanel.Entity
{
    public class Product : BaseEntity
    {
        public Product()
        {
            ProductImages = new List<ProductImage>();
            ProductOffers = new List<ProductOffer>();
        }
        public string Name { get; set; }
        public string Code { get; set; }
        public decimal? BuyPrice { get; set; }
        public decimal? SellPrice { get; set; }
        public int Stock { get; set; }
        public string Detail { get; set; }
        public string Url { get; set; }
        public bool IsActive { get; set; }
        public Category Category { get; set; }
        public int CategoryId { get; set; }
        public List<ProductOffer> ProductOffers { get; set; }
        public List<ProductImage> ProductImages { get; set; }
        public decimal? Discount { get; set; }
        public string Unit { get; set; }
        public string Currency { get; set; }
        public byte[] QRCode { get; set; }
    }
}
