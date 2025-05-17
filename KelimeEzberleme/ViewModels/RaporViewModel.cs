namespace KelimeEzberleme.ViewModels
{
  
        public class RaporViewModel
        {
            public int ToplamDogruCevapSayisi { get; set; }
        public int ToplamYanlisCevapSayisi { get; set; }
        public int OgrenilenKelimeSayisi { get; set; }
        public double BasariOrani { get; set; }
        public List<KategoriBazliIstatistik> KategoriIstatistikleri { get; set; } = new();
    }

    public class KategoriBazliIstatistik
{
    public string KategoriAdi { get; set; } = string.Empty;
    public int DogruSayisi { get; set; }
    public int YanlisSayisi { get; set; }
    public double BasariOrani { get; set; }
}
    }


