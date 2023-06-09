using System.Runtime.Serialization;

namespace Wms3pl.WebServices.DataCommon
{
  [DataContract(Namespace = Constants.SmartNameSpace)]
  public class FaultDetail
  {
    public int Code { get; set; }
    public string ExceptionMessage { get; set; }
  }
}
