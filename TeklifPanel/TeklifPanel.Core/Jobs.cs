using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using QRCoder;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Xml;
using System.Drawing;

namespace TeklifPanel.Core
{
    public static class Jobs
    {
        public const string ImageRoute = "/Content/images/C";
        public const string PdfRoute = "/Content/pdfs/C";
        public const string ExcelRoute = "/Content/excel/C";

        public static string UploadExcel(IFormFile file, string url, int companyId)
        {
            var extension = Path.GetExtension(file.FileName);
            var randomName = $"{url}-{Guid.NewGuid()}{extension}";

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Content/excel/C" + companyId);
            Directory.CreateDirectory(folderPath);
            var path = Path.Combine(folderPath, randomName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            return path;
        }

        public static string UploadImage(IFormFile file, string url, int companyId)
        {
            var extension = Path.GetExtension(file.FileName);
            var randomName = $"{url}-{Guid.NewGuid()}{extension}";

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Content/images/C" + companyId);
            Directory.CreateDirectory(folderPath);
            var path = Path.Combine(folderPath, randomName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            return randomName;
        }

        public static string UploadPdf(IFormFile file, string customerName, int companyId, string companyName)
        {
            var company = companyName.Split(" ");
            var customer = customerName.Split(" ");
            var randomName = $"{company[0]}-{customer[0]}-{DateTime.Now.ToShortDateString()}";

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Content/pdfs/C" + companyId);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath); // Klasörü oluşturur
            }

            var pdfPath = Path.Combine(folderPath, randomName);

            using (var stream = new FileStream(pdfPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            var result = CreatePdf(folderPath, randomName);

            return result;
        }



        public static string CreatePdf(string folderPath, string randomName)
        {
            // Yeni bir PDF belgesi oluşturun
            PdfDocument pdfDocument = new PdfDocument();

            PdfPage pdfPage = pdfDocument.AddPage();
            pdfPage.Size = PageSize.A4;
            pdfPage.Orientation = PageOrientation.Landscape;

            // PDF sayfası oluşturun
            XGraphics gfx = XGraphics.FromPdfPage(pdfPage);

            // PNG dosyasını yükleyin ve PDF sayfasına çizin
            var pngPath = Path.Combine(folderPath, randomName); // PNG dosyasının yolunu belirtin
            XImage pngImage = XImage.FromFile(pngPath);

            // Örnek boyutlar
            double width = pdfPage.Width;
            double height = pdfPage.Height;

            // PNG dosyasını PDF sayfasına çizin, tam sayfa olarak
            gfx.DrawImage(pngImage, 0, 0, width, height);

            // PDF dosyasını oluşturun
            var pdfFilePath = Path.Combine(folderPath, randomName + ".pdf");
            pdfDocument.Save(pdfFilePath);

            return pdfFilePath;
        }

        public static string MakeUrl(string url)
        {
            url = url.Replace("I", "i");
            url = url.Replace("İ", "i");
            url = url.Replace("ı", "i");

            url = url.ToLower();

            url = url.Replace("ö", "o");
            url = url.Replace("ü", "u");
            url = url.Replace("ğ", "g");
            url = url.Replace("ç", "c");
            url = url.Replace("ş", "s");

            url = url.Replace("/", "");
            url = url.Replace("\\", "");
            url = url.Replace(".", "");
            url = url.Replace(" ", "-");

            return url;
        }

        public static string MakeUserName(string userName)
        {
            userName = userName.Replace("ı", "i");
            userName = userName.Replace("ö", "o");
            userName = userName.Replace("ü", "u");
            userName = userName.Replace("ğ", "g");
            userName = userName.Replace("ç", "c");
            userName = userName.Replace("ş", "s");

            userName = userName.ToUpper();

            userName = userName.Replace("İ", "I");
            userName = userName.Replace("Ö", "O");
            userName = userName.Replace("Ü", "U");
            userName = userName.Replace("Ğ", "G");
            userName = userName.Replace("Ç", "C");
            userName = userName.Replace("Ş", "S");

            userName = userName.Replace("/", "");
            userName = userName.Replace("\\", "");
            userName = userName.Replace(".", "");
            userName = userName.Replace(" ", "-");

            return userName;
        }


        public static byte[] GenerateQrCodeFromUrl(string url)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);

            using (Bitmap qrCodeImage = qrCode.GetGraphic(10))
            using (MemoryStream stream = new MemoryStream())
            {
                qrCodeImage.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

    }
}
