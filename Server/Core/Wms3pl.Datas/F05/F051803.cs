namespace Wms3pl.Datas.F05
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// �K�Q���x��i�X����
	/// </summary>
	[Serializable]
	[DataServiceKey("ID")]
	[Table("F051803")]
	public class F051803 : IAuditInfo
	{
		/// <summary>
		/// �y����
		/// </summary>
		[Key]
		[Required]
		public long ID { get; set; }
		/// <summary>
		/// ���y���߽s��
		/// </summary>
		[Required]
		public string DC_CODE { get; set; }
		/// <summary>
		/// �K�Q�ܽs��
		/// </summary>
		[Required]
		public string CONVENIENT_CODE { get; set; }
		/// <summary>
		/// �x��s��
		/// </summary>
		[Required]
		public string CELL_CODE { get; set; }
		/// <summary>
		/// �t�ӽs��(F1908)
		/// </summary>
		[Required]
		public string VNR_CODE { get; set; }
		/// <summary>
		/// �~�D�s��
		/// </summary>
		[Required]
		public string GUP_CODE { get; set; }
		/// <summary>
		/// �f�D�s��
		/// </summary>
		[Required]
		public string CUST_CODE { get; set; }
		/// <summary>
		/// �渹
		/// </summary>
		[Required]
		public string WMS_NO { get; set; }
		/// <summary>
		/// �f�D�渹
		/// </summary>
		public string CUST_ORD_NO { get; set; }
		/// <summary>
		/// �ʧ@���A(1: �J���A2:�X��)
		/// </summary>
		[Required]
		public string STATUS { get; set; }
		/// <summary>
		/// �إߤ��
		/// </summary>
		[Required]
		public string CRT_STAFF { get; set; }
		/// <summary>
		/// �إߤH���s��
		/// </summary>
		[Required]
		public DateTime CRT_DATE { get; set; }
		/// <summary>
		/// �إߤH���W��
		/// </summary>
		[Required]
		public string CRT_NAME { get; set; }
		/// <summary>
		/// ���ʤ��
		/// </summary>
		public string UPD_STAFF { get; set; }
		/// <summary>
		/// ���ʤH���s��
		/// </summary>
		public DateTime? UPD_DATE { get; set; }
		/// <summary>
		/// ���ʤH���W��
		/// </summary>
		public string UPD_NAME { get; set; }
	}
}
