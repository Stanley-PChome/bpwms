using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wms3pl.WpfClient.Common.Converters;

namespace Wms3pl.WpfClient.Common.Enums
{
  public enum SalesStatuses
  {
     /// <summary>
    /// 準備中
    /// </summary>
    [LocalizableDescription("Preparing", typeof(Resource))]
    Preparing,
    /// <summary>
    /// 已扣帳
    /// </summary>
    [LocalizableDescription("Deducted", typeof(Resource))]
    Deducted,
    /// <summary>
    /// 已送出
    /// </summary>
    [LocalizableDescription("Sent", typeof(Resource))]
    Sent,
    /// <summary>
    /// 回收中
    /// </summary>
    [LocalizableDescription("Recycling", typeof(Resource))]
    Recycling
  }
}
