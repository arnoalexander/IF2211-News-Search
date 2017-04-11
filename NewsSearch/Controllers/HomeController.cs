using Microsoft.AspNetCore.Mvc;

namespace SearchNews.Controllers
{
    /**
     * Kelas untuk kontrol webpage.
     */
    public class HomeController : Controller
    {
        /**
         * Action untuk homepage.
         */ 
        public IActionResult Index()
        {
            ViewData["Message"] = "Homepage";
            return View();
        }

        /**
         * Action untuk halaman About.
         */
        public IActionResult About()
        {
            ViewData["Message"] = "Sekilas Tentang Aplikasi";
            return View();
        }

        /**
         * Action untuk halaman Author.
         */
        public IActionResult Author()
        {
            ViewData["Message"] = "Pembuat Aplikasi";
            return View();
        }

        /**
         * Action untuk hasil pencarian.
         */
        public IActionResult Result()
        {
            //Kurang panggil algoritma
            ViewData["Message"] = "Hasil Pencarian";
            ViewData["Keyword"] = Request.Form["keyword"];
            ViewData["Algorithm"] = Request.Form["algorithm"];
            if (ViewData["Algorithm"].ToString().Equals("Boyer-Moore"))
            {
                //panggil Boyer-Moore
            }
            else if (ViewData["Algorithm"].ToString().Equals("KMP"))
            {
                //panggil KMP
            }
            else if (ViewData["Algorithm"].ToString().Equals("Regex"))
            {
                //panggil Regex
            }
            return View();
        }

        /**
         * Action jika terjadi error.
         */
        public IActionResult Error()
        {
            return View();
        }
    }
}
