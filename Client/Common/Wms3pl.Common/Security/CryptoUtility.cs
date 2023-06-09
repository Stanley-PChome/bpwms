using System;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Security.Cryptography;

namespace Wms3pl.Common.Security
{
  public class CryptoUtility
  {
    private static byte[] GetHash(string plainText)
    {
      var defaultCrypto = EnterpriseLibraryContainer.Current.GetInstance<CryptographyManager>();
      byte[] valueToHash = System.Text.Encoding.UTF8.GetBytes(plainText);
      byte[] generatedHash = defaultCrypto.CreateHash("SHA1CryptoServiceProvider", valueToHash);
      // Clear the byte array memory
      Array.Clear(valueToHash, 0, valueToHash.Length);
      return generatedHash;
    }

    public static string GetHashString(string plainText)
    {
      var bytes = GetHash(plainText);
      return Convert.ToBase64String(bytes);
    }

    public static bool CompareHash(string plainText, string hashString)
    {
      byte[] plainBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
      byte[] hashBytes = Convert.FromBase64String(hashString);
      CryptographyManager defaultCrypto = EnterpriseLibraryContainer.Current.GetInstance<CryptographyManager>();
      return defaultCrypto.CompareHash("SHA1CryptoServiceProvider", plainBytes, hashBytes);
    }
  }
}
