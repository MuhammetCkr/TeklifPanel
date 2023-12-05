using TeklifPanel.Entity;

namespace TeklifPanelWebUI.ViewModels
{
    public class CompanySettingsViewModel
    {
        public int Id { get; set; }
        public string EmailAddress { get; set; }
        public string RecipientEmail { get; set; } // Alıcı Email
        public string EmailServer { get; set; }
        public string EmailServerPort { get; set; }
        public string EmailUsername { get; set; }
        public string EmailPassword { get; set; }
        public string Logo { get; set; }
        public string Logo2 { get; set; }
        public string PhoneNumber { get; set; }
        public string FaxNumber { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }
        public ICollection<Iban> Ibans { get; set; }
        public List<OfferTable> OfferTables { get; set; }
    }
}
