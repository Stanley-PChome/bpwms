using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.Datas.Shared.Enums
{
    public enum WmsLogProcType
    {
        /// <summary>
        /// 配庫
        /// </summary>
        AllotStock,
				// 配庫試算
				AllotStockTC,
				/// <summary>
				/// 庫存異動
				/// </summary>
		    StockChange,
				/// <summary>
				/// 產生揀貨單
				/// </summary>
				CreatePick,
				/// <summary>
				/// 自動產生揀貨單
				/// </summary>
				AutoCreatePick,
				/// <summary>
				/// 自動產生揀缺揀貨單 
				/// </summary>
				AutoCreateLackPick,
				/// <summary>
				/// 揀缺配庫
				/// </summary>
				PickLackAllotStock,
				/// <summary>
				/// 集貨出場確認
				/// </summary>
				CollOutboundConfirm,
    }
}
