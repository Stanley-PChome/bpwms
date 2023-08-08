using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.Common
{
	public class HttpRequestHelper
	{

		public static async Task<HttpResponseMessage> CallWebApiByGet(string url, Dictionary<string, string> headers = null)
		{
			HttpClient client = new HttpClient();
			if (headers != null)
			{
				foreach (var header in headers)
				{
					client.DefaultRequestHeaders.Add(header.Key, header.Value);
				}
			}

			var response = await client.GetAsync(url);
			if (!response.IsSuccessStatusCode)
				throw new Exception($"Call Web Api Error!\r\n{response.StatusCode}\r\n{response.ToString()}");
			return response;
		}

		public static async Task<HttpResponseMessage> CallWebApiByPost<T>(string url, T param, Dictionary<string, string> headers = null)
		{
			HttpClient client = new HttpClient();

			if (headers != null)
			{
				foreach (var header in headers)
				{
					client.DefaultRequestHeaders.Add(header.Key, header.Value);
				}
			}

			string serialisedData = string.Empty;
			if (!(param is string))
				serialisedData = JsonConvert.SerializeObject(param);
			else
				serialisedData = param as string;

			var response = await client.PostAsync(url, new StringContent(serialisedData, Encoding.UTF8, "application/json"));
			if (!response.IsSuccessStatusCode)
				throw new Exception($"Call Web Api Error!\r\n{response.StatusCode}\r\n{response.ToString()}");
			return response;
		}

	}
}
