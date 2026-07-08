using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KcetasWeb.ViewModels
{
    public class TutanakGirisViewModel : IValidatableObject
    {
        public long IsEmriId { get; set; }
        public string IsEmriNo { get; set; } = null!;
        public string Tip { get; set; } = null!;

        [Required(ErrorMessage = "Tutanak numarası zorunludur")]
        public string TutanakNo { get; set; } = null!;

        [Required(ErrorMessage = "Saha sonucu zorunludur")]
        public string SahaSonucu { get; set; } = null!;

        public string? Gerekce { get; set; }

        [Required]
        public DateTime IslemTarihi { get; set; } = DateTime.Now;

        public string? MuhurNo { get; set; }

        // Sökme/Takma Endeksleri
        public string? EskiSayacNo { get; set; }
        public string? YeniSayacNo { get; set; }
        public decimal? EskiSonEndeksi { get; set; }
        public decimal? YeniIlkEndeksi { get; set; }

        // Açma/Kesme Endeksleri
        public decimal? KesmeEndeksi { get; set; }
        public decimal? AcmaEndeksi { get; set; }

        public bool IsSokmeTakma => Tip == "Sayaç Sökme" || Tip == "Sayaç Takma" || Tip == "Sayaç Değişim" || Tip == "SOKME" || Tip == "BAGLAMA" || Tip == "DEGISTIRME";
        public bool IsAcmaKesme => Tip == "Açma" || Tip == "Kesme" || Tip == "ACMA" || Tip == "KESME";

        // Belgedeki kural: "Seçilen iş emri tipine göre farklı alanlar zorunlu olmalı"
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            switch (Tip)
            {
                case "Kesme":
                case "KESME":
                    if (!KesmeEndeksi.HasValue)
                        yield return new ValidationResult("Kesme işleminde Kesme Endeksi zorunludur.", new[] { nameof(KesmeEndeksi) });
                    break;

                case "Açma":
                case "ACMA":
                    if (!AcmaEndeksi.HasValue)
                        yield return new ValidationResult("Açma işleminde Açma Endeksi zorunludur.", new[] { nameof(AcmaEndeksi) });
                    break;

                case "Sayaç Sökme":
                case "SOKME":
                    if (string.IsNullOrWhiteSpace(EskiSayacNo))
                        yield return new ValidationResult("Sayaç sökme işleminde Eski Sayaç Seri No zorunludur.", new[] { nameof(EskiSayacNo) });
                    if (!EskiSonEndeksi.HasValue)
                        yield return new ValidationResult("Sayaç sökme işleminde Eski Sayaç Son Endeks zorunludur.", new[] { nameof(EskiSonEndeksi) });
                    if (string.IsNullOrWhiteSpace(MuhurNo))
                        yield return new ValidationResult("Sayaç sökme işleminde Mühür No zorunludur.", new[] { nameof(MuhurNo) });
                    break;

                case "Sayaç Takma":
                case "BAGLAMA":
                    if (string.IsNullOrWhiteSpace(YeniSayacNo))
                        yield return new ValidationResult("Sayaç takma işleminde Yeni Sayaç Seri No zorunludur.", new[] { nameof(YeniSayacNo) });
                    if (!YeniIlkEndeksi.HasValue)
                        yield return new ValidationResult("Sayaç takma işleminde Yeni Sayaç İlk Endeks zorunludur.", new[] { nameof(YeniIlkEndeksi) });
                    if (string.IsNullOrWhiteSpace(MuhurNo))
                        yield return new ValidationResult("Sayaç takma işleminde Mühür No zorunludur.", new[] { nameof(MuhurNo) });
                    break;

                case "Sayaç Değişim":
                case "DEGISTIRME":
                    if (string.IsNullOrWhiteSpace(EskiSayacNo))
                        yield return new ValidationResult("Sayaç değişiminde Eski Sayaç Seri No zorunludur.", new[] { nameof(EskiSayacNo) });
                    if (!EskiSonEndeksi.HasValue)
                        yield return new ValidationResult("Sayaç değişiminde Eski Sayaç Son Endeks zorunludur.", new[] { nameof(EskiSonEndeksi) });
                    if (string.IsNullOrWhiteSpace(YeniSayacNo))
                        yield return new ValidationResult("Sayaç değişiminde Yeni Sayaç Seri No zorunludur.", new[] { nameof(YeniSayacNo) });
                    if (!YeniIlkEndeksi.HasValue)
                        yield return new ValidationResult("Sayaç değişiminde Yeni Sayaç İlk Endeks zorunludur.", new[] { nameof(YeniIlkEndeksi) });
                    if (string.IsNullOrWhiteSpace(MuhurNo))
                        yield return new ValidationResult("Sayaç değişiminde Mühür No zorunludur.", new[] { nameof(MuhurNo) });
                    break;
            }
        }
    }
}