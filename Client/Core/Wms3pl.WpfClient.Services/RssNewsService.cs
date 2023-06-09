using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace Wms3pl.WpfClient.Services
{
  public partial class RssNewsService : INewsService
  {
    private string _rssUrl;

    public RssNewsService()
    {
      _rssUrl = ConfigurationManager.AppSettings["NewsRss"];
    }

    public IEnumerable<News> GetNews()
    {
      try
      {
        var client = new WebClient();
        client.Encoding = Encoding.UTF8;
        var data = client.DownloadString(_rssUrl);
        var doc = XDocument.Parse(data);

        var q = from i in doc.Root.Descendants("item")
                select new News { Title = (string)i.Element("title"), PubDate = ((DateTime)i.Element("pubDate")).AddHours(8), Link = (string)i.Element("link") };
        return q.ToList();
      }
      catch (Exception ex)
      {
        var news = new News() {Title = "無法連上新聞, Message=" + ex.Message, PubDate = DateTime.Now};
        return new List<News>() { news};
      }
    }
  }
}
