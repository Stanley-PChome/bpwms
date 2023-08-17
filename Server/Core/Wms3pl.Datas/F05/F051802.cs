namespace Wms3pl.Datas.F05
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Services.Common;
	using Wms3pl.WebServices.DataCommon;

	/// <summary>
	/// �K�Q���x�����
	/// </summary>
	[Serializable]
	[DataServiceKey("ID")]
	[Table("F051802")]
	public class F051802 : IAuditInfo
	{
		/// <summary>
		/// �y����
		/// </summary>
		[Key]
		[Required]
    [Column(TypeName = "bigint")]
    public long ID { get; set; }
		/// <summary>
		/// ���y���߽s��
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(3)")]
    public string DC_CODE { get; set; }
		/// <summary>
		/// �K�Q�ܽs��
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(6)")]
    public string CONVENIENT_CODE { get; set; }
		/// <summary>
		/// �x��s��
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(10)")]
    public string CELL_CODE { get; set; }
		/// <summary>
		/// �t�ӽs��(F1908)
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(20)")]
    public string VNR_CODE { get; set; }
		/// <summary>
		/// �~�D�s��
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(4)")]
    public string GUP_CODE { get; set; }
		/// <summary>
		/// �f�D�s��
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(6)")]
    public string CUST_CODE { get; set; }
		/// <summary>
		/// �渹
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(20)")]
    public string WMS_NO { get; set; }
    /// <summary>
    /// �f�D�渹
    /// </summary>
    [Column(TypeName = "varchar(50)")]
    public string CUST_ORD_NO { get; set; }
		/// <summary>
		/// �إߤ��
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(20)")]
    public string CRT_STAFF { get; set; }
		/// <summary>
		/// �إߤH���s��
		/// </summary>
		[Required]
    [Column(TypeName = "datetime2(0)")]
    public DateTime CRT_DATE { get; set; }
		/// <summary>
		/// �إߤH���W��
		/// </summary>
		[Required]
    [Column(TypeName = "nvarchar(16)")]
    public string CRT_NAME { get; set; }
    /// <summary>
    /// ���ʤ��
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string UPD_STAFF { get; set; }
    /// <summary>
    /// ���ʤH���s��
    /// </summary>
    [Column(TypeName = "datetime2(0)")]
    public DateTime? UPD_DATE { get; set; }
    /// <summary>
    /// ���ʤH���W��
    /// </summary>
    [Column(TypeName = "nvarchar(16)")]
    public string UPD_NAME { get; set; }
	}
}
