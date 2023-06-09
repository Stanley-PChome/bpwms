using System;
using System.IO;
using System.Security.Cryptography;

namespace Wms3pl.WebServices.DataCommon
{
	public class AesCryptoHelper
	{
		private readonly byte[] _key;
		private static byte[] _iv = Convert.FromBase64String(Constants.EncryptAesIv);

		public AesCryptoHelper(byte[] key)
		{
			_key = key;
		}

		public string Encode(string input)
    {
//#if(DEBUG)
//			return input;
//#else
			if (string.IsNullOrWhiteSpace(input)) return input;
			return Convert.ToBase64String(EncryptStringToBytes_Aes(input, _key));
//#endif
    }

		public string Decode(string encrypted)
		{
//#if(DEBUG)
//			return encrypted;
//#else
      try
      {
        if (string.IsNullOrEmpty(encrypted) || (encrypted.Length % 4 != 0)) return encrypted;
        if (string.IsNullOrWhiteSpace(encrypted)) return encrypted;
        var bytes = Convert.FromBase64String(encrypted);
        return DecryptStringFromBytes_Aes(bytes, _key);
      }
      catch (FormatException)
      {
        return encrypted;
      }
			catch(CryptographicException)
			{
				return encrypted;
			}
//#endif
		}

		private static byte[] EncryptStringToBytes_Aes(string plainText, byte[] key)
		{
			// Check arguments.
			if (plainText == null || plainText.Length <= 0)
				throw new ArgumentNullException("plainText");
			if (key == null || key.Length <= 0)
				throw new ArgumentNullException("Key");

			byte[] encrypted;
			// Create an Aes object
			// with the specified key and IV.
			using (Aes aesAlg = Aes.Create())
			{
				aesAlg.Key = key;
				aesAlg.IV = _iv;

				// Create a decrytor to perform the stream transform.
				ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key
																														, aesAlg.IV);

				// Create the streams used for encryption.
				using (MemoryStream msEncrypt = new MemoryStream())
				{
					using (CryptoStream csEncrypt = new CryptoStream(msEncrypt
																													 , encryptor, CryptoStreamMode.Write))
					{
						using (StreamWriter swEncrypt = new StreamWriter(
							csEncrypt))
						{

							//Write all data to the stream.
							swEncrypt.Write(plainText);
						}
						encrypted = msEncrypt.ToArray();
					}
				}
			}


			// Return the encrypted bytes from the memory stream.
			return encrypted;

		}

		private static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] key)
		{
			// Check arguments.
			if (cipherText == null || cipherText.Length <= 0)
				throw new ArgumentNullException("cipherText");
			if (key == null || key.Length <= 0)
				throw new ArgumentNullException("Key");

			// Declare the string used to hold
			// the decrypted text.
			string plaintext = null;

			// Create an Aes object
			// with the specified key and IV.
			using (Aes aesAlg = Aes.Create())
			{
				aesAlg.Key = key;
				aesAlg.IV = _iv;

				// Create a decrytor to perform the stream transform.
				ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key
																														, aesAlg.IV);

				// Create the streams used for decryption.
				using (MemoryStream msDecrypt = new MemoryStream(cipherText))
				{
					using (CryptoStream csDecrypt = new CryptoStream(msDecrypt
																													 , decryptor, CryptoStreamMode.Read))
					{
						using (StreamReader srDecrypt = new StreamReader(
							csDecrypt))
						{

							// Read the decrypted bytes from the decrypting stream
							// and place them in a string.
							plaintext = srDecrypt.ReadToEnd();
						}
					}
				}

			}

			return plaintext;

		}
	}
}