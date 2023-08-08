namespace Wms3pl.Datas.F05
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// �K�Q���x��D��
	/// </summary>
	[Serializable]
	[DataServiceKey("DC_CODE", "CONVENIENT_CODE", "CELL_CODE")]
	[Table("F051801")]
	public class F051801 : IAuditInfo
	{
		/// <summary>
		/// ���y���߽s��
		/// </summary>
		[Key]
		[Required]
		public string DC_CODE { get; set; }
		/// <summary>
		/// �K�Q�ܽs��
		/// </summary>
		[Key]
		[Required]
		public string CONVENIENT_CODE { get; set; }
		/// <summary>
		/// �x��s��
		/// </summary>
		[Key]
		[Required]
		public string CELL_CODE { get; set; }
		/// <summary>
		/// �t�ӽs��(F1908)
		/// </summary>
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
		/// ���A(0: ���x��B1: �w�w�ơB2: �w��J)
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
