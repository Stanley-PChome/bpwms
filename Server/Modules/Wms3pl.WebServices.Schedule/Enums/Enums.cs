using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.WebServices.Schedule.Enums
{
	/// <summary>
	/// 出貨處理計價項目
	/// </summary>
	public enum DeliveryAccKind
	{
		/// <summary>
		/// 揀貨
		/// </summary>
		[Description("04")]
		Pick,
		/// <summary>
		/// 理貨
		/// </summary>
		[Description("05")]
		Tally,
		/// <summary>
		/// 包裝
		/// </summary>
		[Description("06")]
		Package,
		/// <summary>
		/// 包裝耗材費
		/// </summary>
		[Description("02")]
		PackageSupplies,
		/// <summary>
		/// 理出貨費(3pcs內)
		/// </summary>
		[Description("03")]
		DeliveryBase,
		/// <summary>
		/// 包裝理貨併件費
		/// </summary>
		[Description("01")]
		DeliveryAdd,
		/// <summary>
		/// 列印發票費
		/// </summary>
		[Description("07")]
		PrintInvoice,
		/// <summary>
		/// SA申請書
		/// </summary>
		[Description("08")]
		SaApplication,
		/// <summary>
		/// 特殊類別
		/// </summary>
		[Description("XX")]
		Other
	}

	/// <summary>
	/// 作業計價項目
	/// </summary>
	public enum OperationAccKind
	{
		/// <summary>
		/// 進貨驗收
		/// </summary>
		[Description("01")]
		PurchaseCheck,
		/// <summary>
		/// 進貨上架
		/// </summary>
		[Description("02")]
		PurchaseStock,
		/// <summary>
		/// 廠退作業費
		/// </summary>
		[Description("03")]
		VnrReturn,
		/// <summary>
		/// 廠退下架作業費
		/// </summary>
		[Description("04")]
		VnrRemove,
		/// <summary>
		/// 客退驗收作業費
		/// </summary>
		[Description("05")]
		ReturnCheck,
		/// <summary>
		/// 銷毀費
		/// </summary>
		[Description("08")]
		Destroy,
		/// <summary>
		/// 效期管理費
		/// </summary>
		[Description("09")]
		ValidDateManage,
		/// <summary>
		/// 盤點費
		/// </summary>
		[Description("10")]
		Inventory
	}

	public enum DistrCarKind
	{
		/// <summary>
		/// 物流運費
		/// </summary>
		[Description("01")]
		Shipping,
		/// <summary>
		/// 報廢運費
		/// </summary>
		[Description("02")]
		Scrap,
		/// <summary>
		/// 專車運費
		/// </summary>
		[Description("03")]
		SpecialCar
	}
}
