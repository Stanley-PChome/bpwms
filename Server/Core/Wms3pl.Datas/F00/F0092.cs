using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Services.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F00
{
    [Serializable]
    [DataServiceKey("ID")]
    [Table("F0092")]
    public class F0092 : IAuditInfo
    {
        /// <summary>
        /// 紀錄流水號-SEQ_F0092_ID
        /// </summary>
        [Key]
        [Required]
				[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Int64 ID { get; set; }

        /// <summary>
        /// 作業類別
        /// </summary>
        [Required]
        public string PROC_TYPE { get; set; }

        /// <summary>
        /// 作業批號-{yyyyMMddHHmmss}
        /// </summary>
        [Required]
        public string BATCH_NO { get; set; }

        /// <summary>
        /// 訊息內容
        /// </summary>
        [Required]
        public string PROC_MSG { get; set; }

        /// <summary>
        /// 建立日期
        /// </summary>
        [Required]
        public DateTime CRT_DATE { get; set; }

        /// <summary>
        /// 建立人員
        /// </summary>
        [Required]
        public string CRT_STAFF { get; set; }

        /// <summary>
        /// 建立人名
        /// </summary>
        [Required]
        public string CRT_NAME { get; set; }

        /// <summary>
        /// 異動日期
        /// </summary>
        public DateTime? UPD_DATE { get; set; }

        /// <summary>
        /// 異動人員
        /// </summary>
        public string UPD_STAFF { get; set; }

        /// <summary>
        /// 異動人名
        /// </summary>
        public string UPD_NAME { get; set; }

    }
}
