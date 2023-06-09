using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wms3pl.WpfClient.Common.Converters;

namespace Wms3pl.WpfClient.Common.Enums
{
  public enum TaxIncludeTypes
  {
    /// <summary>
    /// 外加稅
    /// </summary>
    [LocalizableDescription("TaxNotInclude", typeof(Resource))]
    TaxNotInclude = 1,
    /// <summary>
    /// 內含稅
    /// </summary>
    [LocalizableDescription("TaxIncluded", typeof(Resource))]
    TaxIncluded = 2
  }
}
