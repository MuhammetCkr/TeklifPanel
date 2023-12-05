using Microsoft.Extensions.Options;
using System.Xml;
using TeklifPanelWebUI.Models;

namespace TeklifPanelWebUI
{
    public class Currency
    {
        private IOptions<CurrenciesModel> _defaultData;
        private readonly IWritableOptions<CurrenciesModel> _writableOptions;
        private readonly IConfiguration _configuration;

        public Currency(IOptions<CurrenciesModel> defaultData, IWritableOptions<CurrenciesModel> writableOptions, IConfiguration configuration)
        {
            _defaultData = defaultData;
            _writableOptions = writableOptions;
            _configuration = configuration;
        }

        public void UpdateCurrencies()
        {
            try
            {
                var currencyInf = "https://www.tcmb.gov.tr/kurlar/today.xml";
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(currencyInf);
                if (xmlDoc != null)
                {
                    var bulten = Convert.ToString(xmlDoc.SelectSingleNode("//Tarih_Date").Attributes["Tarih"].Value);

                    var usdBanknoteSelling = xmlDoc.SelectSingleNode("Tarih_Date/Currency [@CrossOrder='0']/BanknoteSelling").InnerText;
                    var eurBanknoteSelling = xmlDoc.SelectSingleNode("Tarih_Date/Currency [@CrossOrder='9']/BanknoteSelling").InnerText;
                    var gbpBanknoteSelling = xmlDoc.SelectSingleNode("Tarih_Date/Currency [@CrossOrder='10']/BanknoteSelling").InnerText;

                    var usdBanknoteBuying = xmlDoc.SelectSingleNode("Tarih_Date/Currency [@CrossOrder='0']/BanknoteBuying").InnerText;
                    var eurBanknoteBuying = xmlDoc.SelectSingleNode("Tarih_Date/Currency [@CrossOrder='9']/BanknoteBuying").InnerText;
                    var gbpBanknoteBuying = xmlDoc.SelectSingleNode("Tarih_Date/Currency [@CrossOrder='10']/BanknoteBuying").InnerText;

                    var usdForexSelling = xmlDoc.SelectSingleNode("Tarih_Date/Currency [@CrossOrder='0']/ForexSelling").InnerText;
                    var eurForexSelling = xmlDoc.SelectSingleNode("Tarih_Date/Currency [@CrossOrder='9']/ForexSelling").InnerText;
                    var gbpForexSelling = xmlDoc.SelectSingleNode("Tarih_Date/Currency [@CrossOrder='10']/ForexSelling").InnerText;

                    var usdForexBuying = xmlDoc.SelectSingleNode("Tarih_Date/Currency [@CrossOrder='0']/ForexBuying").InnerText;
                    var eurForexBuying = xmlDoc.SelectSingleNode("Tarih_Date/Currency [@CrossOrder='9']/ForexBuying").InnerText; 
                    var gbpForexBuying = xmlDoc.SelectSingleNode("Tarih_Date/Currency [@CrossOrder='10']/ForexBuying").InnerText;

                    _writableOptions.Update(opt =>
                    {
                        opt.BanknoteSellingUSD = usdBanknoteSelling;
                        opt.BanknoteSellingEUR = eurBanknoteSelling;
                        opt.BanknoteSellingGBP = gbpBanknoteSelling;
                        
                        opt.BanknoteBuyingUSD = usdBanknoteBuying;
                        opt.BanknoteBuyingEUR = eurBanknoteBuying;
                        opt.BanknoteBuyingGBP = gbpBanknoteBuying;
                        
                        opt.ForexSellingUSD = usdForexSelling;
                        opt.ForexSellingEUR = eurForexSelling;
                        opt.ForexSellingGBP = gbpForexSelling;

                        opt.ForexBuyingUSD = usdForexBuying;
                        opt.ForexBuyingEUR = eurForexBuying;
                        opt.ForexBuyingGBP = gbpForexBuying;

                        opt.Bulten = bulten;
                        opt.TL = "1";
                    });
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
