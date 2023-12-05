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
    public class EfCoreCompanySettingsRepository : EfCoreGenericRepository<CompanySettings>, ICompanySettingsRepository
    {
        public EfCoreCompanySettingsRepository(TeklifPanelContext _dbContext) : base(_dbContext) { }
        private TeklifPanelContext context
        {
            get { return _dbContext as TeklifPanelContext; }
        }

        public async Task<bool> CreateCompanySettingsAsync(int companyId)
        {
            var recipientEmail = new CompanySettings()
            {
                CompanyId = companyId,
                Parameter = "AliciEmail",
                Value = "",
                Type = "MailAyarlari"
            };

            var emailServer = new CompanySettings()
            {
                CompanyId = companyId,
                Parameter = "EmailSunucu",
                Value = "",
                Type = "MailAyarlari"
            };

            var emailServerPort = new CompanySettings()
            {
                CompanyId = companyId,
                Parameter = "EmailSunucuPort",
                Value = "",
                Type = "MailAyarlari"
            };

            var emailUsername = new CompanySettings()
            {
                CompanyId = companyId,
                Parameter = "EmailKullaniciAdi",
                Value = "",
                Type = "MailAyarlari"
            };

            var emailPassword = new CompanySettings()
            {
                CompanyId = companyId,
                Parameter = "EmailParola",
                Value = "",
                Type = "MailAyarlari"
            };

            var logo = new CompanySettings()
            {
                CompanyId = companyId,
                Parameter = "Logo",
                Value = "",
                Type = "LogoAyarlari"
            };

            var logo2 = new CompanySettings()
            {
                CompanyId = companyId,
                Parameter = "Logo2",
                Value = "",
                Type = "LogoAyarlari"
            };

            var phone = new CompanySettings()
            {
                CompanyId = companyId,
                Parameter = "TelNo",
                Value = "",
                Type = "TelefonAyarlari"
            };

            var fax = new CompanySettings()
            {
                CompanyId = companyId,
                Parameter = "FaxNo",
                Value = "",
                Type = "TelefonAyarlari"
            };

            var address = new CompanySettings()
            {
                CompanyId = companyId,
                Parameter = "Adres",
                Value = "",
                Type = "AdresAyarlari"
            };

            var note = new CompanySettings()
            {
                CompanyId = companyId,
                Parameter = "Not",
                Value = "",
                Type = "FirmaAyarlari"
            };


            await context.AddRangeAsync(recipientEmail, emailServer, emailServerPort, emailUsername, emailPassword, logo, logo2, phone, fax, address, note);

            var result = await context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<List<CompanySettings>> GetAllCompanySettingsAsync(int companyId)
        {
            var companySettings = await context.CompanySettings.Where(c => c.CompanyId == companyId).ToListAsync();
            return companySettings;
        }

        public async Task<bool> UpdateCompanySettingAsync(List<CompanySettings> settings)
        {
            context.CompanySettings.UpdateRange(settings.ToArray());
            var result = await context.SaveChangesAsync();
            return result > 0;
        }
    }
}
