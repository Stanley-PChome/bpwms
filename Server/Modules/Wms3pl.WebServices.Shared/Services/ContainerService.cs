using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Transactions;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ServiceEntites;

namespace Wms3pl.WebServices.Shared.Services
{
	public class ContainerService
	{
		private WmsTransaction _wmsTransaction;
		public ContainerService(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

    public List<ContainerExecuteResult> CreateContainer(List<CreateContainerParam> param)
    {
      //回傳值改容器、單號、f0701ID
      var result = new List<ContainerExecuteResult>();
      var f0701Repo = new F0701Repository(Schemas.CoreSchema, _wmsTransaction);
      var f070101Repo = new F070101Repository(Schemas.CoreSchema, _wmsTransaction);
      var f070102Repo = new F070102Repository(Schemas.CoreSchema, _wmsTransaction);
      var addF070102 = new List<F070102>();

      var addDatas = param.GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.WAREHOUSE_ID, x.CONTAINER_CODE, x.CONTAINER_TYPE, x.PICK_ORD_NO })
          .Select(x => new
          {
            x.Key.DC_CODE,
            x.Key.GUP_CODE,
            x.Key.CUST_CODE,
            x.Key.WAREHOUSE_ID,
            x.Key.CONTAINER_CODE,
            x.Key.CONTAINER_TYPE,
            x.Key.PICK_ORD_NO,
            F070101s = x.GroupBy(z => new { z.WMS_NO, z.WMS_TYPE })
              .Select(z => new
              {
                z.Key.WMS_NO,
                z.Key.WMS_TYPE,
                F070102s = z.Select(y => new
                {
                  y.ITEM_CODE,
                  y.VALID_DATE,
                  y.MAKE_NO,
                  y.QTY,
                  y.SERIAL_NO_LIST,
                  y.BIN_CODE
                }).ToList()
              }).ToList()
          }).ToList();

      addDatas.ForEach(data =>
      {
        var f0701Id = GetF0701NextId();

        f0701Repo.InsertF0701(f0701Id, data.DC_CODE, data.CUST_CODE, data.WAREHOUSE_ID, data.CONTAINER_CODE, data.CONTAINER_TYPE);

        data.F070101s.ForEach(f070101 =>
        {
          var f070101Id = GetF070101NextId();

          f070101Repo.InsertF070101(f070101Id, f0701Id, data.DC_CODE, data.GUP_CODE, data.CUST_CODE, data.CONTAINER_CODE, f070101.WMS_NO, f070101.WMS_TYPE, data.PICK_ORD_NO);

          result.Add(new ContainerExecuteResult()
          {
            f0701_ID = f0701Id,
            WMS_NO = f070101.WMS_NO,
            ContainerCode = data.CONTAINER_CODE,
            Qty = f070101.F070102s.Sum(o => o.QTY),
            WAREHOUSE_ID = data.WAREHOUSE_ID
          });

          f070101.F070102s.ForEach(f070102 =>
          {
            if (f070102.QTY > 0)
            {
              if (f070102 != null && f070102.SERIAL_NO_LIST != null && f070102.SERIAL_NO_LIST.Any())
                addF070102.AddRange(f070102.SERIAL_NO_LIST.Select(x => new F070102
                {
                  F070101_ID = f070101Id,
                  GUP_CODE = data.GUP_CODE,
                  CUST_CODE = data.CUST_CODE,
                  ITEM_CODE = f070102.ITEM_CODE,
                  VALID_DATE = f070102.VALID_DATE,
                  MAKE_NO = f070102.MAKE_NO,
                  QTY = 1,
                  BIN_CODE = f070102.BIN_CODE,
                  PICK_ORD_NO = data.PICK_ORD_NO,
                  ORG_F070101_ID = f070101Id,
                  SERIAL_NO = x,
                }));
              else
                addF070102.Add(new F070102
                {
                  F070101_ID = f070101Id,
                  GUP_CODE = data.GUP_CODE,
                  CUST_CODE = data.CUST_CODE,
                  ITEM_CODE = f070102.ITEM_CODE,
                  VALID_DATE = f070102.VALID_DATE,
                  MAKE_NO = f070102.MAKE_NO,
                  QTY = f070102.QTY,
                  BIN_CODE = f070102.BIN_CODE,
                  PICK_ORD_NO = data.PICK_ORD_NO,
                  ORG_F070101_ID = f070101Id,
                  //SERIAL_NO = f070102.SERIAL_NO,
                });
            }
          });
        });
      });

      if (addF070102 != null && addF070102.Any())
        f070102Repo.BulkInsert(addF070102);
      return result;
    }

