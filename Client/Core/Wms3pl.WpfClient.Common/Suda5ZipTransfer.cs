using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Wms3pl.WpfClient.Common
{
  public static class Suda5ZipTransfer
  {
    public static string TranZip(string address)
    {
      var request = WebRequest.Create("http://10.35.10.200:8800/egs");
      request.Method = "POST";
      request.ContentType = "application/x-www-form-urlencoded;";
      using (var writer = new StreamWriter(request.GetRequestStream()))
      {
        writer.Write("cmd=query_suda5&address_1=" + HttpUtility.UrlEncode(address));
      }

      var response = request.GetResponse();

      var reader = new StreamReader(response.GetResponseStream());
      var data = HttpUtility.ParseQueryString(reader.ReadToEnd());
      if (data["status"] == "OK")
        return data["suda5_1"];
      return null;
    }

    public static string TranBase(string sdZip)
    {
      var request = WebRequest.Create("http://10.35.10.200:8800/egs");
      request.Method = "POST";
      request.ContentType = "application/x-www-form-urlencoded;";
      using (var writer = new StreamWriter(request.GetRequestStream()))
      {
        writer.Write("cmd=query_base&suda5_1=" + HttpUtility.UrlEncode(sdZip));
      }

      var response = request.GetResponse();

      var reader = new StreamReader(response.GetResponseStream());
      var data = HttpUtility.ParseQueryString(reader.ReadToEnd());
      if (data["status"] == "OK")
        return data["base_1"];
      return null;
    }
  }
}
