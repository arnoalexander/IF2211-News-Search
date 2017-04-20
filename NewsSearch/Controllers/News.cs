using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SearchNews.Controllers
{
    public class News
    {
        public string Link { get; set;}
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime PublishDate { get; set; }

        public News() {
            Link = "";
            Title = "";
            Content = "";
            PublishDate = DateTime.Today;
        }
    }
}
