using System;

namespace Wms3pl.WpfClient.Common.WcfDataServices
{
  class Util
  {
    internal static int IndexOfReference<T>(T[] array, T value) where T : class
    {
      for (int i = 0; i < array.Length; ++i)
      {
        if (object.ReferenceEquals(array[i], value))
        {
          return i;
        }
      }

      return -1;
    }

    internal static Uri CreateUri(Uri baseUri, Uri requestUri)
    {
      if (!requestUri.IsAbsoluteUri)
      {
        if (baseUri.OriginalString.EndsWith("/", StringComparison.Ordinal))
        {
          if (requestUri.OriginalString.StartsWith("/", StringComparison.Ordinal))
          {
            requestUri = new Uri(baseUri, Util.CreateUri(requestUri.OriginalString.TrimStart(Util.ForwardSlash), UriKind.Relative));
          }
          else
          {
            requestUri = new Uri(baseUri, requestUri);
          }
        }
        else
        {
          requestUri = Util.CreateUri(baseUri.OriginalString + "/" + requestUri.OriginalString.TrimStart(Util.ForwardSlash), UriKind.Absolute);
        }
      }

      return requestUri;
    }

    internal static Uri CreateUri(string value, UriKind kind)
    {
      return value == null ? null : new Uri(value, kind);
    }

    internal static readonly char[] ForwardSlash = new char[1] { '/' };
  }
}
