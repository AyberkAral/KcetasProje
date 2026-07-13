using System.ComponentModel.DataAnnotations;

namespace KcetasWeb.ViewModels
{
    public class AboneEkleViewModel
    {
        [Required]
        public bool IsTuzel { get; set; } // false: Gerçek, true: Tüzel

        [Required(ErrorMessage = "Ad Soyad / Ünvan alanı zorunludur.")]
        public string AdSoyadUnvan { get; set; } = null!;

        public string? TCKN { get; set; }
        public string? VKN { get; set; }

        [Required(ErrorMessage = "Telefon zorunludur.")]
        [Phone]
        public string Telefon { get; set; } = null!;

        [Required(ErrorMessage = "Mail zorunludur.")]
        [EmailAddress]
        public string Mail { get; set; } = null!;

        [Required(ErrorMessage = "Tebligat Adresi zorunludur.")]
        public string TebligatAdresi { get; set; } = null!;

        [Range(typeof(bool), "true", "true", ErrorMessage = "KVKK Onayı vermek zorunludur.")]
        public bool KvkkOnayi { get; set; }
    }
}
