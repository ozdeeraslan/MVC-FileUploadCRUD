using Microsoft.AspNetCore.Mvc;
using MvcEfSupermarket.Data;
using MvcEfSupermarket.Models;

namespace MvcEfSupermarket.Controllers
{
    public class MarketController : Controller
    {
        private readonly ILogger<MarketController> _logger;

        private readonly UygulamaDbContext _db;

        public MarketController(ILogger<MarketController> logger, UygulamaDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Listele()
        {
            return View(_db.Urunler.ToList());
        }

        public IActionResult StoktaOlanlar()
        {
            var stoktaOlanlar = _db.Urunler.Where(x => x.StokDurumu == true).ToList();
            return View(stoktaOlanlar);
        }

        public IActionResult StoktaOlmayanlar()
        {
            var stoktaOlmayanlar = _db.Urunler.Where(x => x.StokDurumu == false).ToList();
            return View(stoktaOlmayanlar);
        }

        public IActionResult Ara(string ad)
        {
            var urunler = _db.Urunler.Where(u => u.Ad == ad).ToList();
            return View(urunler);
        }

        public IActionResult Ara2(string ad)
        {
            var urunler = _db.Urunler.Where(u => u.Ad.Contains(ad)).ToList();
            return View(urunler);
        }

        // Sadece ekleme ekranini getirmek icin (formu getiriyor);
        public IActionResult Ekle()
        {
            return View();
        }

        // Ekle butonuna basildigi zaman yapilacaklar;
        [HttpPost]
        public IActionResult Ekle(UrunViewModel urunViewModel)
        {
            // form'dan gelen urun view model nesnesini yakalayip, özelliklerini HAKIKI urun'e atayip bu HAKIKI urunu, db'ye ekleyecegiz.
            try
            {
                //Fiyatin pozitif olmasi durumu
                if (urunViewModel.Fiyat <= 0)
                {
                    throw new Exception("Fiyat negatif olamaz");
                }

                // Ayni ürün isminden baska veremesin.
                if (_db.Urunler.FirstOrDefault(u => u.Ad == urunViewModel.Ad) != null)
                    throw new Exception("Eklemek istediniz ürün mevcuttur.");

                Urun yeniUrun = new Urun(); // Ileride burayi da degistirecegiz.

                // kisi resim yüklemek zorunda DEGIL!
                if (urunViewModel.Resim != null) // kisi resim sectiyse
                {
                    // sectigi dosyanin adini alalim!
                    var dosyaAdi = urunViewModel.Resim.FileName;

                    // Dosyanin kaydedilecegi konumu belirleyelim.
                    var konum = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Resimler", dosyaAdi);

                    // Dosya icin bir akis ortami olusturalim.
                    // Kaydetmek icin hazirliyoruz.
                    var akisOrtami = new FileStream(konum, FileMode.Create);

                    // Resmi o klasöre kaydet
                    urunViewModel.Resim.CopyTo(akisOrtami);

                    // Daha sonra ortami kapatalim.
                    akisOrtami.Close();

                    // Resmin ADINI HAKIKI ürünün string olan adina atiyoruz. Resmi de yukarida klasöre kaydediyoruz.
                    yeniUrun.ResimAdi = dosyaAdi;
                }

                // automapper ile ayni olan property'lerotomatik olarak aktarilabilir. Su an manuel esledik/aktardik. Farkli olanlar icin ayar yapilmasi gereklidir.
                yeniUrun.Ad = urunViewModel.Ad;
                yeniUrun.Fiyat = urunViewModel.Fiyat;
                yeniUrun.StokDurumu = urunViewModel.StokDurumu;

                // Db'ye ekle
                _db.Urunler.Add(yeniUrun);
                _db.SaveChanges();
                return RedirectToAction("Listele");
            }
            catch (Exception ex)
            {

                TempData["Durum"] = "Hata olustu! " + ex.Message;
                return View();
            }

        }

        // post: ekleme-silme-güncelleme kisimlarinda
        // get: formu acar, veritabanindan veri getirirken


        //burasi get method'dudur.
        public IActionResult Guncelle(int id) //route id'yi burada yakaliyoruz.
        {
            // gelen id'ye ait ürünü db'den getir, view'ina model olarak gönderelim ki bilgilerini görelim!
            Urun guncellenecekUrun = _db.Urunler.Find(id);

            UrunViewModel guncellenecekModel = new UrunViewModel();

            guncellenecekModel.Ad = guncellenecekUrun.Ad;
            guncellenecekModel.Fiyat = guncellenecekUrun.Fiyat;
            guncellenecekModel.StokDurumu = guncellenecekUrun.StokDurumu;

            TempData["Id"] = guncellenecekUrun.Id;

            // Secilen ürünün özellikleriyle donanmis olan model gönderiliyor. Cünkü kisi view model ile muhattap. O yüzden biz de hakiki ürünün cekip, viewmodelê dönüstürüp onun bilgilerinin formda gösterilmesini sagliyoruz.
            return View(guncellenecekModel);
        }

        [HttpPost]
        public IActionResult Guncelle(UrunViewModel urunViewModel)
        {
            //Butona basılınca formdan gelecek olan modelin prop.larını yine eklemede yaptığımız gibi HAKİKİ güncellenecek ürünün prop.larına atayıp güncellememizi yapacağız. Fakat güncellenecek olan ürün yukarıdaki metodda kaldı.
            var guncellenecekUrun = _db.Urunler.Find(TempData["Id"]);

            try
            {
                //Fiyatın pozitif olması durumu
                if (urunViewModel.Fiyat <= 0)
                    throw new Exception("Fiyat pozitif olmalıdır!");

                //Aynı ürün isminden başka veremesin.
                if (_db.Urunler.FirstOrDefault(u => u.Ad == urunViewModel.Ad && u.Id != guncellenecekUrun.Id) != null)
                    throw new Exception("Güncellemek istediğiniz ürün ismi mevcuttur!");


                //Kişi resim yüklemek zorunda DEĞİL!
                if (urunViewModel.Resim != null && urunViewModel.Resim.FileName != guncellenecekUrun.ResimAdi)
                //kişi resim seçtiyse ve eski resimden farklı resim seçtiyse
                {
                    //o resmi kullanan başka ürün yoksa eski resmi sil

                    ResimSil(guncellenecekUrun);

                    //seçtiği dosyanın adını alalım
                    var dosyaAdi = urunViewModel.Resim.FileName;

                    //Dosyanın kaydedileceği konumu belirleyelim.
                    var konum = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Resimler", dosyaAdi);

                    //Dosya için bir akış ortamı oluşturalım.
                    //Kaydetmek için hazırlıyoruz.
                    var akisOrtami = new FileStream(konum, FileMode.Create);

                    //Resmi o klasöre kaydet
                    urunViewModel.Resim.CopyTo(akisOrtami);

                    //Daha sonra ortamı kapatalım.
                    akisOrtami.Close();

                    //Resmin ADINI HAKİKİ ürünün string olan adına atıyoruz. Resmi de yukarıda klasöre kaydediyoruz.
                    guncellenecekUrun.ResimAdi = dosyaAdi;
                }
                //Automapper ile aynı olan prop.lar otomatik olarak aktarabilinir. Farklı olanlar için ayar yapılması gereklidir.

                guncellenecekUrun.Ad = urunViewModel.Ad;
                guncellenecekUrun.Fiyat = urunViewModel.Fiyat;
                guncellenecekUrun.StokDurumu = urunViewModel.StokDurumu;

                //Db'ye ekle
                _db.Urunler.Update(guncellenecekUrun);
                _db.SaveChanges();
                return RedirectToAction("Listele");
            }
            catch (Exception ex)
            {
                TempData["Durum"] = "Hata oluştu! " + ex.Message;
                return View();
            }
        }


        //burasi get method'dudur.
        public IActionResult Sil(int id) //route id'yi burada yakaliyoruz.
        {
            // gelen id'ye ait ürünü db'den getir, view'ina model olarak gönderelim ki bilgilerini görelim!
            // Urun urun = _db.Urunler.FirstOrDefault(u => u.Id == id);
            Urun urun = _db.Urunler.Find(id);
            return View(urun);
        }

        [HttpPost]
        public IActionResult Sil(Urun urun)
        {
            // ürünü silerken resmini kullanan baska ürün yoksa resmi de sil.
            ResimSil(urun);

            _db.Urunler.Remove(urun);
            _db.SaveChanges();
            return RedirectToAction("Listele");
        }

        public IActionResult ResimNullYap(int id) //resmi kaldirip null yap
        {
            // id'ye ait olan ürünü getir ve resmini null yap.
            Urun resmiKaldirilacakUrun = _db.Urunler.Find(id);
            if(resmiKaldirilacakUrun.ResimAdi != null)
            {
                // Bu resmi kullanan baska ürün yoksa o resmi klasörden de sil.
                ResimSil(resmiKaldirilacakUrun);

                // kendi resim yolunu da null yap.
                resmiKaldirilacakUrun.ResimAdi = null;

                _db.Urunler.Update(resmiKaldirilacakUrun);
                _db.SaveChanges();

                return RedirectToAction("Listele");
            }
            else
            {
                TempData["Durum"] = "Ürüne ait resim yoktur!";
                return RedirectToAction("Listele");
            }
        }


        public void ResimSil(Urun urun) // database'den resmi silme
        {
            //Bu resmi kullanan BAŞKA (KENDİSİ DIŞINDA) ürün var mı?
            var resmiKullananBaskaVarMi = _db.Urunler.Any(u => u.ResimAdi == urun.ResimAdi && u.Id != urun.Id);

            if (urun.ResimAdi != null && !resmiKullananBaskaVarMi)
            {
                //o resmin tam adını (patikasıyla beraber) getir.
                string dosya = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Resimler", urun.ResimAdi);

                //Silme metoduna bu patikayı gönder.
                System.IO.File.Delete(dosya);
            }

        }
    }
}
