using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeklifPanel.Data.Configurations;
using TeklifPanel.Entity;

namespace TeklifPanel.Data.Concrete.EfCore
{
    public class TeklifPanelContext : IdentityDbContext<User>
    {
        public TeklifPanelContext(DbContextOptions options) : base(options) { }
        //public TeklifPanelContext(DbContextOptions<TeklifPanelContext> options) : base(options) { }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<CompanySettings> CompanySettings { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerContact> CustomerContacts { get; set; }
        public DbSet<SystemLog> SystemLogs { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductOffer> ProductOffers { get; set; }
        public DbSet<Iban> Ibans { get; set; }
        public DbSet<OfferTable> OfferTables { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder
                .Entity<ProductOffer>()
                .HasKey(ut => new
                {
                    ut.ProductId,
                    ut.OfferId
                });

            modelBuilder.ApplyConfiguration(new ProductConfigurations());
            modelBuilder.ApplyConfiguration(new AddressConfigurations());
            modelBuilder.ApplyConfiguration(new CategoryConfigurations());
            modelBuilder.ApplyConfiguration(new CustomerConfigurations());
            modelBuilder.ApplyConfiguration(new CustomerContactConfigurations());
            modelBuilder.ApplyConfiguration(new LogConfigurations());
            modelBuilder.ApplyConfiguration(new OfferConfiguration());
            modelBuilder.ApplyConfiguration(new ProductImagesConfigurations());
            modelBuilder.ApplyConfiguration(new SettingsConfigurations());
        }

    }
}
