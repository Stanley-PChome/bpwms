using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.Common.Enums
{
	public static class EnumExtensions
	{
		public static string DisplayName(this Enum value)
		{
			var enumType = value.GetType();
			var enumValue = Enum.GetName(enumType, value);
			var member = enumType.GetMember(enumValue)[0];
			var attrs = member.GetCustomAttributes(typeof(DisplayAttribute), false);
			var outString = ((DisplayAttribute)attrs[0]).Name;

			if (((DisplayAttribute)attrs[0]).ResourceType != null)
			{
				outString = ((DisplayAttribute)attrs[0]).GetName();
			}
			return outString;
		}

		public static string GetDescriptionFromValue(this Enum value)
		{
			var attribute = value.GetType()
				.GetField(value.ToString())
				.GetCustomAttributes(typeof(DescriptionAttribute), false)
				.SingleOrDefault() as DescriptionAttribute;
			return attribute == null ? value.ToString() : attribute.Description;
		}

		public static T GetValueFromDescription<T>(string description)
		{
			var type = typeof(T);
			if (!type.IsEnum) throw new InvalidOperationException();
			foreach (var field in type.GetFields())
			{
				var attribute = Attribute.GetCustomAttribute(field,
						typeof(DescriptionAttribute)) as DescriptionAttribute;
				if (attribute != null)
				{
					if (attribute.Description == description)
						return (T)field.GetValue(null);
				}
				else
				{
					if (field.Name == description)
						return (T)field.GetValue(null);
				}
			}
			throw new ArgumentException("Not found.", "description");
		}
	}

	/// <summary>
	/// 條碼格式
	/// </summary>
	public enum BarcodeType
	{
		/// <summary>
		/// 非存在條碼
		/// </summary>
		[Display(Name = "非存在條碼")]
		None = -1,
		/// <summary>
		/// 儲值卡盒號
		/// </summary>
		[Display(Name = "儲值卡盒號")]
		BatchNo = 0,
		/// <summary>
		/// 盒號
		/// </summary>
		[Display(Name = "盒號")]
		BoxSerial,
		/// <summary>
		/// 箱號
		/// </summary>
		[Display(Name = "箱號")]
		CaseNo,
		/// <summary>
		/// 序號
		/// </summary>
		[Display(Name = "序號")]
		SerialNo
	}

	public enum ProcessWork
	{
		/// <summary>
		/// 序號刷驗
		/// </summary>
		[Description("A1")]
		ScanSerial,
		/// <summary>
		/// 組合商品
		/// </summary>
		[Description("A2")]
		CombinItem,
		/// <summary>
		/// 序號商品拆解
		/// </summary>
		[Description("A3")]
		Disassemble,
		/// <summary>
		/// 裝盒
		/// </summary>
		[Description("A4")]
		Boxing,
		/// <summary>
		/// 裝箱
		/// </summary>
		[Description("A5")]
		Goxing,
		/// <summary>
		/// 盒QC
		/// </summary>
		[Description("A6")]
		BoxQc,
		/// <summary>
		/// 箱QC
		/// </summary>
		[Description("A7")]
		GoxQc,
		/// <summary>
		/// 卡片QC
		/// </summary>
		[Description("A8")]
		CardQc
	}

	public enum QuoteType
	{
		/// <summary>
		/// 加工
		/// </summary>
		[Description("001")]
		Process,
		/// <summary>
		/// 儲位
		/// </summary>
		[Description("002")]
		Warehousing,
		/// <summary>
		/// 作業
		/// </summary>
		[Description("003")]
		Operation,
		/// <summary>
		/// 出貨
		/// </summary>
		[Description("004")]
		Delivery,
		/// <summary>
		/// 派車
		/// </summary>
		[Description("005")]
		DistrCar,
		/// <summary>
		/// 其他
		/// </summary>
		[Description("006")]
		Other,
		/// <summary>
		/// 專案
		/// </summary>
		[Description("007")]
		Project,
		/// <summary>
		/// 配送商
		/// </summary>
		[Description("008")]
		Distribution			
	}

}
