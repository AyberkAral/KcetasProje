using System.ComponentModel.DataAnnotations;

namespace KcetasWeb.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Ad Soyad zorunludur.")]
        public string AdSoyad { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string EPosta { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
        public string KullaniciAdi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kullanıcı tipi seçilmelidir.")]
        public short RolId { get; set; }

        // YENİ: Sadece Abone seçildiğinde zorunlu, controller'da manuel kontrol edilecek
        public string? AboneTuru { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [DataType(DataType.Password)]
        [MinLength(4, ErrorMessage = "Şifre en az 4 karakter olmalıdır.")]
        public string Sifre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre tekrarı zorunludur.")]
        [DataType(DataType.Password)]
        [Compare("Sifre", ErrorMessage = "Şifreler eşleşmiyor.")]
        public string SifreTekrar { get; set; } = string.Empty;
    }
}