using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace SearchNews.Models
{
    /**
     * Pencarian dengan regex
     */
    public class BMSearch
    {
        /*Array dari rss yang digunakan*/
        private static string[] rssArray = {"http://rss.detik.com/index.php/detikcom", "http://tempo.co/rss/terkini",
            "http://rss.vivanews.com/get/all", "http://www.antaranews.com/rss/terkini"};

        /*Judul website berita*/
        public static string siteTitle = "siteTitle";
        /*Link dari berita*/
        public static string link = "link";
        /*Judul artikel berita*/
        public static string title = "title";
        /*Isi lengkap berita*/
        public static string paragraph = "paragraph";
        /*Kalimat yang cocok dengan keyword*/
        public static string match = "match";

        /*Keyword untuk pencarian*/
        public string keyword;
        /*List dari hasil. Setiap elemen memetakan nama atribut("link"|"isi"|"kalimat") ke nilai atribut yang bersangkutan*/
        public List<Dictionary<string, string>> searchResult;

        /*Konstruktor*/
        public BMSearch(string keyword)
        {
            this.keyword = keyword;
            searchResult = new List<Dictionary<string, string>>();
        }

        /*Prosedur pembacaan dan pencarian dalam rss*/
        public async Task Search()
        {
            System.Diagnostics.Debug.WriteLine("entering BMSearch.search() with keyword = "+keyword);
            foreach (string rss in rssArray)
            {
                var client = new HttpClient();
                var stream = await client.GetStreamAsync(rss);
                var xmlDocument = XDocument.Load(stream);
                XmlReader xmlReader = xmlDocument.CreateReader();
                bool isGetTitle = false;
                string siteTitleLocal = "";
                while (xmlReader.Read())
                {
                    Dictionary<string, string> readValue = new Dictionary<string, string>();
                    if ((xmlReader.NodeType == XmlNodeType.Element) && (xmlReader.Name == "title") && !isGetTitle)
                    {
                        xmlReader.Read();
                        isGetTitle = true;
                        siteTitleLocal = xmlReader.Value;
                    }
                    else if ((xmlReader.NodeType == XmlNodeType.Element) && (xmlReader.Name == "item"))
                    {
                        readValue[siteTitle] = siteTitleLocal;
                        readValue[link] = "";
                        readValue[title] = "";
                        readValue[paragraph] = "";
                        readValue[match] = "";
                        while (xmlReader.NodeType != XmlNodeType.EndElement || xmlReader.Name != "item")
                        {
                            xmlReader.Read();
                            if ((xmlReader.NodeType == XmlNodeType.Element) && (xmlReader.Name == "url" ||
                                xmlReader.Name == "link" || xmlReader.Name == "guid"))
                            {
                                xmlReader.Read();
                                if (xmlReader.NodeType == XmlNodeType.Text || xmlReader.NodeType == XmlNodeType.CDATA)
                                {
                                    readValue[link] = xmlReader.Value;
                                }
                            }
                            else if ((xmlReader.NodeType == XmlNodeType.Element) && (xmlReader.Name == "title"))
                            {
                                xmlReader.Read();
                                if (xmlReader.NodeType == XmlNodeType.Text || xmlReader.NodeType == XmlNodeType.CDATA)
                                {
                                    readValue[title] = xmlReader.Value;
                                }
                            }
                        }
                        //debug hasil parsing xml
                        System.Diagnostics.Debug.WriteLine("<ParseResult> "+readValue[title] + " - " + readValue[link]);
                        //panggil prosedur pencarian
                        try
                        {
                            await MatchInWebsite(readValue);
                            System.Diagnostics.Debug.WriteLine("<LoadResult> Loaded Successfully");
                        } catch (Exception exc)
                        {
                            System.Diagnostics.Debug.WriteLine("<LoadResult> Not Loaded");
                        }     
                    }
                }
            }
        }

        /*Prosedur pencarian dalam website*/
        public async Task MatchInWebsite(Dictionary<string,string> xmlParseResult)
        {
            KeyValuePair<int, int> matchedStringIndex = MatchString(keyword, xmlParseResult[title]);
            //keyword cocok dengan judul
            if (matchedStringIndex.Key != -1 && matchedStringIndex.Value != -1)
            {
                xmlParseResult[match] = xmlParseResult[title];
                searchResult.Add(xmlParseResult);
            }
            //keyword tidak cocok dengan judul, cari di konten berita
            else 
            {
                HtmlWeb web = new HtmlWeb();
                HtmlDocument document = await web.LoadFromWebAsync(xmlParseResult[link]);
                string content = "";
                //parsing berita detik
                if (xmlParseResult[siteTitle]=="news.detik")
                {
                    content = document.GetElementbyId("detikdetailtext").InnerText;
                    content = Regex.Replace(content, @"polong.create\(([\s\S]*?)\)", String.Empty);
                }
                //parsing berita tempo dan viva
                else if (xmlParseResult[siteTitle] == "Tempo.co News Site" || xmlParseResult[siteTitle]== "VIVA.co.id")
                {
                    HtmlNode[] paragraphs = document.DocumentNode.Descendants().Where(n => n.Name == "p").ToArray();
                    foreach (HtmlNode paragraph in paragraphs) {
                        content = content + paragraph.InnerHtml;
                    }
                }
                //parsing berita antara
                else if (xmlParseResult[siteTitle]== "ANTARA News - Berita Terkini")
                {
                    content = document.GetElementbyId("content_news").InnerText;
                }
                content = Regex.Replace(content, @"<([\s\S]*?)>", String.Empty);
                content = Regex.Replace(content, @"(\n)+", String.Empty);
                content = Regex.Replace(content, @"\s{2,}", " ");
                KeyValuePair<int, int> keywordLocation = MatchString(keyword, content);
                if (keywordLocation.Key != -1 && keywordLocation.Value != -1)
                {
                    int begin = keywordLocation.Key;
                    int end = keywordLocation.Value;
                    while (begin >= 0)
                    {
                        if (content[begin] == '.')
                        {
                            begin++;
                            break;
                        }
                        else
                        {
                            begin--;
                        }
                    }
                    while (end < content.Length)
                    {
                        if (content[end] == '.')
                        {
                            end--;
                            break;
                        }
                        else
                        {
                            end++;
                        }
                    }
                    xmlParseResult[match] = content.Substring(begin, end - begin + 1);
                    searchResult.Add(xmlParseResult);
                }
            }
        }

        /*String matching dengan algoritma pilihan*/
        /*Mengembalikan nilai berupa indeks pertama dan terakhir tempat substring ditemukan pada longstring*/
        /*Mengembalikan (-1,-1) jika tidak cocok*/
        public KeyValuePair<int, int> MatchString(string substring, string longstring)
        {
            BM bm = new BM(substring);
            int match = bm.SearchByBM(longstring);
            if (match != -1)
            {
                return new KeyValuePair<int, int>(match, match + substring.Length - 1);
            }
            else
            {
                return new KeyValuePair<int, int>(-1, -1);
            }
        }
    }
}
