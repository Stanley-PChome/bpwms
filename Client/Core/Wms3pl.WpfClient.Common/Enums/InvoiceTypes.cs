using Wms3pl.WpfClient.Common.Converters;

namespace Wms3pl.WpfClient.Common.Enums
{
  public enum InvoiceTypes
  {
    /// <summary>
    /// 二聯(個人)
    /// </summary>
    [LocalizableDescription("Duplicate", typeof(Resource))]
    Duplicate = 1,
    /// <summary>
    /// 三聯(公司)
    /// </summary>
    [LocalizableDescription("Triplicate", typeof(Resource))]
    Triplicate,
  
    /// <summary>
    /// 電子式
    /// </summary>
    [LocalizableDescription("Electronic", typeof(Resource))]
    Electronic
  }
}
