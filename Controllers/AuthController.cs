using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using KcetasWeb.Models;                 // RegisterViewModel için
using KcetasWeb.Models.entities;        // Kullanici, Rol için
using KcetasWeb.Services.Interfaces;    // IKullaniciDeposu, RolListesi için

namespace KcetasWeb.Controllers
{
    public class AuthController : Controller
    {
        private readonly IKullaniciDeposu _kullaniciDeposu;
        private readonly PasswordHasher<Kullanici> _sifreHasher = new();

        public AuthController(IKullaniciDeposu kullaniciDeposu)
        {
            _kullaniciDeposu = kullaniciDeposu;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string kullaniciAdi, string sifre)
        {
            if (kullaniciAdi == "admin" && sifre == "123")
            {
                await GirisYap(kullaniciAdi, "Yonetici");
                return RedirectToAction("Index", "Home");
            }
            else if (kullaniciAdi == "personel" && sifre == "123")
            {
                await GirisYap("Ahmet (Gişe)", "Kullanici");
                return RedirectToAction("Index", "Home");
            }

            var kayitliKullanici = _kullaniciDeposu.BulKullaniciAdiIle(kullaniciAdi);
            if (kayitliKullanici != null)
            {
                var sonuc = _sifreHasher.VerifyHashedPassword(kayitliKullanici, kayitliKullanici.SifreHash, sifre);
                if (sonuc == PasswordVerificationResult.Success)
                {
                    var rol = RolListesi.BulRolId(kayitliKullanici.RolId);
                    var rolAdi = rol?.RolAdi ?? "Kullanici";

                    await GirisYap(kayitliKullanici.AdSoyad, rolAdi);
                    return RedirectToAction("Index", "Home");
                }
            }

            TempData["HataMesaji"] = "Kullanıcı adı veya şifre hatalı!";
            return View();
        }

        private async Task GirisYap(string ad, string rol)
        {
            var kartBilgileri = new List<Claim>
            {
                new Claim(ClaimTypes.Name, ad),
                new Claim(ClaimTypes.Role, rol)
            };

            var kullaniciKimligi = new ClaimsIdentity(kartBilgileri, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(kullaniciKimligi));
        }

        public IActionResult Register()
        {
            ViewBag.Roller = RolListesi.Roller.Where(r => r.RolId != 1).ToList();
            return View(new RegisterViewModel());
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            ViewBag.Roller = RolListesi.Roller.Where(r => r.RolId != 1).ToList();

            // Abone seçildiyse Müşteri Türü zorunlu
            var seciliRol = RolListesi.BulRolId(model.RolId);
            if (seciliRol?.RolAdi == "Abone" && string.IsNullOrWhiteSpace(model.AboneTuru))
            {
                ModelState.AddModelError(nameof(model.AboneTuru), "Müşteri türü seçilmelidir (Mesken / İş Yeri).");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (_kullaniciDeposu.KullaniciAdiVarMi(model.KullaniciAdi))
            {
                ModelState.AddModelError(nameof(model.KullaniciAdi), "Bu kullanıcı adı zaten alınmış.");
                return View(model);
            }

            var yeniKullanici = new Kullanici
            {
                AdSoyad = model.AdSoyad,
                EPosta = model.EPosta,
                KullaniciAdi = model.KullaniciAdi,
                RolId = model.RolId,
                AboneTuru = seciliRol?.RolAdi == "Abone" ? model.AboneTuru : null,
                Durum = "Aktif",
                CreatedAt = DateTime.Now
            };

            yeniKullanici.SifreHash = _sifreHasher.HashPassword(yeniKullanici, model.Sifre);

            _kullaniciDeposu.Ekle(yeniKullanici);

            TempData["BasariMesaji"] = "Hesabınız oluşturuldu! Şimdi giriş yapabilirsiniz.";
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}