		/// <summary>
		/// 容器釋放任務觸發
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsNo"></param>
		/// <param name="f0701_ID">當一張出貨單綁多個容器時需要刪除指定容器(如集貨進場)</param>
		/// <returns></returns>
		public ExecuteResult DelContainer(string dcCode, string gupCode, string custCode, string wmsNo, long? f0701ID = null)
		{
			var result = new ExecuteResult(true);
			
			var f0701Repo = new F0701Repository(Schemas.CoreSchema, _wmsTransaction);
			var f070101Repo = new F070101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f060302Repo = new F060302Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1980Repo = new F1980Repository(Schemas.CoreSchema, _wmsTransaction);

			var f070101s = f070101Repo.GetDatasByTrueAndCondition(o =>
			o.WMS_NO == wmsNo &&
			o.DC_CODE == dcCode &&
			o.GUP_CODE == gupCode &&
			o.CUST_CODE == custCode).ToList();

			if (f0701ID.HasValue)
				f070101s = f070101s.Where(x => x.F0701_ID == f0701ID.Value).ToList();

			if (f070101s.Any())
			{
				var f0701Ids = f070101s.Select(x => x.F0701_ID).Distinct().ToList();

				var f0701s = f0701Repo.GetDatasByF0701Ids(f0701Ids).ToList();

				var f060302s = f060302Repo.GetDatasByF07001s(f0701s, new List<string> { "0", "T" }).ToList();

				f0701Ids.ForEach(f0701Id =>
				{
					var f0701 = f0701s.Where(x => x.ID == f0701Id).FirstOrDefault();

					if (f0701 != null)
					{
						var currF060302s = f060302s.Where(x =>
						x.DC_CODE == f0701.DC_CODE &&
						x.CUST_CODE == f0701.CUST_CODE &&
						x.WAREHOUSE_ID == f0701.WAREHOUSE_ID &&
						x.CONTAINER_CODE == f0701.CONTAINER_CODE);

						// 檢核是否為自動倉
						if (!currF060302s.Any() && f1980Repo.CheckAutoWarehouse(f0701.DC_CODE, f0701.WAREHOUSE_ID))
						{
							f060302Repo.Add(new F060302
							{
								DC_CODE = f0701.DC_CODE,
								CUST_CODE = f0701.CUST_CODE,
								WAREHOUSE_ID = f0701.WAREHOUSE_ID,
								CONTAINER_CODE = f0701.CONTAINER_CODE,
								STATUS = "0"
							});
						}

						f0701Repo.DeleteF0701(f0701.ID);
					}
				});
			}

			return result;
		}

		public long GetF0701NextId()
		{
			var f0701Repo = new F0701Repository(Schemas.CoreSchema);
			return f0701Repo.GetF0701NextId();
		}

		public long GetF070101NextId()
		{
			var f070101Repo = new F070101Repository(Schemas.CoreSchema);
			return f070101Repo.GetF070101NextId();
		}

    public long GetF070104NextId()
    {
      var f070104Repo = new F070104Repository(Schemas.CoreSchema);
      var f070104 = f070104Repo.UseTransationScope(new TransactionScope(TransactionScopeOption.Required,
        new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }),
         () => {
           var lockF070104 = f070104Repo.LockF070104();
           var id = f070104Repo.GetF070104NextId();
           return new F070104
           {
             ID = id
           };
         });
      return f070104.ID;
    }

    public long GetF060207NextId()
		{
			var f060207Repo = new F060207Repository(Schemas.CoreSchema);
			var f060207 = f060207Repo.UseTransationScope(new TransactionScope(TransactionScopeOption.Required,
				new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted}),
				()=>{
					var lockF060207 = f060207Repo.LockF060207();
					var id = f060207Repo.GetF060207NextId();
					return new F060207
					{
						ID = id
					};
				});
			return f060207.ID;
		}

        /// <summary>
        /// 容器條碼共用服務
        /// </summary>
        /// <param name="containerCode"></param>
        /// <returns></returns>
        public ChkContainerResult CheckContainer(string containerCode)
        {
            // 檢核容器條碼是否為空
            if (string.IsNullOrWhiteSpace(containerCode))
                return new ChkContainerResult { IsSuccessed = false, Message = "容器條碼不得為空" };

            // 檢核容器條碼長度是否超過12
            if (containerCode.Length > 12)
                return new ChkContainerResult { IsSuccessed = false, Message = "容器條碼長度不可超過12碼" };

            // 檢核容器條碼第一碼是否為英文字母
            Regex reg1 = new Regex(@"^[A-Za-z]+$");
            if (!reg1.IsMatch(containerCode.Substring(0, 1)))
                return new ChkContainerResult { IsSuccessed = false, Message = "容器條碼必須第一碼為英文" };

            // 檢核容器條碼是否只有一個(-)
            if (containerCode.ToCharArray().Where(x => x == '-').Count() > 1)
                return new ChkContainerResult { IsSuccessed = false, Message = "容器分格符號(-)不可超過1個" };

            // 檢核是否為英數混和與分隔符號(-)
            Regex reg2 = new Regex(@"^[A-Za-z0-9-]+$");
            if (!reg2.IsMatch(containerCode))
                return new ChkContainerResult { IsSuccessed = false, Message = "容器條碼只允許英數混和與分格符號(-)" };

            if (containerCode.Contains("-"))
                return new ChkContainerResult { IsSuccessed = true, ContainerCode = containerCode.Substring(0, containerCode.LastIndexOf("-")).ToUpper(), BinCode = containerCode.ToUpper() };
            else
                return new ChkContainerResult { IsSuccessed = true, ContainerCode = containerCode.ToUpper() };
        }
	}
}
