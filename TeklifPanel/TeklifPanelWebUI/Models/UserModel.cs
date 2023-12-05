using System.ComponentModel.DataAnnotations;

namespace TeklifPanelWebUI.Models
{
    public class UserModel
    {
        public  string Id { get; set; }
        [Required(ErrorMessage = "Bu alan zorunludur!")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Bu alan zorunludur!")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Bu alan zorunludur!")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Bu alan zorunludur!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Bu alan zorunludur!")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Şifreler uyuşmuyor!")]
        public string RePassword { get; set; }
        public bool IsOfferShow { get; set; }
    }
}
