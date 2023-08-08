namespace Wms3pl.Datas.F00
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data.Services.Common;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;
  using Wms3pl.WebServices.DataCommon;

  [Serializable]
  [DataServiceKey("ID")]
  [Table("F0000")]

  public class F0000 : IAuditInfo
  {
    /// <summary>
    /// 流水號
    /// </summary>
    [Key]
    [Required]
    public long ID { get; set; }
    /// <summary>
    /// 取號鎖定資料表
    /// </summary>
    [Required]
    public string UPD_LOCK_TABLE_NAME { get; set; }

    public string UPD_LOCK_API_NAME { get; set; }
    /// <summary>
    /// 是否鎖定
    /// </summary>
    public string IS_LOCK { get; set; }
    /// <summary>
    /// 建立日期
    /// </summary>
    [Required]
    public DateTime CRT_DATE { get; set; }
    /// <summary>
    /// 建立人員編號
    /// </summary>
    [Required]
    public string CRT_STAFF { get; set; }
    /// <summary>
    /// 建立人員名稱
    /// </summary>
    [Required]
    public string CRT_NAME { get; set; }
    /// <summary>
    /// 異動日期
    /// </summary>
    public DateTime? UPD_DATE { get; set; }
    /// <summary>
    /// 異動人員編號
    /// </summary>
    public string UPD_STAFF { get; set; }
    /// <summary>
    /// 異動人員名稱
    /// </summary>
    public string UPD_NAME { get; set; }
    // */
  }
}
