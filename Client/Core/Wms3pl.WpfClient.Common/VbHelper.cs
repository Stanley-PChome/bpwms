using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wms3pl.WpfClient.Common
{
  public static class VbHelper
  {
    public static int Asc(char c)
    {
      int converted = c;
      if (converted >= 0x80)
      {
        var buffer = new byte[2];
        // if the resulting conversion is 1 byte in length, just use the value
        if (Encoding.Default.GetBytes(new char[] { c }, 0, 1, buffer, 0) == 1)
        {
          converted = buffer[0];
        }
        else
        {
          // byte swap bytes 1 and 2;
          converted = buffer[0] << 16 | buffer[1];
        }
      }
      return converted;
    }
  }
}
