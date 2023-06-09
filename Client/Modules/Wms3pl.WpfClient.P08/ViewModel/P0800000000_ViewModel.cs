using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P08.ViewModel
{
	/// <summary>
	/// UI繪製用Class,開發後無用須刪除
	/// </summary>
	public partial class P0800000000_ViewModel : InputViewModelBase
	{
		public P0800000000_ViewModel()
		{

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
