using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wms3pl.WpfClient.Common.Converters;

namespace Wms3pl.WpfClient.Common.Enums
{
  public enum PayTypes
  {
    /// <summary>
    /// 現金
    /// </summary>
    [LocalizableDescription("ByCash", typeof(Resource))]
    ByCash = 1,
    /// <summary>
    /// 支票
    /// </summary>
    [LocalizableDescription("Bycheck", typeof(Resource))]
    Bycheck,
    /// <summary>
    /// 扣款
    /// </summary>
    [LocalizableDescription("ByDebit", typeof(Resource))]
    ByDebit
  }
}
