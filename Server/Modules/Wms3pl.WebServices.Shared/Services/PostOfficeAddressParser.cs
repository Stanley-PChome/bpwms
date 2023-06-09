using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.WebServices.Shared
{
	public class PostOfficeAddressParser
	{
		public PostOfficeAddressParser()
		{ 
		}

		public PostOfficeAddressParser(string originalAddress)
		{
			OriginalAddress = originalAddress;
		}

		public string OriginalAddress { get; set; }
		public string ParsedAddress { get; set; }

		private string _shortZipCode;
		public string ShortZipCode { get { return _shortZipCode; } }

		private string _longZipCode;
		public string LongZipCode { get { return _longZipCode; } }

		public void Run()
		{
			var wsZipCode = new WsZipCode.LZWZIPSoapClient();
			var address = string.Empty;
			_longZipCode = wsZipCode.GetZipCode32(OriginalAddress, out address);
			if (_longZipCode.Length >= 3)
				_shortZipCode = _longZipCode.Substring(0, 3);
			if (!string.IsNullOrEmpty(address))
				ParsedAddress = address;
		}
	}
}