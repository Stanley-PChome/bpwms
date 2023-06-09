using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wms3pl.WpfClient.Common.Converters;

namespace Wms3pl.WpfClient.Common.Enums
{
  public enum RetailTypes
  {
    /// <summary>
    /// 商品販售
    /// </summary>
    [LocalizableDescription("Sale", typeof(Resource))]
    Sale = 0,
    /// <summary>
    /// 客戶領用
    /// </summary>
    [LocalizableDescription("CustomerUse", typeof(Resource))]
    CustomerUse = 1,
    /// <summary>
    /// ＤＣ自領
    /// </summary>
    [LocalizableDescription("DcUse", typeof(Resource))]
    DcUse = 2,
    /// <summary>
    /// ＤＣ餽贈
    /// </summary>
    //[LocalizableDescription("DcUseAsGift", typeof(Resource))]
    //DcUseAsGift = 3
  }
}
