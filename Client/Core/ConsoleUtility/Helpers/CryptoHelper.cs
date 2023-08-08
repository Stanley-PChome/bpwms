using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleUtility.Helpers
{
	public static class CryptoHelper
	{
		public static byte[] bytIV = { };
		public static SymmetricAlgorithm _CryptoService;

		#region 加密
		public static string Encrypt(string source, string key)
		{
			var bytIn = Encoding.UTF8.GetBytes(source);
			byte[] bytOut = null;

			using (var ms = new MemoryStream())
			{
				//set the keys
				_CryptoService = new DESCryptoServiceProvider();
				_CryptoService.Key = GetLegalKey(key);
				_CryptoService.IV = bytIV;
				_CryptoService.Mode = CipherMode.ECB;

				ICryptoTransform encrypto = _CryptoService.CreateEncryptor();
				var cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write);

				cs.Write(bytIn, 0, bytIn.Length);
				cs.FlushFinalBlock();
				cs.Close();
				bytOut = ms.ToArray();
				ms.Close();
			}
			return Convert.ToBase64String(bytOut);

		}
		#endregion

		#region 解密
		public static string Decrypt(string source, string key)
		{
			using (var ms = new MemoryStream(Convert.FromBase64String(source)))
			{
				byte[] bytOut = null;

				_CryptoService = new DESCryptoServiceProvider
				{
					Key = GetLegalKey(key),
					IV = bytIV,
					Mode = CipherMode.ECB
				};
				ICryptoTransform encrypto = _CryptoService.CreateDecryptor();

				var cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Read);
				var reader = new StreamReader(cs);
				var result = reader.ReadToEnd();

				cs.Close();
				bytOut = ms.ToArray();
				ms.Close();

				return result;
			}
		}
		#endregion

		#region Key 編碼
		public static byte[] GetLegalKey(string key)
		{
			string sTemp = null;

			if ((_CryptoService.LegalKeySizes.Length > 0))
			{
				int maxSize = _CryptoService.LegalKeySizes[0].MaxSize;

				if (key.Length * 8 > maxSize)
				{
					sTemp = key.Substring(0, (maxSize / 8));
				}
				else
				{
					int moreSize = _CryptoService.LegalKeySizes[0].MinSize;
					while ((key.Length * 8 > moreSize))
					{
						moreSize += _CryptoService.LegalKeySizes[0].SkipSize;
					}

					sTemp = key.PadRight(moreSize / 8, 'X');
				}
			}
			else
			{
				sTemp = key;
			}
			if ((_CryptoService.LegalBlockSizes.Length > 0))
			{
				int maxSize = _CryptoService.LegalBlockSizes[0].MaxSize;

				Array.Resize(ref bytIV, sTemp.Length);

				if (sTemp.Length * 8 > maxSize)
				{
					Array.Resize(ref bytIV, maxSize / 8);
				}
			}

			return ASCIIEncoding.ASCII.GetBytes(sTemp);
		}
		#endregion
	}
}
