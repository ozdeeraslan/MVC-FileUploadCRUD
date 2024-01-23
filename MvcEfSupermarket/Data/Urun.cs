namespace MvcEfSupermarket.Data
{
    // hakiki sinif
    public class Urun
    {
        public int Id { get; set; }

        public string Ad { get; set; } = null!;

        public int Fiyat { get; set; }

        public bool StokDurumu { get; set; }

        public string? ResimAdi { get; set; } // Sadece dosya adını saklar

    }
}
