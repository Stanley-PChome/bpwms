using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wms3pl.Common.Security
{
  public static class StringExtension
  {
    public static char GetCheckSum(this string input)
    {
      byte[] bytes = System.Text.ASCIIEncoding.ASCII.GetBytes(input);

      int chksum = 0;

      for (int j = 0; j < bytes.Length; j++)
      {
        chksum = chksum ^ bytes[j];
      }

      Console.WriteLine(chksum);
      int xxx = chksum % 10 + '0';

      return (char)(xxx);
    }

    public static bool CheckSum(this string inputToValidate)
    {
      string toValidate = inputToValidate.Substring(0, inputToValidate.Length - 1);
      char checkSumChar = inputToValidate[inputToValidate.Length - 1];
      return checkSumChar == toValidate.GetCheckSum();
    }
  }
}
