using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.WebServices.DataCommon
{
	public class AesCryptor
	{
		private static AesCryptoHelper _helper;

		public static AesCryptoHelper Current {
      get {
        if (_helper == null)
          _helper = new AesCryptoHelper(AesKey);
        return _helper; 
      } 
    }

		private static byte[] GetAesKey()
		{
			return Convert.FromBase64String(Constants.EncryptAesKey);
		}

    private static byte[] _aesKey;
    public static byte[] AesKey
    {
      get {
        _aesKey = GetAesKey();
        return _aesKey; 
      }
    }

    public static void ClearAesKey()
    {
      _aesKey = null;
    }
	}
}
