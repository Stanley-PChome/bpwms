using System;
using System.Collections.Generic;
using System.Data.Services.Common;
using System.Runtime.Serialization;

namespace Wms3pl.Datas.Shared.Entities
{
	public class UseShipContainerItemModel
	{
		public string ITEM_CODE { get; set; }
		public string ALLOWORDITEM { get; set; }
		public string BUNDLE_SERIALNO { get; set; }
		public int QTY { get; set; }
	}

	public class F07010201_partial_Model
	{

		public long F070102_ID { get; set; }
		public string ITEM_CODE { get; set; }
		public string SERIAL_NO { get; set; }

	}

	#region P080805000
	/// <summary>
	/// 揀貨容器資料
	/// </summary>
	[Serializable]
	[DataContract]
	[DataServiceKey("F0701_ID")]
	public class PickContainerInfo
	{
		[DataMember]
		public long F0701_ID { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string PICK_ORD_NO { get; set; }
		[DataMember]
		public string CONTAINER_CODE { get; set; }
		[DataMember]
		public string DEVICE_TYPE { get; set; }
		[DataMember]
		public string DEVICE_TYPE_NAME { get; set; }
		[DataMember]
		public string MOVE_OUT_TARGET { get; set; }
		[DataMember]
		public string CROSS_NAME { get; set; }
		[DataMember]
		public int TOTAL { get; set; }
	}

	/// <summary>
	/// 揀貨容器明細
	/// </summary>
	[Serializable]
	[DataContract]
	[DataServiceKey("ID")]
	public class PickContainerDetail
	{
		[DataMember]
		public long F0701_ID { get; set; }
		[DataMember]
		public string PICK_ORD_NO { get; set; }
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string BUNDLE_SERIALNO { get; set; }
		[DataMember]
		public string SERIAL_NO { get; set; }
		[DataMember]
		public int QTY { get; set; }
		//[DataMember]
		//public long ID { get; set; }
		//[DataMember]
		//public string DC_CODE { get; set; }
		//[DataMember]
		//public string GUP_CODE { get; set; }
		//[DataMember]
		//public string CUST_CODE { get; set; }
		//[DataMember]
		//public string CONTAINER_CODE { get; set; }
		//[DataMember]
		//public string WMS_NO { get; set; }
	}

	/// <summary>
	/// 揀貨容器資訊
	/// </summary>
	[Serializable]
	[DataContract]
	[DataServiceKey("IsSuccessed")]
	public class PickContainerResult : ExecuteResult
	{
		[DataMember]
		public string ContainerCode { get; set; }
		[DataMember]
		public List<PickContainerInfo> PickContainerInfos { get; set; }
		[DataMember]
		public string MoveOutTargetName { get; set; }
		[DataMember]
		public int TotalPcs { get; set; }
	}
	#endregion

	#region P080806000
	/// <summary>
	/// 綁定中揀貨容器資料
	/// </summary>
	[Serializable]
	[DataContract]
	[DataServiceKey("F0534_ID")]
	public class BindingPickContainerInfo
	{
		[DataMember]
		public long F0534_ID { get; set; }
		[DataMember]
		public long F0701_ID { get; set; }
		[DataMember]
		public string CONTAINER_CODE { get; set; }
		[DataMember]
		public string DEVICE_TYPE { get; set; }
		[DataMember]
		public int TOTAL { get; set; }
		[DataMember]
		public string DC_CODE { get; set; }
		[DataMember]
		public string GUP_CODE { get; set; }
		[DataMember]
		public string CUST_CODE { get; set; }
		[DataMember]
		public string PICK_ORD_NO { get; set; }
		[DataMember]
		public string MOVE_OUT_TARGET { get; set; }
		[DataMember]
		public string CROSS_NAME { get; set; }
		[DataMember]
		public string HAS_CP_ITEM { get; set; }
		[DataMember]
		public string ALL_CP_ITEM { get; set; }

		[DataMember]
		public List<BindingPickContainerDetail> ItemList { get; set; }
	}

	/// <summary>
	/// 綁定中揀貨容器明細
	/// </summary>
	[Serializable]
	[DataContract]
	[DataServiceKey("ID")]
	public class BindingPickContainerDetail
	{
		[DataMember]
		public string ITEM_CODE { get; set; }
		[DataMember]
		public string ITEM_NAME { get; set; }
		[DataMember]
		public string EAN_CODE1 { get; set; }
		[DataMember]
		public string EAN_CODE2 { get; set; }
		[DataMember]
		public string EAN_CODE3 { get; set; }
		[DataMember]
		public string BUNDLE_SERIALNO { get; set; }
		[DataMember]
		public int B_SET_QTY { get; set; }
		[DataMember]
		public int A_SET_QTY { get; set; }
	}

	/// <summary>
	/// 綁定揀貨容器資訊
	/// </summary>
	[Serializable]
	[DataContract]
	[DataServiceKey("IsSuccessed")]
	public class BindingPickContainerResult : ExecuteResult
	{
		[DataMember]
		public BindingPickContainerInfo BindingPickContainerInfo { get; set; }
		[DataMember]
		public bool IsReleaseContainer { get; set; }
	}

  /// <summary>
	/// 綁定中揀貨容器明細
	/// </summary>
	[Serializable]
  [DataContract]
  [DataServiceKey("PICK_ORD_NO")]
  public class MoveOutPickOrders
  {
    [DataMember]
    public DateTime DELV_DATE { get; set; }

    [DataMember]
    public string PICK_TIME { get; set; }

    [DataMember]
    public string PICK_ORD_NO { get; set; }

    [DataMember]
    public string MOVE_OUT_TARGET_NAME { get; set; }

    [DataMember]
    public string PICK_STATUS_NAME { get; set; }

    [DataMember]
    public string PICK_TOOL_NAME { get; set; }

    [DataMember]
    public string PK_AREA_NAME { get; set; }
  }

  #endregion
}
