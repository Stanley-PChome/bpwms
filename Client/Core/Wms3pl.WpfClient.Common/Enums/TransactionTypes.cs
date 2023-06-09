using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wms3pl.WpfClient.Common.Converters;

namespace Wms3pl.WpfClient.Common.Enums
{
  public enum TransactionTypes
  {
    /// <summary>
    /// 進貨
    /// </summary>
    [LocalizableDescription("In", typeof(Resource))]
    In, 
    /// <summary>
    /// 退貨
    /// </summary>
    [LocalizableDescription("Out", typeof(Resource))]
    Out
  }

  class MyClass
  {
    private const string xxx = "In";
  }
}
