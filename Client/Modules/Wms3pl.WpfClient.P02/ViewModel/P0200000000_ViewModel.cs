using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.P02.Services;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P02.ViewModel
{
	/// <summary>
	/// UI繪製用Class,開發後無用須刪除
	/// </summary>
	public partial class P0200000000_ViewModel : InputViewModelBase
	{
		public P0200000000_ViewModel()
		{
			
		}

		public List<DgDataClassP020101> DgItemSourceP020101 { get; set; }
		public List<DgDataClassP020103> DgItemSourceP020103 { get; set; }

		public List<DgDataClassP020202> DgItemSourceP020202 { get; set; }

		public List<DgDataClassP02020203> DgItemSourceP02020203 { get; set; }

		public class DgDataClassP020101
		{
			public string OrderDate { get; set; }
			public string OrderNo { get; set; }
			public string VnrCode { get; set; }
			public string VnrName { get; set; }
			public string PierType { get; set; }
			public string PierNo { get; set; }
			public string Str1 { get; set; }
			public string Str2 { get; set; }
			public bool Bool1 { get; set; }
		}
		public class DgDataClassP020103
		{
			public string OrderDate { get; set; }
			public string OrderNo { get; set; }
			public string VnrCode { get; set; }
			public string VnrName { get; set; }
			public string PierType { get; set; }
			public string PierNo { get; set; }
			public string BookTime { get; set; }
			public string InTime { get; set; }
			public string OutTime { get; set; }
			public string CarNo { get; set; }
			public string Work { get; set; }
			public string TotalTime { get; set; }

		}

		public class DgDataClassP020202
		{
			public string OrderNo { get; set; }
			public string ItemCode { get; set; }
			public string ItemName { get; set; }
			public decimal OrderQty { get; set; }
			public decimal PreAcceptQty { get; set; }
			public decimal AcceptQty { get; set; }

			public bool Check1 { get; set; }

			public string Str1 { get; set; }
			public string Str2 { get; set; }
		}

		public class DgDataClassP02020203
		{
			public string FileType { get; set; }
			public string FileDesc { get; set; }
			public decimal Qty { get; set; }
			
		}

		public List<DgDataClass> DgItemSource { get; set; }
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

			public string Str11 { get; set; }
			public string Str12 { get; set; }
			public string Str13 { get; set; }


			public bool Bool1 { get; set; }
			public bool Bool2 { get; set; }
			public bool Bool3 { get; set; }
			public bool Bool4 { get; set; }
			public bool Bool5 { get; set; }

			public int Int1 { get; set; }
			public int Int2 { get; set; }
			public int Int3 { get; set; }
		}

		public List<DelvNo> TreeSource { get; set; }
		
	}
}
