using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SearchNews.Controllers
{
    public class RSSfeed
    {
        public IList<News> ParseRSS(string Link)
        {
            try
            {
                XDocument document = new XDocument();
                var entries = from news in document.Root.Descendants().First(i => i.Name.LocalName == "channel").Elements().Where(i => i.Name.LocalName == "Item")
                              select new News
                              {
                                  Content = news.Elements().First(i => i.Name.LocalName == "description").Value,
                                  Link = news.Elements().First(i => i.Name.LocalName == "link").Value,
                                  PublishDate = ParseDate(news.Elements().First(i => i.Name.LocalName == "pubDate").Value),
                                  Title = news.Elements().First(i => i.Name.LocalName == "title").Value
                              };
                return entries.ToList();
            }
            catch
            {
                return new List<News>();
            }
        }
        private DateTime ParseDate(string date)
        {
            DateTime result;
            if (DateTime.TryParse(date, out result))
                return result;
            else
                return DateTime.MinValue;
        }
    }
}
