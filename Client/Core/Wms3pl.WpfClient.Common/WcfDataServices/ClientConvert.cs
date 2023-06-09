using System;
using System.Xml;
using Wms3pl.WpfClient.DataServices.WcfDataServices;

namespace Wms3pl.WpfClient.Common.WcfDataServices
{
  class ClientConvert
  {
    private static readonly Type[] knownTypes = CreateKnownPrimitives();
    internal enum StorageType
    {
      Boolean,
      Byte,
      ByteArray,
      Char,
      CharArray,
      DateTime,
      DateTimeOffset,
      Decimal,
      Double,
      Guid,
      Int16,
      Int32,
      Int64,
      Single,
      String,
      SByte,
      TimeSpan,
      Type,
      UInt16,
      UInt32,
      UInt64,
      Uri,
      XDocument,
      XElement,
      Binary,
    }

    internal static object ChangeType(string propertyValue, Type propertyType)
    {
      try
      {
        switch ((StorageType)IndexOfStorage(propertyType))
        {
          case StorageType.Boolean:
            return XmlConvert.ToBoolean(propertyValue);
          case StorageType.Byte:
            return XmlConvert.ToByte(propertyValue);
          case StorageType.ByteArray:
            return Convert.FromBase64String(propertyValue);
          case StorageType.Char:
            return XmlConvert.ToChar(propertyValue);
          case StorageType.CharArray:
            return propertyValue.ToCharArray();
          case StorageType.DateTime:
            return XmlConvert.ToDateTime(propertyValue, XmlDateTimeSerializationMode.RoundtripKind);
          //                   case StorageType.DateTimeOffset:
          //                       return XmlConvert.ToDateTimeOffset(propertyValue);
          case StorageType.Decimal:
            return XmlConvert.ToDecimal(propertyValue);
          case StorageType.Double:
            return XmlConvert.ToDouble(propertyValue);
          case StorageType.Guid:
            return new Guid(propertyValue);
          case StorageType.Int16:
            return XmlConvert.ToInt16(propertyValue);
          case StorageType.Int32:
            return XmlConvert.ToInt32(propertyValue);
          case StorageType.Int64:
            return XmlConvert.ToInt64(propertyValue);
          case StorageType.Single:
            return XmlConvert.ToSingle(propertyValue);
          case StorageType.String:
            return propertyValue;
          case StorageType.SByte:
            return XmlConvert.ToSByte(propertyValue);
          case StorageType.TimeSpan:
            return XmlConvert.ToTimeSpan(propertyValue);
          case StorageType.Type:
            return Type.GetType(propertyValue, true);
          case StorageType.UInt16:
            return XmlConvert.ToUInt16(propertyValue);
          case StorageType.UInt32:
            return XmlConvert.ToUInt32(propertyValue);
          case StorageType.UInt64:
            return XmlConvert.ToUInt64(propertyValue);
          case StorageType.Uri:
            return Util.CreateUri(propertyValue, UriKind.RelativeOrAbsolute);
          case StorageType.XDocument:
            return (0 < propertyValue.Length ? System.Xml.Linq.XDocument.Parse(propertyValue) : new System.Xml.Linq.XDocument());
          case StorageType.XElement:
            return System.Xml.Linq.XElement.Parse(propertyValue);
          case StorageType.Binary:
            return Activator.CreateInstance(knownTypes[(int)StorageType.Binary]);
          default:
            return propertyValue;
        }
      }
      catch (FormatException ex)
      {
        propertyValue = (0 == propertyValue.Length ? "String.Empty" : "String");
        throw new InvalidOperationException(propertyValue, ex);
      }
      catch (OverflowException ex)
      {
        propertyValue = (0 == propertyValue.Length ? "String.Empty" : "String");
        throw new InvalidOperationException(propertyValue, ex);
      }
    }

    internal static bool IsKnownNullableType(Type type)
    {
      return IsKnownType(Nullable.GetUnderlyingType(type) ?? type);
    }

    internal static bool IsKnownType(Type type)
    {
      return (0 <= IndexOfStorage(type));
    }


    private static int IndexOfStorage(Type type)
    {
      int index = Util.IndexOfReference(ClientConvert.knownTypes, type);
      return index;
    }

    private static Type[] CreateKnownPrimitives()
    {
      Type[] types = new Type[1 + (int)StorageType.Binary];
      types[(int)StorageType.Boolean] = typeof(Boolean);
      types[(int)StorageType.Byte] = typeof(Byte);
      types[(int)StorageType.ByteArray] = typeof(Byte[]);
      types[(int)StorageType.Char] = typeof(Char);
      types[(int)StorageType.CharArray] = typeof(Char[]);
      types[(int)StorageType.DateTime] = typeof(DateTime);
      types[(int)StorageType.Decimal] = typeof(Decimal);
      types[(int)StorageType.Double] = typeof(Double);
      types[(int)StorageType.Guid] = typeof(Guid);
      types[(int)StorageType.Int16] = typeof(Int16);
      types[(int)StorageType.Int32] = typeof(Int32);
      types[(int)StorageType.Int64] = typeof(Int64);
      types[(int)StorageType.Single] = typeof(Single);
      types[(int)StorageType.String] = typeof(String);
      types[(int)StorageType.SByte] = typeof(SByte);
      types[(int)StorageType.TimeSpan] = typeof(TimeSpan);
      types[(int)StorageType.Type] = typeof(Type);
      types[(int)StorageType.UInt16] = typeof(UInt16);
      types[(int)StorageType.UInt32] = typeof(UInt32);
      types[(int)StorageType.UInt64] = typeof(UInt64);
      types[(int)StorageType.Uri] = typeof(Uri);
      types[(int)StorageType.XDocument] = typeof(System.Xml.Linq.XDocument);
      types[(int)StorageType.XElement] = typeof(System.Xml.Linq.XElement);
      types[(int)StorageType.Binary] = null;
      return types;
    }
  }
}
