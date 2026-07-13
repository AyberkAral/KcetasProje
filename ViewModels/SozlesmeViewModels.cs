namespace KcetasWeb.ViewModels;

public class SozlesmeViewModels
{
    public string tekil_kod { get; set; }
    public string? TarifeGrubu { get; set; }
    public long sozlesme_id { get; set; }
    public string sozlesme_no { get; set; }
    public long tuketim_noktasi_id { get; set; }
    public long abone_id { get; set; }
    public string ad { get; set; }
    public string soyad { get; set; }
    public string unvan { get; set; }
    public string tckn { get; set; }
    public string vkn { get; set; }
    public string telefon { get; set; }
    public string e_posta { get; set; }
    public string iletisim_tercihi { get; set; }
    public string sozlesme_tipi { get; set; }
    public DateTime baslangic_tarihi { get; set; }
    public DateTime? bitis_tarihi { get; set; }
    public string statu { get; set; }
    public string tarife_grubu { get; set; }
    public int tarife_id { get; set; }
    public decimal guvence_bedeli { get; set; }
    public string status{ get; set; }
     
    public int? created_by { get; set; }
    public int? updated_by { get; set; }


    public DateTime created_at { get; set; }
    public DateTime? updated_at { get; set; }
}