using Microsoft.AspNetCore.Mvc;
using MvcEfSupermarket.Data;
using MvcEfSupermarket.Models;
using System.Diagnostics;

namespace MvcEfSupermarket.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UygulamaDbContext _db;

        public HomeController(ILogger<HomeController> logger, UygulamaDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            // Görev14 ve Görev15 (parametre vermeye gerek kalmadi)
            var a = Request.Query["x"];
            var b = Request.Query["y"]; 

            ////// ürün olmasi
            ViewBag.UrunVarMi = _db.Urunler.Count() > 0;
            // bütün ürünlerin stok durumunun true olmasi
            ViewBag.ButunUrunlerStoktaMi = _db.Urunler.All(u => u.StokDurumu);
            // bütün ürünlerin stok durumunun false olmasi
            ViewBag.HicbirUrunStoktaYokMu = _db.Urunler.All(u => !u.StokDurumu);
            // en az 1 tane stokta olan ürün bulunmasi
            ViewBag.EnAzBirUrunStoktaMi = _db.Urunler.Any(u => u.StokDurumu);
            // en az 1 tane stokta olmayan ürün bulunmasi
            ViewBag.EnAzBirUrunStoktaYokMu = _db.Urunler.Any(u => !u.StokDurumu);


            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
