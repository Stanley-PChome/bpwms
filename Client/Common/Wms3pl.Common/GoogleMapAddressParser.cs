using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.GoogleMapAddress;

namespace Wms3pl.Common
{
	public class GoogleMapAddressParser
	{
		public GoogleMapAddressParser()
		{ 
		}

		public GoogleMapAddressParser(string originalAddress)
		{
			OriginalAddress = originalAddress;
		}

		public string OriginalAddress { get; set; }
		public string ParsedAddress { get; set; }
		public AddressInfo AddressInfo { get; set; }

		private string _shortZipCode;
		public string ShortZipCode { get { return _shortZipCode; } }

		private string _longZipCode;
		public string LongZipCode { get { return _longZipCode; } }

		public void Run()
		{
			var webReq = (HttpWebRequest)WebRequest.Create("http://maps.googleapis.com/maps/api/geocode/json?language=zh-TW&address=" + OriginalAddress);
			webReq.ContentType = "application/x-www-form-urlencoded";

			webReq.Method = "GET";
			var json = "";
			using (var res = webReq.GetResponse())
			{
				var sr = new StreamReader(res.GetResponseStream());
				json = sr.ReadToEnd();
				AddressInfo = JsonConvert.DeserializeObject<AddressInfo>(json);
				if (!AddressInfo.Results.Any() || AddressInfo.Results.Count > 1)
					return;
				_shortZipCode = GetShortName("postal_code");
				if (_shortZipCode == null)
					_shortZipCode = "";
				_longZipCode = GetLongName("postal_code");
				if (_longZipCode == null)
					_longZipCode = "";
				ParsedAddress = GetParsedAddress();
			};
		}

		private string GetShortName(string typeName)
		{
			return (from a in AddressInfo.Results[0].Address_components
							from b in a.Types
							where b == typeName
							select a.Short_name).FirstOrDefault();
		}

		private string GetLongName(string typeName)
		{
			return (from a in AddressInfo.Results[0].Address_components
							from b in a.Types
							where b == typeName
							select a.Long_name).FirstOrDefault();
		}
		private string GetParsedAddress()
		{
			var formattedAddress = AddressInfo.Results[0].Formatted_address;
			if (string.IsNullOrEmpty(formattedAddress))
				return string.Empty;
			var country = GetLongName("country");
			if (!string.IsNullOrEmpty(_shortZipCode))
				formattedAddress = formattedAddress.Replace(_shortZipCode, "");
			if (!string.IsNullOrEmpty(country))
				formattedAddress = formattedAddress.Replace(country, "");
			return formattedAddress;
		}
	}
}

namespace Wms3pl.GoogleMapAddress
{
	public class AddressInfo
	{
		public string Status { get; set; }
		public List<Result> Results { get; set; }
	}
	public class Result
	{
		public List<string> Types { get; set; }
		public string Formatted_address { get; set; }
		public List<AddressComponent> Address_components { get; set; }
	}

	public class AddressComponent
	{
		public string Long_name { get; set; }
		public string Short_name { get; set; }
		public List<string> Types { get; set; }
	}
}