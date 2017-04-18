using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net;
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
    public class RegexSearch
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
        public List<Dictionary<string,string>> searchResult;
        
        /*Konstruktor*/
        public RegexSearch(string keyword)
        {
            this.keyword = keyword;
            searchResult = new List<Dictionary<string, string>>();
        }

        /*Prosedur pembacaan dan pencarian dalam rss*/
        public async Task Search()
        {
            System.Diagnostics.Debug.WriteLine("entering RegexSearch.search() with keyword = "+keyword);
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
            if (matchedStringIndex.Key != -1 && matchedStringIndex.Value != -1)
            {
                xmlParseResult[match] = xmlParseResult[title];
                searchResult.Add(xmlParseResult);
            } else
            {
                HtmlWeb web = new HtmlWeb();
                HtmlDocument document = await web.LoadFromWebAsync(xmlParseResult[link]);
            }
        }

        /*String matching dengan algoritma pilihan*/
        /*Mengembalikan nilai berupa indeks pertama dan terakhir tempat substring ditemukan pada longstring*/
        /*Mengembalikan (-1,-1) jika tidak cocok*/
        public KeyValuePair<int,int> MatchString(string substring, string longstring)
        {
            Regex regex = new Regex(substring, RegexOptions.IgnoreCase|RegexOptions.IgnorePatternWhitespace);
            Match match = regex.Match(longstring);
            if (match.Success)
            {
                return new KeyValuePair<int, int>(match.Index, match.Index+match.Length-1);
            }
            else
            {
                return new KeyValuePair<int, int>(-1, -1);
            }
        }
    }
}
