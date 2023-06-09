using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.TransApiServices;
using Wms3pl.WebServices.Shared.TransApiServices.Check;

namespace Wms3pl.WebServices.FromWcsWebApi.Business.mssql.Services
{

    public class CommonAutoLocAbnormalNotifyService
    {
        #region 定義需檢核欄位、必填、型態、長度
        // 補貨超揀申請檢核設定
        private List<ApiCkeckColumnModel> receiptCheckColumnList = new List<ApiCkeckColumnModel>
        {
            new ApiCkeckColumnModel{  Name = "OwnerCode",   Type = typeof(string),   MaxLength = 12,  Nullable = false },
            new ApiCkeckColumnModel{  Name = "DcCode",      Type = typeof(string),   MaxLength = 10,  Nullable = false },
            new ApiCkeckColumnModel{  Name = "ZoneCode",    Type = typeof(string),   MaxLength = 5,   Nullable = false },
            new ApiCkeckColumnModel{  Name = "AbnormalType",Type = typeof(int),      MaxLength = 0,   Nullable = false },
            new ApiCkeckColumnModel{  Name = "ShelfCode",   Type = typeof(string),   MaxLength = 20,  Nullable = false },
            new ApiCkeckColumnModel{  Name = "BinCode",     Type = typeof(string),   MaxLength = 20,  Nullable = false },
            new ApiCkeckColumnModel{  Name = "OrderCode",   Type = typeof(string),   MaxLength = 32,  Nullable = true  },
            new ApiCkeckColumnModel{  Name = "SkuCode",     Type = typeof(string),   MaxLength = 20,  Nullable = false },
            new ApiCkeckColumnModel{  Name = "SkuQty",      Type = typeof(int),      MaxLength = 0,   Nullable = false },
            new ApiCkeckColumnModel{  Name = "Operator",    Type = typeof(string),   MaxLength = 20,  Nullable = false }
        };
        #endregion

        public ApiResult AutoLocAbnormalNotify(AutoLocAbnormalNotifyReq req)
        {
            ApiResult result = new ApiResult() { IsSuccessed = true };
            TransApiBaseService tacService = new TransApiBaseService();
            CheckTransWcsApiService ctaService = new CheckTransWcsApiService();
            CommonService commonService = new CommonService();
            ApiResult res = new ApiResult { IsSuccessed = true };
            WmsTransaction wmsTransation = new WmsTransaction();
            List<ApiResponse> data = new List<ApiResponse>();
            var f060801Repo = new F060801Repository(Schemas.CoreSchema, wmsTransation);

            #region 資料檢核
            // 檢核參數
            if (req == null)
                return new ApiResult { IsSuccessed = false, MsgCode = "20056", MsgContent = tacService.GetMsg("20056") };

            // 檢核物流中心 必填、是否存在
            ctaService.CheckDcCode(ref res, req);
            if (!res.IsSuccessed)
                return res;

            // 檢核貨主編號 必填、是否存在
            ctaService.CheckOwnerCode(ref res, req);
            if (!res.IsSuccessed)
                return res;

            ctaService.CheckZoneCode(ref res, req);
            if (!res.IsSuccessed)
                return res;

            // 共用欄位格式檢核
            data.AddRange((List<ApiResponse>)CheckColumnNotNullAndMaxLength(req).Data);

            result.IsSuccessed = !data.Any();
            result.Data = data;
            if (!result.IsSuccessed)
                return result;
            #endregion

                // 取得業主編號
            string gupCode = commonService.GetGupCode(req.OwnerCode);

            f060801Repo.Add(new F060801()
            {
                DC_CODE=req.DcCode,
                GUP_CODE=gupCode,
                CUST_CODE=req.OwnerCode,
                WAREHOUSE_ID=req.ZoneCode,
                ABNORMALTYPE=req.AbnormalType,
                SHELFCODE=req.ShelfCode,
                BINCODE=req.BinCode,
                ORDERCODE=req.OrderCode,
                SKUCODE=req.SkuCode,
                SKUQTY=req.SkuQty,
                OPERATOR=req.Operator
            });

            wmsTransation.Complete();
            return result;
        }

        protected ApiResult CheckColumnNotNullAndMaxLength(AutoLocAbnormalNotifyReq receipt)
        {
            TransApiBaseService tacService = new TransApiBaseService();
            ApiResult res = new ApiResult();
            List<ApiResponse> data = new List<ApiResponse>();

            string nullErrorMsg = tacService.GetMsg("20034");
            string formatErrorMsg = tacService.GetMsg("20035");

            #region 檢查盤點單欄位必填、最大長度
            List<string> notDateColumn = new List<string>();

            // 找出欄位不符合需必填、超過最大長度的欄位後，寫入各List
            receiptCheckColumnList.ForEach(column =>
            {
                var value = Convert.ToString(DataCheckHelper.GetRequireColumnValue(receipt, column.Name));

                // 必填
                if (!column.Nullable)
                    if (!DataCheckHelper.CheckRequireColumn(receipt, column.Name))
                        data.Add(new ApiResponse { ErrorColumn = column.Name, MsgCode = "20034", MsgContent = string.Format(nullErrorMsg, column.Name) });

                // 最大長度
                if (column.MaxLength > 0)
                    if (!DataCheckHelper.CheckDataMaxLength(receipt, column.Name, column.MaxLength))
                        data.Add(new ApiResponse { ErrorColumn = column.Name, MsgCode = "20035", MsgContent = string.Format(formatErrorMsg, column.Name) });
            });
            #endregion

            res.Data = data;

            return res;
        }

    }
}
