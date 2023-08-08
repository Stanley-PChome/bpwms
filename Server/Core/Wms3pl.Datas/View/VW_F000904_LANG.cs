namespace Wms3pl.Datas.View
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Services.Common;

    /// <summary>
    /// 程式下拉選單參數設定語系對應檔
    /// </summary>
    [Serializable]
    [DataServiceKey("TOPIC", "SUBTOPIC", "VALUE", "LANG")]
    [Table("VW_F000904_LANG")]
    public class VW_F000904_LANG
    {

        /// <summary>
        /// 程式編號(資料表)
        /// </summary>
        
        [Required]
        public string TOPIC { get; set; }

        /// <summary>
        /// 選單ID
        /// </summary>
        
        [Required]
        public string SUBTOPIC { get; set; }

        /// <summary>
        /// 參數值
        /// </summary>
       
        [Required]
        public string VALUE { get; set; }

        /// <summary>
        /// 參數名稱
        /// </summary>
        public string NAME { get; set; }

        /// <summary>
        /// 選單名稱
        /// </summary>
        public string SUB_NAME { get; set; }

        /// <summary>
        /// 語系
        /// </summary>
       
        public string LANG { get; set; }

        /// <summary>
        /// 是否使用(0否1是)
        /// </summary>
        [Required]
        public string ISUSAGE { get; set; }
    }
}
