using Microsoft.AspNetCore.Mvc;
using SearchNews.Models;
using System.Threading.Tasks;

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
        public async Task<IActionResult> Result()
        {
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
                KMPSearch kmpSearch = new KMPSearch(ViewData["Keyword"].ToString());
                await kmpSearch.Search();
                ViewData["Result"] = kmpSearch.searchResult;
            }
            else if (ViewData["Algorithm"].ToString().Equals("Regex"))
            {
                //panggil Regex
                RegexSearch regexSearch = new RegexSearch(ViewData["Keyword"].ToString());
                await regexSearch.Search();
                ViewData["Result"] = regexSearch.searchResult;
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
