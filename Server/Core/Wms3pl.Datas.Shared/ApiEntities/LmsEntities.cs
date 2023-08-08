using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.Datas.Shared.ApiEntities
{
    #region 取得訂單出貨列印清單
    public class PrintJobListRes
    {
        /// <summary>
        /// 表單名稱
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// API網址
        /// </summary>
        public string Url { get; set; }
    }
    #endregion
}
