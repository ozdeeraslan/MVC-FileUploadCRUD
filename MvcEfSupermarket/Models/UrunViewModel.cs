using MvcEfSupermarket.Data;

namespace MvcEfSupermarket.Models
{
    // veritabaninda olmayacak. SADECE, kullanicidaan istenecek olan prop'lari ekleyelim (kullanicinin alacagi).
    public class UrunViewModel
    {
        public string Ad { get; set; } = null!;

        public int Fiyat { get; set; }

        public bool StokDurumu { get; set; }

        // Kullanıcıdan resim yüklemesi istenecegi icin buraya IFormFile türünde ekledik.
        public IFormFile Resim { get; set; } 
    }
}
