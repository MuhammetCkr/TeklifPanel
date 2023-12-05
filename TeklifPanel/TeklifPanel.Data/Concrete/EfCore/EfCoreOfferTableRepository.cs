using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeklifPanel.Data.Abstract;
using TeklifPanel.Entity;

namespace TeklifPanel.Data.Concrete.EfCore
{
    public class EfCoreOfferTableRepository : EfCoreGenericRepository<OfferTable>, IOfferTableRepository
    {
        public EfCoreOfferTableRepository(TeklifPanelContext _dbContext) : base(_dbContext)
        {

        }
        private TeklifPanelContext context
        {
            get
            {
                return _dbContext as TeklifPanelContext;
            }
        }

        public async Task<bool> CreateCompanyOfferTableAsync(int companyId)
        {
            var offerTableProductCode = new OfferTable()
            {
                MenuName = "Ürün Kodu",
                CompanyId = companyId,
                IsShow = true,
                SiraNo = 1,
                Name = "productCode"
            };

            var offerTableProductName = new OfferTable()
            {
                MenuName = "Ürün Adı",
                CompanyId = companyId,
                IsShow = true,
                SiraNo = 2,
                Name = "productName"
            };

            var offerTableProductDetails = new OfferTable()
            {
                MenuName = "Ürün Açıklama",
                CompanyId = companyId,
                IsShow = true,
                SiraNo = 3,
                Name = "productDetails"
            };

            var offerTablePrice = new OfferTable()
            {
                MenuName = "Miktar",
                CompanyId = companyId,
                IsShow = true,
                SiraNo = 4,
                Name = "productAmount"
            };

            var offerTableUnit = new OfferTable()
            {
                MenuName = "Birim",
                CompanyId = companyId,
                IsShow = true,
                SiraNo = 5,
                Name = "productUnit"
            };

            var offerTableKDV = new OfferTable()
            {
                MenuName = "KDV",
                CompanyId = companyId,
                IsShow = true,
                SiraNo = 6,
                Name = "productKdv"
            };

            var offerTableSellPrice = new OfferTable()
            {
                MenuName = "Ürün Satış Fiyatı",
                CompanyId = companyId,
                IsShow = true,
                SiraNo = 7,
                Name = "productSellPrice"
            };

            var offerTableDiscount = new OfferTable()
            {
                MenuName = "İskonto",
                CompanyId = companyId,
                IsShow = true,
                SiraNo = 8,
                Name = "productDiscount"
            };

            var offerTableDiscountPrice = new OfferTable()
            {
                MenuName = "İskontolu Fiyatı",
                CompanyId = companyId,
                IsShow = true,
                SiraNo = 9,
                Name = "productDiscountPrice"
            };

            var offerTableQRCodu = new OfferTable()
            {
                MenuName = "QR Kod",
                CompanyId = companyId,
                IsShow = true,
                SiraNo = 10,
                Name = "productQRCode"
            };

            var offerTableDeadline = new OfferTable()
            {
                MenuName = "Termin Süresi",
                CompanyId = companyId,
                IsShow = true,
                SiraNo = 11,
                Name = "productDeadline"
            };

            var offerTableImage = new OfferTable()
            {
                MenuName = "Ürün Resmi",
                CompanyId = companyId,
                IsShow = true,
                SiraNo = 12,
                Name = "productImage"
            };

            await context.AddRangeAsync(offerTableDiscount, offerTableDiscountPrice, offerTableKDV, offerTablePrice, offerTableProductCode, offerTableProductDetails, offerTableProductName, offerTableQRCodu, offerTableSellPrice, offerTableUnit, offerTableDeadline, offerTableImage);
            var result = await context.SaveChangesAsync();
            return result > 0;
        }
    }
}
