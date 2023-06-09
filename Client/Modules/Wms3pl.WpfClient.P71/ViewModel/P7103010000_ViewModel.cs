using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P71.ViewModel
{
  public partial class P7103010000_ViewModel : InputViewModelBase
  {
    public P7103010000_ViewModel()
    {
      if (!IsInDesignMode)
      {
        //初始化執行時所需的值及資料
      }
    }
    public List<DgDataClass> DgItemSource { get; set; }
    public List<DgDataClass> DgItemSource2 { get; set; }
    public List<DgDataClass> DgItemSource3 { get; set; }
    public List<DgDataClass> DgItemSource4 { get; set; }
    public List<DgDataClass> DgItemSource5 { get; set; }
    public class DgDataClass
    {
      public string Str1 { get; set; }
      public string Str2 { get; set; }
      public string Str3 { get; set; }
      public string Str4 { get; set; }
      public string Str5 { get; set; }
      public string Str6 { get; set; }
      public string Str7 { get; set; }
      public string Str8 { get; set; }
      public string Str9 { get; set; }
      public string Str10 { get; set; }

      public bool Bool1 { get; set; }
      public bool Bool2 { get; set; }
      public bool Bool3 { get; set; }
      public bool Bool4 { get; set; }
      public bool Bool5 { get; set; }

      public int Int1 { get; set; }
      public int Int2 { get; set; }
      public int Int3 { get; set; }
    }
  }
}
