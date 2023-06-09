using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wms3pl.WpfClient.Common.Converters;

namespace Wms3pl.WpfClient.Common.Enums
{
  public enum TaxTypes
  {
    /// <summary>
    /// 應稅
    /// </summary>
    [LocalizableDescription("ShouldTax", typeof(Resource))]
    ShouldTax,
    /// <summary>
    /// 免稅
    /// </summary>
    [LocalizableDescription("FreeTax", typeof(Resource))]
    FreeTax,
    /// <summary>
    /// 零稅
    /// </summary>
    [LocalizableDescription("ZeroTax", typeof(Resource))]
    ZeroTax
  }
}
