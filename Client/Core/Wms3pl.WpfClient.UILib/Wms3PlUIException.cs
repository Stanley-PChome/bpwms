using System;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;

namespace Wms3pl.WpfClient.UILib
{
  [Serializable]
  public class Wms3plUIException : Exception
  {
    private readonly UserControl _container;

    public Wms3plUIException(UserControl container)
    {
      _container = container;
    }

    public Wms3plUIException(UserControl container, string message)
      : base(message)
    {
      _container = container;
    }

    public Wms3plUIException(UserControl container, string message, Exception inner)
      : base(message, inner)
    {
      _container = container;
    }

    protected Wms3plUIException(UserControl container, SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      _container = container;
    }

    public UserControl Container
    {
      get { return _container; }
    }
  }
}