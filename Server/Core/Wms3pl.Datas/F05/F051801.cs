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
    [Column(TypeName = "varchar(3)")]
    public string DC_CODE { get; set; }
		/// <summary>
		/// �K�Q�ܽs��
		/// </summary>
		[Key]
		[Required]
    [Column(TypeName = "varchar(6)")]
    public string CONVENIENT_CODE { get; set; }
		/// <summary>
		/// �x��s��
		/// </summary>
		[Key]
		[Required]
    [Column(TypeName = "nvarchar(10)")]
    public string CELL_CODE { get; set; }
    /// <summary>
    /// �t�ӽs��(F1908)
    /// </summary>
    [Column(TypeName = "varchar(20)")]
    public string VNR_CODE { get; set; }
		/// <summary>
		/// �~�D�s��
		/// </summary>
    [Column(TypeName = "varchar(4)")]
    public string GUP_CODE { get; set; }
		/// <summary>
		/// �f�D�s��
		/// </summary>
    [Column(TypeName = "varchar(6)")]
    public string CUST_CODE { get; set; }
		/// <summary>
		/// ���A(0: ���x��B1: �w�w�ơB2: �w��J)
		/// </summary>
		[Required]
    [Column(TypeName = "varchar(1)")]
    public string STATUS { get; set; }
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
