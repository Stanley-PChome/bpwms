using System;
using System.Collections.Generic;
using System.Text;

namespace Wms3pl.WpfClient.Common
{
  /// <summary>
  /// Code 128
  /// Convert an input string to the equivilant string including start and stop characters.
  /// This object compresses the values to the shortest possible code 128 barcode format 
  /// </summary>
  public static class BarcodeConverter128
  {
    /// <summary>
    /// Converts an input string to the equivilant string, that need to be produced using the 'Code 128' font.
    /// </summary>
    /// <param name="value">String to be encoded</param>
    /// <returns>Encoded string start/stop and checksum characters included</returns>
    public static string StringToBarcode(string value)
    {
      // Parameters : a string
      // Return     : a string which give the bar code when it is dispayed with CODE128.TTF font
      // 			 : an empty string if the supplied parameter is no good
      int charPos, minCharPos;
      int currentChar, checksum;
      bool isTableB = true, isValid = true;
      string returnValue = string.Empty;

      if (value!=null && value.Length > 0)
      {

        // Check for valid characters
        for (int charCount = 0; charCount < value.Length; charCount++)
        {
          //currentChar = char.GetNumericValue(value, charPos);
          currentChar = (int)char.Parse(value.Substring(charCount, 1));
          if (!(currentChar >= 32 && currentChar <= 126))
          {
            isValid = false;
            break;
          }
        }

        // Barcode is full of ascii characters, we can now process it
        if (isValid)
        {
          charPos = 0;
          while (charPos < value.Length)
          {
            if (isTableB)
            {
              // See if interesting to switch to table C
              // yes for 4 digits at start or end, else if 6 digits
              if (charPos == 0 || charPos + 4 == value.Length)
                minCharPos = 4;
              else
                minCharPos = 6;


              minCharPos = BarcodeConverter128.IsNumber(value, charPos, minCharPos);

              if (minCharPos < 0)
              {
                // Choice table C
                if (charPos == 0)
                {
                  // Starting with table C
                  returnValue = ((char)205).ToString(); // char.ConvertFromUtf32(205);
                }
                else
                {
                  // Switch to table C
                  returnValue = returnValue + ((char)199).ToString();
                }
                isTableB = false;
              }
              else
              {
                if (charPos == 0)
                {
                  // Starting with table B
                  returnValue = ((char)204).ToString(); // char.ConvertFromUtf32(204);
                }

              }
            }

            if (!isTableB)
            {
              // We are on table C, try to process 2 digits
              minCharPos = 2;
              minCharPos = BarcodeConverter128.IsNumber(value, charPos, minCharPos);
              if (minCharPos < 0) // OK for 2 digits, process it
              {
                currentChar = int.Parse(value.Substring(charPos, 2));
                currentChar = currentChar < 95 ? currentChar + 32 : currentChar + 100;
                returnValue = returnValue + ((char)currentChar).ToString();
                charPos += 2;
              }
              else
              {
                // We haven't 2 digits, switch to table B
                returnValue = returnValue + ((char)200).ToString();
                isTableB = true;
              }
            }
            if (isTableB)
            {
              // Process 1 digit with table B
              returnValue = returnValue + value.Substring(charPos, 1);
              charPos++;
            }
          }

          // Calculation of the checksum
          checksum = 0;
          for (int loop = 0; loop < returnValue.Length; loop++)
          {
            currentChar = (int)char.Parse(returnValue.Substring(loop, 1));
            currentChar = currentChar < 127 ? currentChar - 32 : currentChar - 100;
            if (loop == 0)
              checksum = currentChar;
            else
              checksum = (checksum + (loop * currentChar)) % 103;
          }

          // Calculation of the checksum ASCII code
          checksum = checksum < 95 ? checksum + 32 : checksum + 100;
          // Add the checksum and the STOP
          returnValue = returnValue +
              ((char)checksum).ToString() +
              ((char)206).ToString();
        }
      }

      return returnValue;
    }


