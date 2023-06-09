using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F02;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.F91;
using Wms3pl.Datas.Shared;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.DataCommon.Enums;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.PdaWebApi.Business.Services
{
  public partial class P81Service
  {
    private WmsTransaction _wmsTransation;
    public P81Service(WmsTransaction wmsTransation = null)
    {
      _wmsTransation = wmsTransation;
    }

    /// <summary>
    /// 取得訊息代碼定義資料
    /// </summary>
    /// <returns></returns>
    private List<MsgModel> GetMsgData()
    {
      // 目前先寫死資料
      return new List<MsgModel>
      { 
        #region 00共用作業
        new MsgModel{ MsgCode = "10001", MsgContent = "取得資料成功" },
        new MsgModel{ MsgCode = "10002", MsgContent = "更新成功" },
        new MsgModel{ MsgCode = "10003", MsgContent = "下架成功" },
        new MsgModel{ MsgCode = "10004", MsgContent = "上架成功" },
        new MsgModel{ MsgCode = "10005", MsgContent = "執行成功" },
        new MsgModel{ MsgCode = "10006", MsgContent = "商品放入成功，請回收容器" },
        new MsgModel{ MsgCode = "10007", MsgContent = "第一箱容器進場成功" },
        new MsgModel{ MsgCode = "10008", MsgContent = "此容器出場成功" },
        new MsgModel{ MsgCode = "10009", MsgContent = "揀貨完成，請到{0}" },
        new MsgModel{ MsgCode = "10010", MsgContent = "檢驗成功" },
        new MsgModel{ MsgCode = "10011", MsgContent = "複驗通過" },
        new MsgModel{ MsgCode = "10012", MsgContent = "新容器請移至異常區。原容器有商品複驗不通過，也請移異常區" },
        new MsgModel{ MsgCode = "10013", MsgContent = "本商品複驗不通過，但容器中仍有商品尚未複驗" },
        new MsgModel{ MsgCode = "10014", MsgContent = "容器已複驗/不須複驗" },
        new MsgModel{ MsgCode = "10015", MsgContent = "本商品複驗通過，但容器有複驗失敗的商品，請移異常區" },
        new MsgModel{ MsgCode = "10016", MsgContent = "本商品改為不須複驗，但容器中有複驗失敗的商品，請移至異常區" },
        new MsgModel{ MsgCode = "10017", MsgContent = "本商品改為不須複驗。本容器中的所有商品都已完成複驗" },
        new MsgModel{ MsgCode = "10018", MsgContent = "商品已轉入新容器。因為複驗不通過，請將新容器移動到異常區" },
        new MsgModel{ MsgCode = "10019", MsgContent = "複驗不通過，請將容器移動到異常區" },
        new MsgModel{ MsgCode = "10020", MsgContent = "出貨箱進場成功" },
        new MsgModel{ MsgCode = "10021", MsgContent = "此廠退出貨單出場成功!" },
        new MsgModel{ MsgCode = "10022",MsgContent = "接受任務成功"},
        new MsgModel{ MsgCode = "10023",MsgContent = "放棄任務成功"},
        new MsgModel{ MsgCode = "10024",MsgContent = "完成補貨成功"},
        new MsgModel{ MsgCode = "10025",MsgContent = "商品已轉入新容器。因為複驗不通過，請將新容器移動到異常區，原容器已無任何商品，請回收"},

        new MsgModel{ MsgCode = "20050", MsgContent = "{0}不得為空" },
        new MsgModel{ MsgCode = "20051", MsgContent = "取得資料失敗" },
        new MsgModel{ MsgCode = "20052", MsgContent = "更新失敗" },
        new MsgModel{ MsgCode = "20053", MsgContent = "無此儲位" },
        new MsgModel{ MsgCode = "20054", MsgContent = "上架儲位凍結進" },
        new MsgModel{ MsgCode = "20055", MsgContent = "您無此儲位權限" },
        new MsgModel{ MsgCode = "20056", MsgContent = "上架儲位非此貨主儲位" },
        new MsgModel{ MsgCode = "20057", MsgContent = "此商品不允許混批" },
        new MsgModel{ MsgCode = "20058", MsgContent = "此商品不允許混品" },
        new MsgModel{ MsgCode = "20059", MsgContent = "數量必須大於0" },
        new MsgModel{ MsgCode = "20060", MsgContent = "已超過剩餘應上架數" },
        new MsgModel{ MsgCode = "20061", MsgContent = "此單據已被其他作業人員使用" },
        new MsgModel{ MsgCode = "20062", MsgContent = "下架儲位凍結出" },
        new MsgModel{ MsgCode = "20063", MsgContent = "已超過剩餘應下架數" },
        new MsgModel{ MsgCode = "20064", MsgContent = "單據不存在" },
        new MsgModel{ MsgCode = "20065", MsgContent = "單據已刪除" },
        new MsgModel{ MsgCode = "20066", MsgContent = "單據已完成" },
        new MsgModel{ MsgCode = "20067", MsgContent = "單據明細已完成下架，下架失敗" },
        new MsgModel{ MsgCode = "20068", MsgContent = "單據明細已完成上架，上架失敗" },
        new MsgModel{ MsgCode = "20069", MsgContent = "傳入的參數驗證失敗。" },
        new MsgModel{ MsgCode = "20070", MsgContent = "目前的單據狀態非此作業可執行。" },
        new MsgModel{ MsgCode = "20071", MsgContent = "儲位{0}溫層{1}不符合商品{2}溫層{3}" },
        new MsgModel{ MsgCode = "20072", MsgContent = "該商品{0}的溫層資料不存在" },
        new MsgModel{ MsgCode = "20073", MsgContent = "該儲位{0}的溫層資料不存在" },
        new MsgModel{ MsgCode = "20074", MsgContent = "此儲位已經存放其他貨主的商品" },
        new MsgModel{ MsgCode = "30001", MsgContent = "該單據已有人員(%s)進行作業中，請問是否要更換作業人員?" },
        #endregion

        #region 01權限作業
        new MsgModel{ MsgCode = "10101", MsgContent = "登入成功。" },
        new MsgModel{ MsgCode = "10102", MsgContent = "登出成功" },
        new MsgModel{ MsgCode = "20151", MsgContent = "您輸入的資料有誤，請重新輸入。" },
        new MsgModel{ MsgCode = "20152", MsgContent = "登出失敗" },
        new MsgModel{ MsgCode = "20153", MsgContent = "有新版本，請下載最新版本" },
        new MsgModel{ MsgCode = "20154", MsgContent = "此帳號已在其他裝置登入" },
        #endregion

        #region 02主檔作業
        new MsgModel{ MsgCode = "20251", MsgContent = "取得商品資料失敗" },
        new MsgModel{ MsgCode = "20252", MsgContent = "取得商品序號資料失敗" },
        #endregion

        #region 03調撥作業
        new MsgModel{ MsgCode = "20351", MsgContent = "取得調撥單據資料失敗" },
        new MsgModel{ MsgCode = "20352", MsgContent = "取得調撥明細資料失敗" },
        new MsgModel{ MsgCode = "20353", MsgContent = "此容器{0}無法找到對應的調撥單號" },
        new MsgModel{ MsgCode = "20354", MsgContent = "此儲位{0}不存在" },
        new MsgModel{ MsgCode = "20355", MsgContent = "此儲位{0}不存在於{1}" },
        new MsgModel{ MsgCode = "20356", MsgContent = "此序號{0}無法找到對應的調撥單號" },
        #endregion

        #region 04 出貨作業
        new MsgModel{ MsgCode = "20451", MsgContent = "取得揀貨單據資料失敗" },
        new MsgModel{ MsgCode = "20452", MsgContent = "取得揀貨明細資料失敗" },
        new MsgModel{ MsgCode = "20453", MsgContent = "揀貨數量必須大於等於0" },
        new MsgModel{ MsgCode = "20454", MsgContent = "缺貨數量必須大於0" },
        new MsgModel{ MsgCode = "20455", MsgContent = "揀貨數量已超過剩餘應揀數" },
        new MsgModel{ MsgCode = "20456", MsgContent = "缺貨數量已超過剩餘應揀數" },
        new MsgModel{ MsgCode = "20457", MsgContent = "揀貨明細已被[{0}]揀貨完成" },
        new MsgModel{ MsgCode = "20458", MsgContent = "揀貨明細已取消" },
        new MsgModel{ MsgCode = "20459", MsgContent = "揀貨明細不存在" },
        new MsgModel{ MsgCode = "20460", MsgContent = "訂單已取消，不須綁定與揀貨" },
        #endregion

        #region 05 庫存作業
        new MsgModel{ MsgCode = "20551", MsgContent = "取得庫存資料失敗" },
        #endregion

        #region 06 盤點作業
        new MsgModel{ MsgCode = "20651", MsgContent = "取得盤點單據資料失敗" },
        new MsgModel{ MsgCode = "20652", MsgContent = "取得盤點明細資料失敗" },
        new MsgModel{ MsgCode = "20601", MsgContent = "盤點資料確認成功" },
        new MsgModel{ MsgCode = "20602", MsgContent = "盤點資料新增成功" },
        new MsgModel{ MsgCode = "20653", MsgContent = "盤點數量應不小於0" },
        new MsgModel{ MsgCode = "20654", MsgContent = "盤點單號不存在" },
        new MsgModel{ MsgCode = "20655", MsgContent = "品號不存在" },
        new MsgModel{ MsgCode = "20656", MsgContent = "儲位不存在" },
        new MsgModel{ MsgCode = "20657", MsgContent = "盤點資料不存在" },
        new MsgModel{ MsgCode = "20658", MsgContent = "盤點資料已存在  不可新增" },
        new MsgModel{ MsgCode = "21951", MsgContent = "序號不存在"},
        new MsgModel{ MsgCode = "20659", MsgContent = "該儲位的倉別不存在" },
        new MsgModel{ MsgCode = "20660", MsgContent = "該儲位的倉別不可為自動倉" },
        new MsgModel{ MsgCode = "20661", MsgContent = "序號綁儲位商品數量不能超過1" },
        #endregion

        #region 08 序號作業
        new MsgModel{ MsgCode = "20851", MsgContent = "品號、序號擇一必填" },
        #endregion

        #region 搬移作業
        new MsgModel{ MsgCode = "20701", MsgContent = "搬移單建立成功" },
        new MsgModel{ MsgCode = "20751", MsgContent = "取得查詢儲位資料失敗" },
        new MsgModel{ MsgCode = "20752", MsgContent = "取得查詢商品儲位資料失敗" },
        new MsgModel{ MsgCode = "20753", MsgContent = "搬移數量應大於0" },
        new MsgModel{ MsgCode = "20754", MsgContent = "庫存數量{0}少於搬移數量{1}，搬移失敗" },
        new MsgModel{ MsgCode = "20755", MsgContent = "儲位{0}不存在，搬移失敗" },
        new MsgModel{ MsgCode = "20756", MsgContent = "搬移單建立失敗，商品未被搬移，原因為{0}" },
        new MsgModel{ MsgCode = "20757", MsgContent = "此為自動倉儲位，請刷入品號" },
        new MsgModel{ MsgCode = "20758", MsgContent = "上架儲位{0}尚未設定儲區，不可以上架" },
        new MsgModel{ MsgCode = "20759", MsgContent = "儲位{0}尚未設定儲區" },
        new MsgModel{ MsgCode = "20760", MsgContent = "來源儲位{0}已凍結" },
        #endregion

        #region 10 跨庫調撥作業
        new MsgModel{ MsgCode = "21001", MsgContent = "容器編號不得為空" },
        new MsgModel{ MsgCode = "21002", MsgContent = "容器編號不存在" },
        new MsgModel{ MsgCode = "21003", MsgContent = "容器內並無商品存在" },
        new MsgModel{ MsgCode = "21004", MsgContent = "該容器沒有進倉單" },
        new MsgModel{ MsgCode = "21005", MsgContent = "進倉單狀態有誤！{0}" },
        new MsgModel{ MsgCode = "21006", MsgContent = "品號條碼/序號不得為空" },
        new MsgModel{ MsgCode = "21007", MsgContent = "該條碼不存在品號/進倉序號" },
        new MsgModel{ MsgCode = "21008", MsgContent = "此商品不屬於此容器" },
        new MsgModel{ MsgCode = "21009", MsgContent = "找不到跨庫調撥進貨倉別編號" },
        new MsgModel{ MsgCode = "21010", MsgContent = "找不到跨庫調撥進貨倉別名稱" },
        new MsgModel{ MsgCode = "21011", MsgContent = "商品序號檢核有誤！{0}" },
        new MsgModel{ MsgCode = "21012", MsgContent = "此序號不屬於此容器" },
        new MsgModel{ MsgCode = "21013", MsgContent = "該容器已驗收過" },
        new MsgModel{ MsgCode = "21014", MsgContent = "儲位編號不得為空" },
        new MsgModel{ MsgCode = "21015", MsgContent = "此儲位並不允許進行跨庫調撥上架，請輸入正確儲位" },
        new MsgModel{ MsgCode = "21016", MsgContent = "儲位編號不存在" },
        new MsgModel{ MsgCode = "21017", MsgContent = "此儲位已經有其他貨主的商品，請尋找其他儲位" },
        new MsgModel{ MsgCode = "21018", MsgContent = "此儲位被凍結，不允許進貨上架" },
        new MsgModel{ MsgCode = "21019", MsgContent = "此容器已經全數上架完成，無法查詢" },
        new MsgModel{ MsgCode = "21020", MsgContent = "此容器尚未驗收，請先進行進倉驗收" },
        new MsgModel{ MsgCode = "21021", MsgContent = "批次上架失敗，原因為{0}" },
        new MsgModel{ MsgCode = "21022", MsgContent = "新建調撥單失敗，原因為{0}" },
        new MsgModel{ MsgCode = "21023", MsgContent = "此跨庫進貨容器{0}系統正在處理中"},
        new MsgModel{ MsgCode = "21024", MsgContent = "上架倉別不存在"},
        #endregion

        #region 11 集貨出入場
        new MsgModel{ MsgCode = "21101", MsgContent = "集貨場不得為空" },
        new MsgModel{ MsgCode = "21102", MsgContent = "此容器無對應的單據號碼" },
        new MsgModel{ MsgCode = "21103", MsgContent = "找不到出貨批次明細檔" },
        new MsgModel{ MsgCode = "21104", MsgContent = "此容器應到集貨場{0}集貨" },
        new MsgModel{ MsgCode = "21105", MsgContent = "此容器不需進集貨場，請到{0}" },
        new MsgModel{ MsgCode = "21106", MsgContent = "此出貨單都已到齊，不可再進場" },
        new MsgModel{ MsgCode = "21107", MsgContent = "集貨場{0}儲格類型{1}已滿" },
        new MsgModel{ MsgCode = "21108", MsgContent = "集貨場編號不存在" },
        new MsgModel{ MsgCode = "21109", MsgContent = "請將商品放到第一箱中後，第二箱請回收" },
        new MsgModel{ MsgCode = "21110", MsgContent = "請將容器放到儲位上，並刷入儲格條碼" },
        new MsgModel{ MsgCode = "21111", MsgContent = "找不到集貨格料架" },
        new MsgModel{ MsgCode = "21112", MsgContent = "儲格條碼不得為空" },
        new MsgModel{ MsgCode = "21113", MsgContent = "刷入的儲格條碼有誤" },
        new MsgModel{ MsgCode = "21114", MsgContent = "尚無可出場的容器" },
        new MsgModel{ MsgCode = "21115", MsgContent = "此容器已放入儲格" },
        new MsgModel{ MsgCode = "21116", MsgContent = "容器不正確，不可出場" },
        new MsgModel{ MsgCode = "21117", MsgContent = "此容器{0}綁定多張單{1}，不可進場" },
        new MsgModel{ MsgCode = "21118", MsgContent = "此容器{0}綁定單號{1}，與集貨場容器綁定單號{2}不同，不可進場" },
        new MsgModel{ MsgCode = "21119", MsgContent = "此容器{0}已綁定集貨場為{1}，與人員選取的集貨場{2}不同，不可進場" },
        new MsgModel{ MsgCode = "21120", MsgContent = "此單號{0}第一箱容器{1}未完成集貨進場確認，請先將第一箱容器進場確認後再進場其他容器" },
        new MsgModel{ MsgCode = "21121", MsgContent = "此容器{0}綁定的單號{1}未完成預約集貨格，不可集貨進場確認，請重新執行集貨進場" },
        new MsgModel{ MsgCode = "21122", MsgContent = "此第一箱容器{0}綁定多張單{1}，不可進場" },
        new MsgModel{ MsgCode = "21123", MsgContent = "此容器系統正在處理中，請稍後再試" },

        #endregion

        #region 12 進貨收發
        new MsgModel{ MsgCode = "21201", MsgContent = "進貨單號/貨主單號不得為空" },
        new MsgModel{ MsgCode = "21202", MsgContent = "無進倉單資料" },
        new MsgModel{ MsgCode = "21203", MsgContent = "跨併的進貨單不須做進貨收發作業" },
        new MsgModel{ MsgCode = "21204", MsgContent = "該進倉單{0}狀態為{1}，無法進行收貨" },
        new MsgModel{ MsgCode = "21205", MsgContent = "進貨單號不得為空" },
        new MsgModel{ MsgCode = "21206", MsgContent = "尚未設定進倉作業與影資系統整合參數" },
        new MsgModel{ MsgCode = "21207", MsgContent = "需要有工作站編號和影資系統綁定，請提供工作站編號" },
        new MsgModel{ MsgCode = "21208", MsgContent = "工作站編號 {0} 未設定在收貨區，必須為G開頭或通知資訊管理人員調整" },
        new MsgModel{ MsgCode = "21209", MsgContent = "工作站編號錯誤，必須輸入4碼" },
        new MsgModel{ MsgCode = "21210", MsgContent = "工作站編號 {0} 未設定，請通知資訊管理人員新增工作站編號" },
        #endregion

        #region 13 商品複驗
        new MsgModel{ MsgCode = "21301", MsgContent = "並非使用中容器" },
        new MsgModel{ MsgCode = "21302", MsgContent = "容器已複驗/不須複驗" },
        new MsgModel{ MsgCode = "21303", MsgContent = "此容器尚未關箱" },
        new MsgModel{ MsgCode = "21304", MsgContent = "此為不良品序號，商品不可複驗" },
        new MsgModel{ MsgCode = "21305", MsgContent = "此容器複驗失敗，請至 [複驗異常處理功能]，進行後續的作業" },
        new MsgModel{ MsgCode = "21306", MsgContent = "驗收容器資料對應的資料不得為空" },
        new MsgModel{ MsgCode = "21307", MsgContent = "LMS回覆:{0}" },
        new MsgModel{ MsgCode = "21308", MsgContent = "LMS回覆的品號不正確" },
        new MsgModel{ MsgCode = "21309", MsgContent = "找不到進倉單資料" },
        new MsgModel{ MsgCode = "21310", MsgContent = "條碼不正確" },
        new MsgModel{ MsgCode = "21311", MsgContent = "複驗不通過原因不得為空" },
        new MsgModel{ MsgCode = "21312", MsgContent = "新容器條碼不得為空" },
        new MsgModel{ MsgCode = "21313", MsgContent = "新容器已被使用，請更換其他容器" },
        new MsgModel{ MsgCode = "21314", MsgContent = "原本的容器是混和型容器，並不允許使用" },
        new MsgModel{ MsgCode = "21315", MsgContent = "容器頭檔資料有誤" },
        new MsgModel{ MsgCode = "21316", MsgContent = "容器身檔資料有誤" },
        new MsgModel{ MsgCode = "21317", MsgContent = "容器與驗收紀錄商品對應有誤" },
        new MsgModel{ MsgCode = "21318", MsgContent = "舊容器中的商品不存在" },
        new MsgModel{ MsgCode = "21319", MsgContent = "舊容器不存在" },
        new MsgModel{ MsgCode = "21320", MsgContent = "請勿使用料盒式(有分格)容器" },
        new MsgModel{ MsgCode = "21321", MsgContent = "此容器已完成複驗，正在移動中，不可重新複驗" },
        new MsgModel{ MsgCode = "21322", MsgContent = "此容器已完成複驗，也到達上架的工作站，不可重新複驗" },
        new MsgModel{ MsgCode = "21323", MsgContent = "該容器已關箱待複驗，但卻沒有商品資料" },
        new MsgModel{ MsgCode = "21324", MsgContent = "請勿使用料盒式容器" },
        new MsgModel{ MsgCode = "21325", MsgContent = "非待上架容器，請確認條碼" },
        new MsgModel{ MsgCode = "21326", MsgContent = "此容器尚未關箱" },
        new MsgModel{ MsgCode = "21327", MsgContent = "商品未完成檢驗通過，尚未複驗不可上架" },
        new MsgModel{ MsgCode = "21328", MsgContent = "此容器已移動到上架的工作站，不可再被移動" },
        new MsgModel{ MsgCode = "21329", MsgContent = "此驗收容器{0}系統正在處理中，請稍後再試" },
        new MsgModel{ MsgCode = "21330", MsgContent = "複驗通過處理有誤" },
        new MsgModel{ MsgCode = "21331", MsgContent = "複驗通過處理有誤" },
        new MsgModel{ MsgCode = "21332", MsgContent = "此容器已取消" },
        new MsgModel{ MsgCode = "21333", MsgContent = "此容器已完成複驗" },
        new MsgModel{ MsgCode = "21334", MsgContent = "查無驗收單資料" },
        #endregion

        #region 14 容器查詢
        new MsgModel{ MsgCode = "21401", MsgContent = "此為混和型容器，暫不提供查詢" },
        new MsgModel{ MsgCode = "21402", MsgContent = "該容器類型有誤" },
        new MsgModel{ MsgCode = "21403", MsgContent = "此容器條碼等待單據綁定中，請稍後再查詢" },
        new MsgModel{ MsgCode = "21404", MsgContent = "此為特殊結構容器，暫不提供查詢" },
        
        
        #endregion

        #region 16 廠退便利倉
        new MsgModel{ MsgCode = "21601", MsgContent = "便利倉編號不得為空" },
        new MsgModel{ MsgCode = "21602", MsgContent = "廠退出貨單號不得為空" },
        new MsgModel{ MsgCode = "21603", MsgContent = "廠退出貨單號不存在" },
        new MsgModel{ MsgCode = "21604", MsgContent = "此廠退出貨單尚未配庫" },
        new MsgModel{ MsgCode = "21605", MsgContent = "此廠退出貨單尚未完成包裝，不可入場" },
        new MsgModel{ MsgCode = "21606", MsgContent = "此廠退出貨單已出貨，不可入場" },
        new MsgModel{ MsgCode = "21607", MsgContent = "此廠退出貨單已取消，不可入場" },
        new MsgModel{ MsgCode = "21608", MsgContent = "此廠退出貨單{0}，不可入場" },
        new MsgModel{ MsgCode = "21609", MsgContent = "此廠退出貨單已入場廠退便利倉，不可入場" },
        new MsgModel{ MsgCode = "21610", MsgContent = "空儲格不足，無法入場" },
        new MsgModel{ MsgCode = "21611", MsgContent = "廠商編號不得為空" },
        new MsgModel{ MsgCode = "21612", MsgContent = "便利倉儲格編號不得為空" },
        new MsgModel{ MsgCode = "21613", MsgContent = "便利倉儲格編號不存在" },
        new MsgModel{ MsgCode = "21614", MsgContent = "便利倉儲格已被 {0} 廠商使用，不可放入" },
        new MsgModel{ MsgCode = "21615", MsgContent = "無此廠商 {0} 便利倉入場資料" },
        new MsgModel{ MsgCode = "21616", MsgContent = "廠商編號不存在" },
        new MsgModel{ MsgCode = "21617", MsgContent = "儲格編號不得為空" },
        new MsgModel{ MsgCode = "21618", MsgContent = "此儲格已無入場資料，系統已釋放此儲格" },
        new MsgModel{ MsgCode = "21619", MsgContent = "此儲格無此廠退出貨單號" },
        #endregion

        #region 17 紙箱補貨
        new MsgModel{ MsgCode = "21701",MsgContent = "此任務已被{0}接收任務"},
        new MsgModel{ MsgCode = "21702",MsgContent = "查無任務"},
        #endregion

        #region 18 跨庫調撥驗收入自動倉
        new MsgModel{ MsgCode = "21801", MsgContent = "請先設定跨庫調撥驗收的上架倉別清單"},
        new MsgModel{ MsgCode = "21802", MsgContent = "請先設定跨庫調撥驗收的上架倉別清單"},
        #endregion 18 跨庫調撥驗收入自動倉
      };
    }

    /// <summary>
    /// 帳號檢核
    /// </summary>
    /// <param name="empID">帳號</param>
    /// <returns></returns>
    public IQueryable<F1924> CheckAcc(string empID)
    {
      var f1924Repository = new F1924Repository(Schemas.CoreSchema);
      return f1924Repository.CheckAcc(empID);
    }

    /// <summary>
    /// 檢核人員功能權限
    /// </summary>
    /// <param name="funcNo">功能編號</param>
    /// <param name="accNo">帳號</param>
    /// <returns></returns>
    public int CheckAccFunction(string funcNo, string accNo)
    {
      var f192401Repository = new F192401Repository(Schemas.CoreSchema);
      return f192401Repository.CheckAccFunction(funcNo, accNo);
    }

    /// <summary>
    /// 檢核人員貨主權限
    /// </summary>
    /// <param name="custCode">貨主編號</param>
    /// <param name="empId">帳號</param>
    /// <returns></returns>
    public int CheckAccCustCode(string custCode, string empId)
    {
      var f192402Repository = new F192402Repository(Schemas.CoreSchema);
      return f192402Repository.CheckAccCustCode(custCode, empId);
    }

    /// <summary>
    /// 檢核人員物流中心權限
    /// </summary>
    /// <param name="dcCode">物流中心編號</param>
    /// <param name="empId">帳號</param>
    /// <returns></returns>
    public int CheckAccDc(string dcCode, string empId)
    {
      var f192402Repository = new F192402Repository(Schemas.CoreSchema);
      return f192402Repository.CheckAccDc(dcCode, empId);
    }

    /// <summary>
    /// 取得業主編號
    /// </summary>
    /// <param name="custCode">貨主編號</param>
    /// <returns></returns>
    public string GetGupCode(string custCode)
    {
      var f1909Repository = new F1909Repository(Schemas.CoreSchema);
      return f1909Repository.GetGupCode(custCode);
    }

    /// <summary>
    /// 檢核帳號是否已登入在其他裝置
    /// </summary>
    /// <param name="accNo">帳號</param>
    /// <returns></returns>
    public bool CheckLoginLog(string accNo, string mcCode)
    {
      var f0070Repository = new F0070Repository(Schemas.CoreSchema);
      return f0070Repository.CheckLoginLog(accNo, mcCode);
    }

    /// <summary>
    /// 取得人員名稱
    /// </summary>
    /// <param name="empId">帳號</param>
    /// <returns></returns>
    public string GetEmpName(string empID)
    {
      var f1924Repository = new F1924Repository(Schemas.CoreSchema);
      return f1924Repository.GetEmpName(empID);
    }

    /// <summary>
    /// 取得調撥路線編號
    /// </summary>
    /// <param name="detailList"></param>
    /// <returns></returns>
    public List<GetRouteListRes> GetRouteList(List<GetRouteListReq> detailList)
    {
      List<GetRouteListRes> result = new List<GetRouteListRes>();

      detailList = detailList.OrderBy(x => x.LocCode).ToList();

      for (int i = 0; i < detailList.Count; i++)
      {
        var currData = detailList[i];

        result.Add(new GetRouteListRes
        {
          Route = (i + 1),
          No = currData.No,
          Seq = currData.Seq,
          LocCode = currData.LocCode
        });
      }

      return result;
    }

    /// <summary>
    /// 取得商品序號清單
    /// </summary>
    /// <param name="dcCode">物流中心編號</param>
    /// <param name="custCode">貨主編號</param>
    /// <param name="gupCode">業主編號</param>
    /// <param name="itemCode">商品編號</param>
    /// <param name="rtNo">驗收單號</param>
    /// <param name="wmsNo">系統單號</param>
    /// <returns></returns>
    public IQueryable<GetAllocItemSn> GetSnList(string dcCode, string custCode, string gupCode, List<string> itemCode, List<string> rtNo = null, List<string> snList = null)
    {
      var f2501Repository = new F2501Repository(Schemas.CoreSchema);
      var f02020104Repository = new F02020104Repository(Schemas.CoreSchema);
      if (rtNo != null)
      {
        // 取得進倉序號清單
        var getSnlistFromF02020104 = f02020104Repository.GetSnList(dcCode, custCode, gupCode, rtNo);
        // 取得商品序號清單
        return f2501Repository.GetSnList(dcCode, gupCode, custCode, getSnlistFromF02020104.ToList());
      }
      else
      {
        return f2501Repository.GetSnList(gupCode, custCode, itemCode, snList);
      }

    }

    /// <summary>
    /// 取得訊息內容
    /// </summary>
    /// <param name="msgCode">訊息代碼</param>
    /// <returns></returns>
    public string GetMsg(string msgCode)
    {
      List<MsgModel> msgData = GetMsgData();
      MsgModel data = msgData.Where(x => x.MsgCode.Equals(msgCode)).SingleOrDefault();
      return data != null ? data.MsgContent : string.Empty;
    }

    /// <summary>
    /// 檢核裝置驗證碼
    /// </summary>
    /// <param name="devCode"></param>
    /// <returns></returns>
    public DbKeyEnum? CheckDevCode(string devCode)
    {
      DbKeyEnum? res = null;
      int number;
      if (devCode != null)
      {
        if (!string.IsNullOrWhiteSpace(devCode) && devCode.Length == 10)
        {
          // decode值前3碼
          string firstDecode = devCode.Substring(0, 3);

          // decode值後7碼
          string lastDecode = devCode.Substring(3, 7);

          // 檢核前3碼是否為數字，若非數值則回傳null
          // 檢核後7碼是否為A-Z,a-z,0-9，如不是則回傳null
          if (int.TryParse(firstDecode, out number) && new Regex(@"^.[A-Za-z0-9]+$").Match(lastDecode).Success)
          {
            // decode值前3碼將每一個字元透由ASCII轉換數值並累加後值+後7碼數值 MOD 174 取餘數值
            int key = (Convert.ToInt32(firstDecode) + Encoding.ASCII.GetBytes(lastDecode).Sum(x => x)) % 174;
            //int key = (Encoding.ASCII.GetBytes(lastDecode).Sum(x => x) + Convert.ToInt32(firstDecode)) % 174;

            // 找出符合key的Enum
            var dbKeyEnums = System.Enum.GetValues(typeof(DbKeyEnum)).Cast<DbKeyEnum>();
            var currKeyEnum = dbKeyEnums.Where(x => (int)x == key);
            res = currKeyEnum.Count() > 0 ? currKeyEnum.FirstOrDefault() : default(DbKeyEnum?);
          }
        }
      }

      return res;
    }

    /// <summary>
    /// 帳號密碼檢核
    /// </summary>
    /// <param name="accNo"></param>
    /// <param name="pwd"></param>
    /// <returns></returns>
    public GetValidateUser ValidateUser(string accNo)
    {
      var f1952Repository = new F1952Repository(Schemas.CoreSchema);
      return f1952Repository.ValidateUser(accNo);
    }

    /// 檢核是否登入者有權限
    /// </summary>
    /// <param name="empId">作業人員帳號</param>
    /// <param name="dcCode">物流中心編號</param>
    /// <param name="locCode">實際儲位</param>
    /// <returns></returns>
    public bool CheckActLoc(string empId, string dcCode, string locCode)
    {
      var f192403Repository = new F192403Repository(Schemas.CoreSchema);
      return f192403Repository.CheckActLoc(empId, dcCode, locCode);
    }
    /// <summary>
    /// 檢核儲位是否凍結
    /// </summary>
    /// <param name="dcCode">物流中心編號</param>
    /// <param name="locCode">實際儲位</param>
    /// <param name="allocType">單據類別</param>
    /// <returns></returns>
    public bool CheckLocFreeze(string dcCode, string locCode, string allocType)
    {
      var sharedService = new SharedService();
      return sharedService.CheckLocFreeze(dcCode, locCode, allocType).IsSuccessed;
    }

    /// <summary>
    /// 檢則商品是否混批
    /// </summary>
    /// <returns></returns>
    public bool CheckItemMixBatch(string dcCode, string gupCode, string custCode, string itemCode, string locCode, string validDate)
    {
      var sharedService = new SharedService();
      return sharedService.CheckItemMixBatch(dcCode, gupCode, custCode, itemCode, locCode, validDate);
    }
    /// <summary>
    /// 檢則商品是否混品
    /// </summary>
    /// <returns></returns>
    public bool CheckItemMixLoc(string dcCode, string gupCode, string custCode, string itemCode, string locCode)
    {
      var sharedService = new SharedService();
      return sharedService.CheckItemMixLoc(dcCode, gupCode, custCode, itemCode, locCode);
    }

    public void InsertLoginLog(string mcCode, string accNo, string devCode)
    {
      var f0070Repository = new F0070Repository(Schemas.CoreSchema);
      f0070Repository.InsertLoginLog(mcCode, accNo, devCode);
    }
    /// <summary>
    /// 取得功能權限
    /// </summary>
    /// <param name="funcNo">功能編號</param>
    /// <returns></returns>
    public string GetFunName(string funcNo)
    {
      var f1954Repository = new F1954Repository(Schemas.CoreSchema);
      return f1954Repository.GetFunName(funcNo).FirstOrDefault();


    }

    /// <summary>
    /// 檢核字串是否為數字
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool CheckIsNum(string value)
    {
      int n;
      return !string.IsNullOrWhiteSpace(value) ? int.TryParse(value, out n) : false;
    }

    /// <summary>
    /// 刪除登入紀錄
    /// </summary>
    /// <param name="accNo">帳號</param>
    /// <param name="devCode">裝置驗證碼</param>
    public void DeleteLoginLog(string accNo, string devCode, string mcCode = null)
    {
      F0070Repository f0070Repository = new F0070Repository(Schemas.CoreSchema);
      f0070Repository.DeleteLoginLog(devCode, mcCode, accNo);
    }

    /// <summary>
    /// 取得商品單位名稱
    /// </summary>
    /// <param name="unit">商品單位編號</param>
    /// <returns></returns>
    public string GetItemUnit(string unit)
    {
      F91000302Repository f91000302Repository = new F91000302Repository(Schemas.CoreSchema);
      return f91000302Repository.GetItemUnit(unit);
    }

    /// <summary>
    /// 取得狀態名稱
    /// </summary>
    /// <param name="topic">程式編號(資料表)</param>
    /// <param name="subTopic">選單ID</param>
    /// <param name="value">參數值</param>
    /// <returns></returns>
    public string GetTopicValueName(string topic, string subTopic, string value)
    {
      F000904Repository f000904Repository = new F000904Repository(Schemas.CoreSchema);
      return f000904Repository.GetTopicValueName(topic, subTopic, value);
    }

    /// <summary>
    /// 取得狀態名稱
    /// </summary>
    /// <param name="topic">程式編號(資料表)</param>
    /// <param name="subTopic">選單ID</param>
    /// <param name="value">參數值</param>
    /// <returns></returns>
    public string GetTopicValueNameByVW(string topic, string subTopic, string value)
    {
      F000904Repository f000904Repository = new F000904Repository(Schemas.CoreSchema);
      return f000904Repository.GetTopicValueNameByVW(topic, subTopic, value);
    }

    public string GetWhName(string dcCode, string warehouseId)
    {
      F1980Repository f1980Repository = new F1980Repository(Schemas.CoreSchema);
      return f1980Repository.GetWhName(dcCode, warehouseId);
    }

    /// <summary>
    /// 商品溫層與倉別溫層對照表
    ///  商品溫度              倉別溫層
    ///  02(恆溫),03(冷藏) =>  02(低溫)
    ///  01(常溫)          =>  01(常溫)
    ///  04(冷凍)          =>  03(冷凍) 
    /// </summary>
    /// <param name="itemTmpr"></param>
    /// <returns></returns>
    public string GetWareHouseTmprByItemTmpr(string itemTmpr)
    {
      var sharedService = new SharedService();
      return sharedService.GetWareHouseTmprByItemTmpr(itemTmpr);
    }

    /// <summary>
    /// 更新來源單號狀態
    /// </summary>
    /// <param name="sourceType">來源單據類型</param>
    /// <param name="dcCode">物流中心</param>
    /// <param name="gupCode">業主</param>
    /// <param name="custCode">貨主</param>
    /// <param name="wmsNo">各類單據單號(非來源單號)</param>
    /// <param name="wmsNoStatus">單據單號狀態(非來源單號)</param>
    public ExecuteResult UpdateSourceNoStatus(SourceType sourceType, string dcCode, string gupCode, string custCode, string wmsNo, string wmsNoStatus)
    {
      var sharedService = new SharedService(_wmsTransation);
      return sharedService.UpdateSourceNoStatus(sourceType, dcCode, gupCode, custCode, wmsNo, wmsNoStatus);
    }

    /// <summary>
    /// 清除 已拆開序號的箱號/盒號/儲值卡盒號
    /// </summary>
    /// <param name="dcCode">物流中心</param>
    /// <param name="gupCode">業主</param>
    /// <param name="custCode">貨主</param>
    /// <param name="wmsNo">各類單據單號(非來源單號)</param>
    /// <param name="type">下架:傳TD，上架傳TU</param>
    public void ClearSerialByBoxOrCaseNo(string dcCode, string gupCode, string custCode, string wmsNo, string type)
    {
      var serialNoService = new SerialNoService();
      serialNoService.ClearSerialByBoxOrCaseNo(dcCode, gupCode, custCode, wmsNo, type);
    }

    /// <summary>
    /// 填入登入者帳號、姓名
    /// </summary>
    /// <param name="accNo"></param>
    public void SetDefaulfStaff(StaffModel staff)
    {
      if (!string.IsNullOrWhiteSpace(staff.AccNo) && Current.DefaultStaff != staff.AccNo)
      {
        Current.DefaultStaff = staff.AccNo;
        Current.DefaultStaffName = GetEmpName(staff.AccNo);
      }
    }

    /// <summary>
    /// 更新要揀貨的儲位已使用容積量
    /// </summary>
    /// <param name="dcCode">物流中心</param>
    /// <param name="gupCode">業主</param>
    /// <param name="custCode">貨主</param>
    /// <param name="pickOrdNos">揀貨單號</param>
    public void UpdatePickOrdNoLocVolumn(string dcCode, string gupCode, string custCode, List<string> pickOrdNos)
    {
      var sharedService = new SharedService();
      sharedService.UpdatePickOrdNoLocVolumn(dcCode, gupCode, custCode, pickOrdNos);
    }

    /// <summary>
    /// 取得商品名稱
    /// </summary>
    /// <param name="gupCode">業主編號</param>
    /// <param name="custCode">貨主編號</param>
    /// <param name="itemCode">品號</param>
    /// <returns></returns>
    public string GetItemName(string gupCode, string custCode, string itemCode)
    {
      F1903Repository f1903Repository = new F1903Repository(Schemas.CoreSchema);
      return f1903Repository.GetItemName(gupCode, custCode, itemCode);
    }

    /// <summary>
    /// 檢核儲位權限
    /// </summary>
    /// <param name="dcCode">物流中心編號</param>
    /// <param name="accNo"></param>
    /// <param name="custCode"></param>
    /// <param name="locCode">貨主編號</param>
    /// <param name="loctype">loctype</param>
    /// <returns></returns>
    public ApiResult CheckLocCode(string dcCode, string accNo, string custCode, string locCode, string loctype)
    {
      ApiResult apiResult = new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent = GetMsg("10001") };
      F1912Repository f1912Repo = new F1912Repository(Schemas.CoreSchema);
      F192403Repository f192403Repo = new F192403Repository(Schemas.CoreSchema);
      // 檢核儲位是否存在
      var f1912 = f1912Repo.CheckLocExist(dcCode);
      if (apiResult.IsSuccessed && !f1912)
      {
        apiResult = new ApiResult { IsSuccessed = false, MsgCode = "20053", MsgContent = GetMsg("20053") };
      }

      // 檢核儲位是否有登入者有權限
      var f192403 = f192403Repo.CheckActLoc(accNo, dcCode, locCode);
      if (apiResult.IsSuccessed && !f192403)
      {
        apiResult = new ApiResult { IsSuccessed = false, MsgCode = "20055", MsgContent = GetMsg("20055") };
      }

      // 檢核來源儲位是否凍結 若loctype=1 (來源倉)，若loctype=2 (目的倉)
      SharedService sharedService = new SharedService();
      var checkLocFreeze = sharedService.CheckLocFreeze(dcCode, locCode, loctype);
      if (loctype == "1")
      {
        if (apiResult.IsSuccessed && !checkLocFreeze.IsSuccessed)
        {
          apiResult = new ApiResult { IsSuccessed = false, MsgCode = "20062", MsgContent = checkLocFreeze.Message };
        }
      }
      else if (loctype == "2")
      {
        if (apiResult.IsSuccessed && !checkLocFreeze.IsSuccessed)
        {
          apiResult = new ApiResult { IsSuccessed = false, MsgCode = "20062", MsgContent = checkLocFreeze.Message };
        }
      }

      // 檢核儲位是否非此貨主儲位
      var checkCustCodeLoc = f1912Repo.CheckCustCodeLoc(dcCode, locCode);
      if (apiResult.IsSuccessed && (checkCustCodeLoc.CUST_CODE != custCode && checkCustCodeLoc.CUST_CODE != "0"))
      {
        apiResult = new ApiResult { IsSuccessed = false, MsgCode = "20056", MsgContent = GetMsg("20056") };
      }
      else
      {
        if (apiResult.IsSuccessed && (checkCustCodeLoc.NOW_CUST_CODE != custCode && checkCustCodeLoc.NOW_CUST_CODE != "0"))
        {
          apiResult = new ApiResult { IsSuccessed = false, MsgCode = "20056", MsgContent = GetMsg("20056") };
        }
      }

      return apiResult;
    }

    /// <summary>
    /// 檢核儲位的溫層
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="itemCode"></param>
    /// <param name="custCode"></param>
    /// <param name="locCode"></param>
    /// <param name="gupCode"></param>
    /// <returns></returns>
    public ApiResult CheckLocTmpr(string dcCode, string itemCode, string custCode, string locCode, string gupCode)
    {
      P81Service p81Service = new P81Service();
      F1903Repository f1903Repo = new F1903Repository(Schemas.CoreSchema);
      F1912Repository f1912Repo = new F1912Repository(Schemas.CoreSchema);
      SharedService sharedService = new SharedService();
      ApiResult apiResult = new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent = p81Service.GetMsg("10001") };


      // 檢查商品溫層
      var getF1903Tmpr = f1903Repo.GetF1903Tmpr(itemCode, custCode, gupCode);
      if (apiResult.IsSuccessed && getF1903Tmpr == null)
      {
        apiResult = new ApiResult { IsSuccessed = false, MsgCode = "20072", MsgContent = string.Format(GetMsg("20072"), locCode) };
      }

      // 檢查儲位溫層
      var getF1912Tmpr = f1912Repo.GetF1912Tmpr(dcCode, locCode);
      if (apiResult.IsSuccessed && getF1912Tmpr == null)
      {
        apiResult = new ApiResult { IsSuccessed = false, MsgCode = "20073", MsgContent = string.Format(GetMsg("20073"), itemCode) };
      }

      if (apiResult.IsSuccessed)
      {
        string newTmpr = p81Service.GetWareHouseTmprByItemTmpr(getF1903Tmpr.TmprType);

        // 比較儲位溫層 = 商品溫層轉儲位溫層 如果不相同則回傳 & 取得訊息內容[20071](儲位, 儲位溫層名稱, 品號, 商品溫層名稱)
        if (!newTmpr.Split(',').Contains(getF1912Tmpr.TmprType))
        {
          //"儲位{0}溫層{1}不符合商品{2}溫層{3}"
          apiResult = new ApiResult
          {
            IsSuccessed = false,
            MsgCode = "20071",
            MsgContent = string.Format(p81Service.GetMsg("20071"),
                  locCode, getF1912Tmpr.TmprTypeName, getF1903Tmpr.TmprTypeName, itemCode)
          };
        }
      }

      return apiResult;
    }

    /// <summary>
    /// 檢查商品混批混品
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name=""></param>
    /// <returns></returns>
    public ApiResult CheckLocHasItem(string dcCode, string gupCode, string custCode, string itemCode, string locCode, string valiDate)
    {
      P81Service p81Service = new P81Service();
      SharedService sharedService = new SharedService();
      ApiResult apiResult = new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent = p81Service.GetMsg("10001") };

      if (!sharedService.CheckItemMixBatch(dcCode, gupCode, custCode, itemCode, locCode, valiDate))
      {
        apiResult = new ApiResult { IsSuccessed = false, MsgCode = "20057", MsgContent = p81Service.GetMsg("20057") };
      }

      if (apiResult.IsSuccessed)
      {
        if (!sharedService.CheckItemMixLoc(dcCode, gupCode, custCode, itemCode, locCode))
        {
          apiResult = new ApiResult { IsSuccessed = false, MsgCode = "20058", MsgContent = p81Service.GetMsg("20058") };
        }
      }

      return apiResult;
    }

    // 取得商品品號
    public F1903 GetItemCode(string custCode, string itemCode)
    {
      var f1903Repository = new F1903Repository(Schemas.CoreSchema);
      return f1903Repository.GetItemCode(custCode, itemCode);
    }

    /// <summary>
    /// 用容器取得單號
    /// </summary>
    /// <param name="containerCode"></param>
    /// <returns></returns>
    public List<GetWmsNoByContainerCodeRes> GetWmsNoByContainerCode(string containerCode)
    {
      var result = new List<GetWmsNoByContainerCodeRes>();
      var f0701Repo = new F0701Repository(Schemas.CoreSchema);
      var f0701 = f0701Repo.GetDatasByTrueAndCondition(o => o.CONTAINER_CODE == containerCode && o.CONTAINER_TYPE == "0").FirstOrDefault();

      //(1) 若F0701.container_type = 0，則
      //	撈 F070101.F0701_ID = F0701.ID、
      //	回傳 F070101.WMS_NO、GUP_CODE、CUST_CODE
      if (f0701 != null)
      {
        var f070101Repo = new F070101Repository(Schemas.CoreSchema);
        var f070101s = f070101Repo.GetDatasByF0701Ids(new List<long> { f0701.ID });
        if (f070101s.Any())
        {
          result.AddRange(f070101s.Select(x => new GetWmsNoByContainerCodeRes
          {
            GUP_CODE = x.GUP_CODE,
            CUST_CODE = x.CUST_CODE,
            WMS_NO = x.WMS_NO
          }).ToList());
        }
      }

      return result;
    }

    public List<string> GetItemCodeByBarcode(ref bool isSn, string dcCode, string gupCode, string custCode, string barcode)
    {
      var itemService = new ItemService();
      var f1903Repo = new F1903Repository(Schemas.CoreSchema);
      var f020302Repo = new F020302Repository(Schemas.CoreSchema);

      F2501 f2501 = null;
      var itemCodes = itemService.FindItems(gupCode, custCode, barcode, ref f2501);

      if (itemCodes.Any())
      {
        isSn = f2501 != null;
        return itemCodes;
      }

      var f020302 = f020302Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.SERIAL_NO == barcode).FirstOrDefault();
      if (f020302 != null)
      {
        isSn = true;
        return new List<string> { f020302.ITEM_CODE };
      }

      return null;
    }
  }
}
