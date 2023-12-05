using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using TeklifPanel.Entity;
using TeklifPanelWebUI.Models;
using Microsoft.Ajax.Utilities;

namespace TeklifPanelWebUI.Controllers
{
    public class PasswordController : Controller
    {
        private readonly UserManager<User> _userManager;

        public PasswordController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult PasswordEdit()
        {
            var userId = HttpContext.Session.GetString("UserId");
            ChangePassword changePassword = new ChangePassword()
            {
                Id = userId
            };
            return View(changePassword);
        }
        [HttpPost]
        public async Task<IActionResult> PasswordEdit(ChangePassword changePassword)
        {

            var user = await _userManager.FindByIdAsync(changePassword.Id);
            var result = await _userManager.ChangePasswordAsync(user, changePassword.OldPassword, changePassword.NewPassword);
            if (result.Succeeded)
            {
                TempData["Message"] = "Şifreniz değiştirildi!";
                return View(new { changePassword.Id });
            }
            TempData["Hata"] = "Şifreniz değiştirilemedi!";
            return View(new { changePassword.Id });
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var getUser = await _userManager.FindByEmailAsync(email);

            if (getUser == null)
                return Json(new { status = 400 });

            HttpContext.Session.SetString("UserId", getUser.Id);
            return Json(new { status = 200 });

        }

        public IActionResult PasswordReset(int code)
        {
            var securityCode = HttpContext.Session.GetInt32("SecurityCode");
            if (code != securityCode)
                return Json(new { status = 400 });

            return Json(new { status = 200 });

        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(UserModel userModel)
        {
            var userId = HttpContext.Session.GetString("UserId");
            var user = await _userManager.FindByIdAsync(userId);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, token, userModel.Password);
            if (result.Succeeded)
            {
                TempData["Message"] = "Şifreniz değiştirildi!";
                return RedirectToAction("Index", "Login");
            }
            TempData["Hata"] = "Şifreniz değiştirilemedi!";
            return View();
        }

        public async Task<ActionResult> SecurityCode()
        {
            var random = new Random();
            var securityCode = random.Next(100000, 999999);
            HttpContext.Session.SetInt32("SecurityCode", securityCode);

            var userId = HttpContext.Session.GetString("UserId");
            var user = await _userManager.FindByIdAsync(userId);

            var smtpServer = "eposta.ttrbilisim.com";
            var smtpPort = 587;
            var smtpUsername = "noreply@ttr.gen.tr";
            var smtpPassword = "$TY9qajt";
            var senderEmail = user.Email;

            var fromAddress = new MailAddress(smtpUsername, "Güvenlik Kodu");
            var toAddress = new MailAddress(senderEmail);

            SmtpClient smtp = new SmtpClient();

            smtp.Host = smtpServer;
            smtp.Port = smtpPort;
            smtp.EnableSsl = false;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(smtpUsername, smtpPassword);

            var message = new MailMessage(fromAddress, toAddress);
            message.Body = String.Format(@"Merhaba, <br/><b> Tek kullanımlık şifreniz: " + securityCode + "</b>");
            message.Subject = "Şifre Sıfırlama";
            message.IsBodyHtml = true;
            smtp.Send(message);

            return Json(new { status = 200 });
        }
    }
}