    private static int IsNumber(string InputValue, int CharPos, int MinCharPos)
    {
      // if the MinCharPos characters from CharPos are numeric, then MinCharPos = -1
      MinCharPos--;
      if (CharPos + MinCharPos < InputValue.Length)
      {
        while (MinCharPos >= 0)
        {
          if ((int)char.Parse(InputValue.Substring(CharPos + MinCharPos, 1)) < 48
              || (int)char.Parse(InputValue.Substring(CharPos + MinCharPos, 1)) > 57)
          {
            break;
          }
          MinCharPos--;
        }
      }
      return MinCharPos;
    }
  }

  /// <summary>
  /// Code 39
  /// Convert an input string to the equivilant string including start and stop characters.
  /// </summary>
  public static class BarcodeConverter39
  {
    /// <summary>
    /// Converts an input string to the equivilant string, that need to be produced using the 'Code 3 de 9' font.
    /// </summary>
    /// <param name="value">String to be encoded</param>
    /// <returns>Encoded string start/stop characters included</returns>
    public static string StringToBarcode(string value)
    {
      return StringToBarcode(value, false);
    }

    /// <summary>
    /// Converts an input string to the equivilant string, that need to be produced using the 'Code 3 de 9' font.
    /// </summary>
    /// <param name="value">String to be encoded</param>
    /// <param name="addChecksum">Is checksum to be added</param>
    /// <returns>Encoded string start/stop and checksum characters included</returns>
    public static string StringToBarcode(string value, bool addChecksum)
    {
      // Parameters : a string
      // Return     : a string which give the bar code when it is dispayed with CODE128.TTF font
      // 			 : an empty string if the supplied parameter is no good
      bool isValid = true;
      char currentChar;
      string returnValue = string.Empty;
      int checksum = 0;
      if (value.Length > 0)
      {

        //Check for valid characters
        for (int CharPos = 0; CharPos < value.Length; CharPos++)
        {
          currentChar = char.Parse(value.Substring(CharPos, 1));
          if (!((currentChar >= '0' && currentChar <= '9') || (currentChar >= 'A' && currentChar <= 'Z') ||
              currentChar == ' ' || currentChar == '-' || currentChar == '.' || currentChar == '$' ||
              currentChar == '/' || currentChar == '+' || currentChar == '%'))
          {
            isValid = false;
            break;
          }
        }
        if (isValid)
        {
          // Add start char
          returnValue = "*";
          // Add other chars, and calc checksum
          for (int CharPos = 0; CharPos < value.Length; CharPos++)
          {
            currentChar = char.Parse(value.Substring(CharPos, 1));
            returnValue += currentChar.ToString();
            if (currentChar >= '0' && currentChar <= '9')
            {
              checksum = checksum + (int)currentChar - 48;
            }
            else if (currentChar >= 'A' && currentChar <= 'Z')
            {
              checksum = checksum + (int)currentChar - 55;
            }
            else
            {
              switch (currentChar)
              {
                case '-':
                  checksum = checksum + (int)currentChar - 9;
                  break;
                case '.':
                  checksum = checksum + (int)currentChar - 9;
                  break;
                case '$':
                  checksum = checksum + (int)currentChar + 3;
                  break;
                case '/':
                  checksum = checksum + (int)currentChar - 7;
                  break;
                case '+':
                  checksum = checksum + (int)currentChar - 2;
                  break;
                case '%':
                  checksum = checksum + (int)currentChar + 5;
                  break;
                case ' ':
                  checksum = checksum + (int)currentChar + 6;
                  break;
              }
            }
          }
          // Calculation of the checksum ASCII code
          if (addChecksum)
          {
            checksum = checksum % 43;
            if (checksum >= 0 && checksum <= 9)
            {
              returnValue += ((char)(checksum + 48)).ToString();
            }
            else if (checksum >= 10 && checksum <= 35)
            {
              returnValue += ((char)(checksum + 55)).ToString();
            }
            else
            {
              switch (checksum)
              {
                case 36:
                  returnValue += "-";
                  break;
                case 37:
                  returnValue += ".";
                  break;
                case 38:
                  returnValue += " ";
                  break;
                case 39:
                  returnValue += "$";
                  break;
                case 40:
                  returnValue += "/";
                  break;
                case 41:
                  returnValue += "+";
                  break;
                case 42:
                  returnValue += "%";
                  break;
              }
            }
          }
          // Add stop char
          returnValue += "*";
        }
      }
      return returnValue;
    }
  }

}
