using System;
using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.ApiEntities
{
    /// <summary>
    /// 出庫任務中介_傳入
    /// </summary>
    public class WcsOutboundReq
    {
        /// <summary>
        /// 業主編號=貨主編號
        /// </summary>
        public string OwnerCode { get; set; }
        /// <summary>
        /// 出庫單號=WMS出貨單號/揀貨單號
        /// </summary>
        public string OrderCode { get; set; }
        /// <summary>
        /// 出庫類型(0=訂單出、1=跨庫調撥出、2=庫內移動出、3=廠退出、4=補貨出)
        /// </summary>
        public int OrderType { get; set; }
        /// <summary>
        /// 允許缺量揀(0=禁止, 1=允許)
        /// </summary>
        public int IsAllowLack { get; set; }
        /// <summary>
        /// 優先處理 (0: 普通、1: 緊急、2: 加急、99999=特級) 
        /// </summary>
        public int Priority { get; set; }
        /// <summary>
        /// 下一步操作提示(ex:集貨、包裝、跨庫調撥、廠退)
        /// </summary>
        public string NextStep { get; set; }
        /// <summary>
        /// 當出貨單類型=0，指定容器為M-則目的地=包裝大小線編號否則為空白
        /// 當出貨單類型=1，則目的地=跨庫物流中心編號、
        /// 當出貨單類型=2，則目的地=倉庫編號
        /// 當出貨單類型 = 3，目的地=空白
        /// </summary>
        public string TargetCode { get; set; }
        /// <summary>
        /// 指定容器(A7) (00: 不限、01: M-周轉箱、02: 2L周轉箱、06=調撥箱)
        /// </summary>
        public string ContainerType { get; set; }
        /// <summary>
        /// 上游出貨單號
        /// </summary>
        public string OriOrderCode { get; set; }
        /// <summary>
        /// 單據是否自我滿足(0=否，1=是)
        /// </summary>
        public int IsSelfSatisfy { get; set; }
        /// <summary>
        /// 明細數
        /// </summary>
        public int SkuTotal { get; set; }
        /// <summary>
        /// 明細資料
        /// </summary>
        public List<WcsOutboundSkuModel> SkuList { get; set; }
    }

    /// <summary>
    /// 明細資料
    /// </summary>
    public class WcsOutboundSkuModel
    {
        /// <summary>
        /// 單據項次
        /// </summary>
        public int RowNum { get; set; }
        /// <summary>
        /// 商品編號
        /// </summary>
        public string SkuCode { get; set; }
        /// <summary>
        /// 預計入庫數量
        /// </summary>
        public int SkuQty { get; set; }
        /// <summary>
        /// 商品等級(0=殘品/客退品, 1=正品/新品) 
        /// </summary>
        public int SkuLevel { get; set; }
        /// <summary>
        /// 商品效期(yyyy/mm/dd)
        /// </summary>
        public string ExpiryDate { get; set; }
        /// <summary>
        /// 外部批次號 = WMS商品入庫日(yyyy/mm/dd)
        /// </summary>
        public string OutBatchCode { get; set; }
    }

    /// <summary>
    /// 出庫任務中介_回傳
    /// </summary>
    public class WcsOutboundResData
    {
        /// <summary>
        /// 錯誤單號
        /// </summary>
        public string OrderCode { get; set; }
        /// <summary>
        /// 錯誤欄位
        /// </summary>
        public string ErrorColumn { get; set; }
        /// <summary>
        /// AGV錯誤回應
        /// </summary>
        public List<WcsErrorModel> errors { get; set; }
    }

    public class WcsOrderNosModel
    {
        public string PickNo { get; set; }
        public int MultiFlag { get; set; }
    }

    public class WmsOrdNoCnt
    {
        public string WmsOrdNo { get; set; }
        public int Cnt { get; set; }
    }
}
