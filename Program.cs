using Microsoft.AspNetCore.Authentication.Cookies;
using KcetasWeb.Services.Interfaces;
using KcetasWeb.Services.Mock;

var builder = WebApplication.CreateBuilder(args);

// Sisteme MVC Controller yapılarını ekliyoruz
builder.Services.AddControllersWithViews();

// 1. SİSTEME COOKIE (ÇEREZ) KİMLİK DOĞRULAMASINI TANITIYORUZ
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(ayarlar =>
    {
        ayarlar.LoginPath = "/Auth/Login";       // Yetkisiz biri girerse buraya at
        ayarlar.LogoutPath = "/Auth/Logout";     // Çıkış yapınca buraya at
        ayarlar.AccessDeniedPath = "/Auth/Yetkisiz";
    });

// 2. API SERVİSLERİNİN DI CONTAINER'A KAYDI
var baseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "http://172.10.38.27:5050";
builder.Services.AddHttpClient<IIsEmriService, KcetasWeb.Services.Api.ApiIsEmriService>(client => client.BaseAddress = new Uri(baseUrl));
builder.Services.AddHttpClient<IEndeksOkumaService, KcetasWeb.Services.Api.ApiEndeksOkumaService>(client => client.BaseAddress = new Uri(baseUrl));
builder.Services.AddHttpClient<IFaturaService, KcetasWeb.Services.Api.ApiFaturaService>(client => client.BaseAddress = new Uri(baseUrl));
builder.Services.AddHttpClient<IOutboxService, KcetasWeb.Services.Api.ApiOutboxService>(client => client.BaseAddress = new Uri(baseUrl));
builder.Services.AddHttpClient<IKullaniciDeposu, KcetasWeb.Services.Api.ApiKullaniciDeposu>(client => client.BaseAddress = new Uri(baseUrl));
builder.Services.AddHttpClient<ITuketimNoktasiService, KcetasWeb.Services.Api.ApiTuketimNoktasiService>(client => client.BaseAddress = new Uri(baseUrl));
builder.Services.AddHttpClient<ISozlesmeService, KcetasWeb.Services.Api.ApiSozlesmeService>(client => client.BaseAddress = new Uri(baseUrl));
builder.Services.AddHttpClient<ISayacService, KcetasWeb.Services.Api.ApiSayacService>(client => client.BaseAddress = new Uri(baseUrl));
builder.Services.AddHttpClient<IAboneService, KcetasWeb.Services.Api.ApiAboneService>(client => client.BaseAddress = new Uri(baseUrl));
builder.Services.AddHttpClient<IAuditLogService, KcetasWeb.Services.Api.ApiAuditLogService>(client => client.BaseAddress = new Uri(baseUrl));
// builder.Services.AddSingleton<IAuditLogService, KcetasWeb.Services.Mock.MockAuditLogService>(); // Geçici olarak hafızada tutsun
// İnternetsiz, sahte (Mock) verilerle çalışmak için (EV MODU) - İPTAL EDİLDİ
// builder.Services.AddScoped<IIsEmriService, MockIsEmriService>();
// builder.Services.AddScoped<IEndeksOkumaService, MockEndeksOkumaService>();
// builder.Services.AddScoped<IFaturaService, MockFaturaService>();
// builder.Services.AddScoped<IOutboxService, MockOutboxService>();
// builder.Services.AddScoped<IKullaniciDeposu, MockKullaniciDeposu>();
// builder.Services.AddScoped<ITuketimNoktasiService, MockTuketimNoktasiService>();
// builder.Services.AddScoped<ISozlesmeService, MockSozlesmeService>();
// builder.Services.AddScoped<ISayacService, MockSayacService>();
// builder.Services.AddScoped<IAboneService, MockAboneService>();
// builder.Services.AddScoped<IAuditLogService, MockAuditLogService>();

// 3. ARKA PLAN SERVİSLERİ (BACKGROUND SERVICES)
builder.Services.AddHostedService<KcetasWeb.Services.Background.IsEmriOlusturucuBackgroundService>();

var app = builder.Build();

// 2.5 HATA YÖNETİMİ (EXCEPTION HANDLING)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // HSTS (HTTP Strict Transport Security)
    app.UseHsts();
}
else 
{
    // Geliştirme ortamında bile şık hata sayfasını görmek için zorunlu yönlendirme eklenebilir, 
    // ama şimdilik production'da /Error sayfasına gitsin, Development'ta DeveloperExceptionPage çıksın.
    // Eğer isterseniz her ortamda /Error için yorum satırından çıkarın:
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();

// 3. KİMLİK SORGULAMA VE YETKİLENDİRME (Bu ikisinin sırası çok önemlidir!)
app.UseAuthentication(); // Kimlik sor
app.UseAuthorization();  // Yetkisi var mı bak

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.Run();