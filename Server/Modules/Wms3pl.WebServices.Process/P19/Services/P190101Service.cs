
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P19.Services
{
  public partial class P190101Service
  {
    private WmsTransaction _wmsTransaction;
    public P190101Service(WmsTransaction wmsTransaction = null)
    {
      _wmsTransaction = wmsTransaction;
    }

    public ExecuteResult InsertP190101(F1901 dc, List<F190101> dcService, string userId)
    {
      ExecuteResult result = new ExecuteResult() { IsSuccessed = true };
      var repoF1901 = new F1901Repository(Schemas.CoreSchema, _wmsTransaction);
      var repoF190101 = new F190101Repository(Schemas.CoreSchema, _wmsTransaction);
      

      // 0. 先檢查是否已存在
      if (repoF1901.Filter(x => x.DC_CODE == dc.DC_CODE).Any())
      {
        // 資料已存在
        result.IsSuccessed = false;
        result.Message = Properties.Resources.P190101Service_DataExist;
        return result;
      }
      // 1. 新增主檔
      repoF1901.Add(new F1901()
      {
          DC_CODE = dc.DC_CODE,
          DC_NAME = dc.DC_NAME,
          TEL = dc.TEL,
          FAX = dc.FAX,
          MAIL_BOX = dc.MAIL_BOX,
          BOSS = dc.BOSS,
          SHORT_CODE = dc.SHORT_CODE,
          BUILD_AREA = dc.BUILD_AREA,
          LAND_AREA = dc.LAND_AREA,
          ADDRESS = dc.ADDRESS
      });

      // 2. 新增F190101 - 寫入新資料
      foreach (var p in dcService)
          repoF190101.Add(new F190101() { DC_CODE = p.DC_CODE, CUST_CODE = p.CUST_CODE, GUP_CODE = p.GUP_CODE });
  
      repoF1901 = null;
      repoF190101 = null;
      return result;
    }

    public ExecuteResult InsertP190101(F1901 dc, List<F190101> dcService, List<F190904> F190904List, List<F050006> F050006List,List<F190102> F190102List ,string userId, string gupCode, string custCode)
    {
        ExecuteResult result = new ExecuteResult() { IsSuccessed = true };
        var repoF1901 = new F1901Repository(Schemas.CoreSchema, _wmsTransaction);
        var repoF190101 = new F190101Repository(Schemas.CoreSchema, _wmsTransaction);
        var repoF190904 = new F190904Repository(Schemas.CoreSchema, _wmsTransaction);
        var repoF050006 = new F050006Repository(Schemas.CoreSchema, _wmsTransaction);
        var repoF190102 = new F190102Repository(Schemas.CoreSchema, _wmsTransaction);
        var repoF0005 = new F0005Repository(Schemas.CoreSchema, _wmsTransaction);
        var repoF0003 = new F0003Repository(Schemas.CoreSchema, _wmsTransaction);
        var repoF000301 = new F000301Repository(Schemas.CoreSchema);
        var repoF190906 = new F190906Repository(Schemas.CoreSchema, _wmsTransaction);
				var repoF190907 = new F190907Repository(Schemas.CoreSchema, _wmsTransaction);
			  var repoF190105 = new F190105Repository(Schemas.CoreSchema,_wmsTransaction);

			// 0. 先檢查是否已存在
			if (repoF1901.Filter(x => x.DC_CODE == dc.DC_CODE).Any())
        {
            // 資料已存在
            result.IsSuccessed = false;
            result.Message = Properties.Resources.P190101Service_DC_Code_Duplicate;
            return result;
        }
        // 1. 新增主檔
        repoF1901.Add(new F1901()
        {
            DC_CODE = dc.DC_CODE,
            DC_NAME = dc.DC_NAME,
            TEL = dc.TEL,
            FAX = dc.FAX,
            MAIL_BOX = dc.MAIL_BOX,
            BOSS = dc.BOSS,
            SHORT_CODE = dc.SHORT_CODE,
            BUILD_AREA = dc.BUILD_AREA,
            LAND_AREA = dc.LAND_AREA,
            ADDRESS = dc.ADDRESS,
			ZIP_CODE = dc.ZIP_CODE
        });

			// 2. 新增F190101 - 寫入新資料

			if(dcService.Any())
				foreach (var p in dcService)
					repoF190101.Add(new F190101() { DC_CODE = p.DC_CODE, CUST_CODE = p.CUST_CODE, GUP_CODE = p.GUP_CODE });
			else
				repoF190101.Add(new F190101() { DC_CODE = dc.DC_CODE, CUST_CODE = custCode, GUP_CODE = gupCode });

			// 3. 新增F190904 - 寫入新資料
			foreach (var p in F190904List)
            repoF190904.Add(new F190904() { DC_CODE = p.DC_CODE, CUST_CODE = p.CUST_CODE, GUP_CODE = p.GUP_CODE, LOC_CODE = p.LOC_CODE });

			//4. 新增F050006 - 寫入新資料
			if (F050006List.Any())		//若有手動建立，則依照所建立資料的存入
			{
				repoF050006.BulkInsert(F050006List);
			}
			else  //預設 存入目前cust code或者依照勾選的服務客戶(cust code)存入
			{
				var saveDatas = new List<F050006>();
				var f050006Datas = repoF050006.GetDatasByTrueAndCondition(o => o.DC_CODE == "00" && o.GUP_CODE == "00" && o.CUST_CODE == "00");

				if (dcService.Any())
				{
					foreach (var i in dcService)
					{
						foreach (var data in f050006Datas)
						{
							var curr = new F050006();
							curr.DC_CODE = i.DC_CODE;
							curr.GUP_CODE = i.GUP_CODE;
							curr.CUST_CODE = i.CUST_CODE;
							curr.ZIP_CODE = data.ZIP_CODE;

							saveDatas.Add(curr);
						}
					}
				}
				else
				{
					foreach (var data in f050006Datas)
					{
						data.DC_CODE = dc.DC_CODE;
						data.GUP_CODE = gupCode;
						data.CUST_CODE = custCode;

						saveDatas.Add(data);
					}
				}

				repoF050006.BulkInsert(saveDatas);
			}
        //5. 新增F190102 - 寫入新資料
		if(F190102List.Any())
			foreach (var p in F190102List)
				repoF190102.Add(new F190102() { DC_CODE = p.DC_CODE, DELV_EFFIC = p.DELV_EFFIC, SORT = p.SORT });
		else
		{
			var f190102Datas = repoF190102.GetDatasByTrueAndCondition(o=>o.DC_CODE == "00").ToList();
			foreach (var data in f190102Datas)
			{
				data.DC_CODE = dc.DC_CODE;
				repoF190102.Add(data);
			}
		}

		//6.新增F0005 - 寫入新資料
		var f0005Datas = repoF0005.GetDatasByTrueAndCondition(o => o.DC_CODE == "00").ToList();
		if(f0005Datas.Any())
		{
			 foreach(var data in f0005Datas)
			 {
			     data.DC_CODE = dc.DC_CODE;
			     repoF0005.Add(data);
			 }
		}

		//7.新增F0003 - 寫入DC共用資料
    var dcDefault = repoF000301.GetDefaultSetting("1");
    var settingList = new List<F0003>();

    foreach (var set in dcDefault)
    {
        settingList.Add(
        new F0003
        {
          AP_NAME = set.AP_NAME,
          SYS_PATH = set.SYS_PATH,
          DESCRIPT = set.DESCRIPT,
          DC_CODE = dc.DC_CODE,
          GUP_CODE = "00",
          CUST_CODE = "00"
        }
      );
    }

    // 新增F0003 - 寫入CUST共用資料
		if(dcService.Any())
		{
			var custDefault = repoF000301.GetDefaultSetting("2");
			foreach(var p in dcService)
			{ 
				{ 
					foreach(var data in custDefault)
					{
              settingList.Add(
              new F0003
              {
                AP_NAME = data.AP_NAME,
                SYS_PATH = data.SYS_PATH,
                DESCRIPT = data.DESCRIPT,
                DC_CODE = p.DC_CODE,
                GUP_CODE = p.GUP_CODE,
                CUST_CODE = p.CUST_CODE
              }
            );
					}
				}
			}
		}

    repoF0003.BulkInsert(settingList);

			//8.新增F190906 - 寫入資料
			var imgNoMax = repoF190906.GetDatasByTrueAndCondition().Max(a => a.IMG_NO);   //找出目前流水號(PK值)
			if (dcService.Any())
			{
				var f190906DatasDefault = repoF190906.GetDatasByTrueAndCondition(o => o.DC_CODE == "00" && o.GUP_CODE == "00" && o.CUST_CODE == "00");
				foreach (var p in dcService)
				{
					var f190906Datas = repoF190906.GetDatasByTrueAndCondition(o => o.DC_CODE == p.DC_CODE && o.GUP_CODE == p.GUP_CODE && o.CUST_CODE == p.CUST_CODE);
					if (!f190906Datas.Any())
					{
						foreach (var data in f190906DatasDefault)
						{
							data.IMG_NO = ++imgNoMax;  //流水號遞增
							data.DC_CODE = p.DC_CODE;
							data.GUP_CODE = p.GUP_CODE;
							data.CUST_CODE = p.CUST_CODE;
							repoF190906.Add(data);
						}
					}
				}
			}
			else
			{
				var f190906DatasDefault = repoF190906.GetDatasByTrueAndCondition(o => o.DC_CODE == "00" && o.GUP_CODE == "00" && o.CUST_CODE == "00");
				foreach (var data in f190906DatasDefault)
				{
					data.IMG_NO = ++imgNoMax;  //流水號遞增
					data.DC_CODE = dc.DC_CODE;
					data.GUP_CODE = gupCode;
					data.CUST_CODE = custCode;
					repoF190906.Add(data);
				}
			}

			//9.新增F190907 - 寫入資料
			var patchNoMax = repoF190907.GetDatasByTrueAndCondition().Max(a => a.PATH_NO);   //找出目前流水號(PK值)
			if (dcService.Any())
			{
				var f190907DatasDefault = repoF190907.GetDatasByTrueAndCondition(o => o.DC_CODE == "00" && o.GUP_CODE == "00" && o.CUST_CODE == "00");
				foreach (var p in dcService)
				{
					var f190907Datas = repoF190907.GetDatasByTrueAndCondition(o => o.DC_CODE == p.DC_CODE && o.GUP_CODE == p.GUP_CODE && o.CUST_CODE == p.CUST_CODE);
					if (!f190907Datas.Any())
					{
						foreach (var data in f190907DatasDefault)
						{
							data.PATH_NO = ++patchNoMax;  //流水號遞增
							data.DC_CODE = p.DC_CODE;
							data.GUP_CODE = p.GUP_CODE;
							data.CUST_CODE = p.CUST_CODE;
							repoF190907.Add(data);
						}
					}
				}
			}
			else
			{
				var f190907DatasDefault = repoF190907.GetDatasByTrueAndCondition(o => o.DC_CODE == "00" && o.GUP_CODE == "00" && o.CUST_CODE == "00");
				foreach (var data in f190907DatasDefault)
				{
					data.PATH_NO = ++patchNoMax;  //流水號遞增
					data.DC_CODE = dc.DC_CODE;
					data.GUP_CODE = gupCode;
					data.CUST_CODE = custCode;
					repoF190907.Add(data);
				}
			}

			// 新增F190105
			repoF190105.Add(new F190105
			{
				DC_CODE = dc.DC_CODE,
				OPEN_AUTO_ALLOC_STOCK = "0",
				B2B_PDA_PICK_PECENT = 1,
				B2C_PDA_PICK_PERCENT = 1,
				B2B_AUTO_PICK_PRINT = "0",
				B2C_AUTO_PICK_PRINT = "0",
				WAIT_SEND_AUTO_PICK = "0",
				USE_CONTAINER = "0",
				PICKORDER_MAX_RECORD = 21,
				ORDER_MAX_ITEMCNT = 10,
				ORDER_MAX_RECORD = 21,
				OPEN_SPECIAL_ORDER = "0",
				SHIP_MODE = "0"
			});

			repoF1901 = null;
        repoF190101 = null;
        repoF190904 = null;
        repoF190102 = null;
				repoF190906 = null;
				repoF190907 = null;
			return result;
    }


    public ExecuteResult UpdateP190101(F1901 dc, List<F190101> dcService, string userId)
    {
      ExecuteResult result = new ExecuteResult() { IsSuccessed = true };
      var repoF1901 = new F1901Repository(Schemas.CoreSchema, _wmsTransaction);
      var repoF190101 = new F190101Repository(Schemas.CoreSchema, _wmsTransaction);

      // 0. 先檢查是否已被刪除
      var data = repoF1901.Find(x => x.DC_CODE == dc.DC_CODE);
      if (data == null)
      {
        // 資料已被刪除
        result.IsSuccessed = false;
        result.Message = Properties.Resources.DataDelete;
        return result;
      }
      // 1. 更新主檔
      data.DC_NAME = dc.DC_NAME;
      data.TEL = dc.TEL;
      data.FAX = dc.FAX;
      data.ADDRESS = dc.ADDRESS;
      data.LAND_AREA = dc.LAND_AREA;
      data.BUILD_AREA = dc.BUILD_AREA;
      data.SHORT_CODE = dc.SHORT_CODE;
      data.BOSS = dc.BOSS;
      data.MAIL_BOX = dc.MAIL_BOX;
      repoF1901.Update(data);

      // 2. 更新F190101 - 刪除不在選取範圍裡的資料
      foreach (var p in repoF190101.Filter(x => x.DC_CODE.Equals(dc.DC_CODE)))
      {
        if (!dcService.Contains(p)) repoF190101.Delete(o => o.DC_CODE == p.DC_CODE && o.CUST_CODE == p.CUST_CODE && o.GUP_CODE == p.GUP_CODE);
      }
      // 2.1.更新F190101 - 寫入新資料
      foreach (var p in dcService)
      {
        if (repoF190101.Filter(x => x.CUST_CODE == p.CUST_CODE && x.GUP_CODE == p.GUP_CODE && x.DC_CODE == p.DC_CODE).Any()) continue;
        repoF190101.Add(new F190101() { DC_CODE = p.DC_CODE, CUST_CODE = p.CUST_CODE, GUP_CODE = p.GUP_CODE });
      }
      repoF1901 = null;
      repoF190101 = null;
      return result;
    }

    public ExecuteResult UpdateP190101(F1901 dc, List<F190101> dcService, List<F190904> F190904List, List<F050006> F050006List, List<F190102> F190102List, string userId, string gupCode, string custCode)
    {
        ExecuteResult result = new ExecuteResult() { IsSuccessed = true };
        var repoF1901 = new F1901Repository(Schemas.CoreSchema, _wmsTransaction);
        var repoF190101 = new F190101Repository(Schemas.CoreSchema, _wmsTransaction);
        var repoF190904 = new F190904Repository(Schemas.CoreSchema, _wmsTransaction);
        var repoF050006 = new F050006Repository(Schemas.CoreSchema, _wmsTransaction);
        var repoF190102 = new F190102Repository(Schemas.CoreSchema, _wmsTransaction);
			  var repoF0003 = new F0003Repository(Schemas.CoreSchema, _wmsTransaction);
			  var repoF190906 = new F190906Repository(Schemas.CoreSchema, _wmsTransaction);
			  var repoF190907 = new F190907Repository(Schemas.CoreSchema, _wmsTransaction);
        var repoF000301 = new F000301Repository(Schemas.CoreSchema);

			// 0. 先檢查是否已被刪除
			var data = repoF1901.Find(x => x.DC_CODE == dc.DC_CODE);
        if (data == null)
        {
            // 資料已被刪除
            result.IsSuccessed = false;
            result.Message = Properties.Resources.DataDelete;
            return result;
        }
        // 1. 更新主檔
        data.DC_NAME = dc.DC_NAME;
        data.TEL = dc.TEL;
        data.FAX = dc.FAX;
        data.ADDRESS = dc.ADDRESS;
        data.LAND_AREA = dc.LAND_AREA;
        data.BUILD_AREA = dc.BUILD_AREA;
        data.SHORT_CODE = dc.SHORT_CODE;
        data.BOSS = dc.BOSS;
        data.MAIL_BOX = dc.MAIL_BOX;
		data.ZIP_CODE = dc.ZIP_CODE;
        repoF1901.Update(data);

        // 2. 更新F190101 - 刪除不在選取範圍裡的資料
        //foreach (var p in repoF190101.Filter(x => x.DC_CODE.Equals(dc.DC_CODE)))
        //{
        //    //if (!dcService.Contains(p)) repoF190101.Delete(o => o.DC_CODE == p.DC_CODE && o.CUST_CODE == p.CUST_CODE && o.GUP_CODE == p.GUP_CODE);
        //    repoF190101.Delete(o => o.DC_CODE == p.DC_CODE && o.CUST_CODE == p.CUST_CODE && o.GUP_CODE == p.GUP_CODE);
        //}
        // 2.1.更新F190101 - 寫入新資料
        foreach (var p in dcService)
        {
            if (repoF190101.Filter(x => x.CUST_CODE == p.CUST_CODE && x.GUP_CODE == p.GUP_CODE && x.DC_CODE == p.DC_CODE).Any()) continue;
            //repoF190101.Add(new F190101() { DC_CODE = p.DC_CODE, CUST_CODE = p.CUST_CODE, GUP_CODE = p.GUP_CODE });
            repoF190101.Add(new F190101() { DC_CODE = p.DC_CODE, CUST_CODE = p.CUST_CODE, GUP_CODE = p.GUP_CODE });
        }
			var nowF190101Data = repoF190101.GetDatasByTrueAndCondition(o=>o.DC_CODE == dc.DC_CODE).ToList();

			foreach (var d in nowF190101Data)
			{
				var exist = dcService.Find(o => o.DC_CODE == d.DC_CODE && o.GUP_CODE == d.GUP_CODE && o.CUST_CODE == d.CUST_CODE);
				if (exist == null)
					repoF190101.Delete(o => o.DC_CODE == d.DC_CODE && o.GUP_CODE == d.GUP_CODE && o.CUST_CODE == d.CUST_CODE);
			}

		// 3. 更新F190904 - 刪除不在選取範圍裡的資料
		//foreach (var p in repoF190904.Filter(x => x.DC_CODE.Equals(dc.DC_CODE)))
		//{
		//    //if (!F190904List.Contains(p)) repoF190904.Delete(o => o.DC_CODE == p.DC_CODE && o.CUST_CODE == p.CUST_CODE && o.GUP_CODE == p.GUP_CODE && o.LOC_CODE == p.LOC_CODE);
		//    repoF190904.Delete(o => o.DC_CODE == p.DC_CODE && o.CUST_CODE == p.CUST_CODE && o.GUP_CODE == p.GUP_CODE && o.LOC_CODE == p.LOC_CODE);
		//}
		// 3.1.更新F190904 - 寫入新資料
			foreach (var p in F190904List)
				{
					var curr = repoF190904.Find(x => x.CUST_CODE == p.CUST_CODE && x.GUP_CODE == p.GUP_CODE && x.DC_CODE == p.DC_CODE);
					if (curr == null)
						repoF190904.Add(p);
					else
					{
						curr.LOC_CODE = p.LOC_CODE;
						repoF190904.Update(curr);
					}
				}

			//4. 更新F050006 - 新增或刪除郵遞區號資料
			if (F050006List.Any())
			{
				var F050006Datas = repoF050006.GetAllDatas().Where(x => x.DC_CODE == dc.DC_CODE);
				var modifyDatas = F050006List.Select(o => new { DC_CODE = o.DC_CODE, GUP_CODE = o.GUP_CODE, CUST_CODE = o.CUST_CODE, ZIP_CODE = o.ZIP_CODE }).ToList();
				var sourceDatas = F050006Datas.Select(o => new { DC_CODE = o.DC_CODE, GUP_CODE = o.GUP_CODE, CUST_CODE = o.CUST_CODE, ZIP_CODE = o.ZIP_CODE }).ToList();

				foreach (var i in dcService)  //勾選的貨主
				{
					var F050006CustCodeModify = modifyDatas.Where(x => x.DC_CODE == dc.DC_CODE && x.CUST_CODE == i.CUST_CODE).ToList(); //勾選的貨主與畫面上郵遞區號比對
					var F050006CustCodeSource = sourceDatas.Where(x => x.DC_CODE == dc.DC_CODE && x.CUST_CODE == i.CUST_CODE).ToList(); //勾選的貨主與資料庫郵遞區號比對

					if (!F050006CustCodeModify.Any() && !F050006CustCodeSource.Any())   //畫面無資料，資料庫無資料(全部新增)
					{
						var saveDatas = new List<F050006>();
						var f050006Datas = repoF050006.GetDatasByTrueAndCondition(o => o.DC_CODE == "00" && o.GUP_CODE == "00" && o.CUST_CODE == "00");

						foreach (var item in f050006Datas)
						{
							var curr = new F050006();
							curr.DC_CODE = i.DC_CODE;
							curr.GUP_CODE = i.GUP_CODE;
							curr.CUST_CODE = i.CUST_CODE;
							curr.ZIP_CODE = item.ZIP_CODE;

							saveDatas.Add(curr);
						}
						repoF050006.BulkInsert(saveDatas);
					}
					else if (!F050006CustCodeModify.Any() && F050006CustCodeSource.Any())   //畫面無資料，資料庫有資料(全部刪除)
					{
						repoF050006.Delete(x => x.CUST_CODE == i.CUST_CODE);
					}
					else   //畫面有資料，資料庫有、無資料(單筆新增、刪除)
					{
						var addDatas = modifyDatas.Where(x => x.CUST_CODE == i.CUST_CODE).Except(sourceDatas.Where(x => x.CUST_CODE == i.CUST_CODE)).ToList();      //取差集(新增郵遞區號)
						var deleteDatas = sourceDatas.Where(x => x.CUST_CODE == i.CUST_CODE).Except(modifyDatas).Where(x => x.CUST_CODE == i.CUST_CODE).ToList();   //取差集(刪除郵遞區號)

						var addF050006Datas = new List<F050006>();
						foreach (var k in addDatas)
						{
							addF050006Datas.Add(new F050006 { DC_CODE = k.DC_CODE, GUP_CODE = k.GUP_CODE, CUST_CODE = k.CUST_CODE, ZIP_CODE = k.ZIP_CODE });
						}

						repoF050006.BulkInsert(addF050006Datas);  //新增

						var keys = from a in deleteDatas
											 select a.DC_CODE + a.GUP_CODE + a.CUST_CODE + a.ZIP_CODE;
						repoF050006.DeletedF050006Datas(keys.ToList());   //刪除
					}

				}

			}
			else
			{
				//畫面無資料(全部刪除)
				repoF050006.Delete(x => x.DC_CODE == dc.DC_CODE);
			}


			//5. 更新F190102 - 新增或刪除配送效率順序
			if (F190102List.Any())
			{
				var F190102Datas = repoF190102.GetAllDatas().Where(x => x.DC_CODE == dc.DC_CODE);
				var modifyDatas = F190102List.Select(o => new { DC_CODE = o.DC_CODE, DELV_EFFIC = o.DELV_EFFIC, SORT = o.SORT }).ToList();
				var sourceDatas = F190102Datas.Select(o => new { DC_CODE = o.DC_CODE, DELV_EFFIC = o.DELV_EFFIC, SORT = o.SORT }).ToList();

				var addDatas = modifyDatas.Except(sourceDatas).ToList();      //取差集(新增配送效率順序)
				var deleteDatas = sourceDatas.Except(modifyDatas).ToList();   //取差集(刪除配送效率順序)

				var addF190102Datas = new List<F190102>();
				foreach (var i in addDatas)
				{
					addF190102Datas.Add(new F190102 { DC_CODE = i.DC_CODE, DELV_EFFIC = i.DELV_EFFIC, SORT = i.SORT });
				}

				repoF190102.BulkInsert(addF190102Datas);  //新增

				var keys = from a in deleteDatas
									 select a.DC_CODE + a.DELV_EFFIC;
				repoF190102.DeletedF190102Datas(keys.ToList());   //刪除
			}
			else
			{
				// 刪除repoF190102資料
				repoF190102.Delete(x => x.DC_CODE == dc.DC_CODE);
			}

      var settingList = new List<F0003>();

			foreach (var p in dcService)
			{
				var f0003Datas = repoF0003.GetDatasByTrueAndCondition(o => o.DC_CODE == p.DC_CODE && o.GUP_CODE == p.GUP_CODE && o.CUST_CODE == p.CUST_CODE);
				if (!f0003Datas.Any())
				{
					var f0003DatasDefault = repoF000301.GetDefaultSetting("2");
					foreach (var defData in f0003DatasDefault)
					{
              settingList.Add(
              new F0003
              {
                DC_CODE = p.DC_CODE,
                GUP_CODE = p.GUP_CODE,
                CUST_CODE = p.CUST_CODE,
                AP_NAME = defData.AP_NAME,
                SYS_PATH = defData.SYS_PATH,
                DESCRIPT = defData.DESCRIPT
              }
            );
					}
				}
			}

      repoF0003.BulkInsert(settingList);
      
			var imgNoMax = repoF190906.GetDatasByTrueAndCondition().Max(a => a.IMG_NO);   //找出目前流水號(PK值)
			var f190906DatasDefault = repoF190906.GetDatasByTrueAndCondition(o => o.DC_CODE == "00" && o.GUP_CODE == "00" && o.CUST_CODE == "00");
			foreach (var p in dcService)
			{
				var f190906Datas = repoF190906.GetDatasByTrueAndCondition(o => o.DC_CODE == p.DC_CODE && o.GUP_CODE == p.GUP_CODE && o.CUST_CODE == p.CUST_CODE);
				if (!f190906Datas.Any())
				{
					foreach (var defData in f190906DatasDefault)
					{
						defData.IMG_NO = ++imgNoMax;  //流水號遞增
						defData.DC_CODE = p.DC_CODE;
						defData.GUP_CODE = p.GUP_CODE;
						defData.CUST_CODE = p.CUST_CODE;
						repoF190906.Add(defData);
					}
				}
			}

			var patchNoMax = repoF190907.GetDatasByTrueAndCondition().Max(a => a.PATH_NO);   //找出目前流水號(PK值)
			var f190907DatasDefault = repoF190907.GetDatasByTrueAndCondition(o => o.DC_CODE == "00" && o.GUP_CODE == "00" && o.CUST_CODE == "00");
			foreach (var p in dcService)
			{
				var f190907Datas = repoF190907.GetDatasByTrueAndCondition(o => o.DC_CODE == p.DC_CODE && o.GUP_CODE == p.GUP_CODE && o.CUST_CODE == p.CUST_CODE);
				if (!f190907Datas.Any())
				{
					foreach (var defData in f190907DatasDefault)
					{
						defData.PATH_NO = ++patchNoMax;  //流水號遞增
						defData.DC_CODE = p.DC_CODE;
						defData.GUP_CODE = p.GUP_CODE;
						defData.CUST_CODE = p.CUST_CODE;
						repoF190907.Add(defData);
					}
				}
			}

			repoF1901 = null;
        repoF190101 = null;
        repoF190904 = null;
        repoF190102 = null;
        return result;
    }

    public ExecuteResult DeleteP190101(string dcCode)
    {
      ExecuteResult result = new ExecuteResult() { IsSuccessed = true };
      var repoF1901 = new F1901Repository(Schemas.CoreSchema, _wmsTransaction);
      var repoF190101 = new F190101Repository(Schemas.CoreSchema, _wmsTransaction);
      var repoF190904 = new F190904Repository(Schemas.CoreSchema, _wmsTransaction);
      var repoF050006 = new F050006Repository(Schemas.CoreSchema, _wmsTransaction);
      var repoF190102 = new F190102Repository(Schemas.CoreSchema, _wmsTransaction);
      var repoF0005 = new F0005Repository(Schemas.CoreSchema, _wmsTransaction);
      var repoF0003 = new F0003Repository(Schemas.CoreSchema, _wmsTransaction);
			var repoF190906 = new F190906Repository(Schemas.CoreSchema, _wmsTransaction);
			var repoF190907 = new F190907Repository(Schemas.CoreSchema, _wmsTransaction);
			var repoF190105 = new F190105Repository(Schemas.CoreSchema, _wmsTransaction);

			// 先判斷存不存在該項目, 不存在就回傳該資料已糟刪除
			if (!repoF1901.Filter(x => x.DC_CODE == dcCode).Any())
      {
        result.IsSuccessed = false;
        result.Message = Properties.Resources.P190101Service_DataDeleted;
      }
      else
      {
        // 刪除F1901的資料
        repoF1901.Delete(x => x.DC_CODE == dcCode);

        // 刪除F190101資料
        repoF190101.Delete(x => x.DC_CODE == dcCode);

        // 刪除F190904資料
        repoF190904.Delete(x => x.DC_CODE == dcCode);

        // 刪除F050006資料
        repoF050006.Delete(x => x.DC_CODE == dcCode);

        // 刪除F190102資料
        repoF190102.Delete(x => x.DC_CODE == dcCode);
		// 刪除F0005資料
		repoF0005.Delete(x=>x.DC_CODE == dcCode);
		// 刪除F0003資料
		repoF0003.Delete(x=>x.DC_CODE == dcCode);

				// 刪除F190906資料
				repoF190906.Delete(x => x.DC_CODE == dcCode);

				// 刪除F190907資料
				repoF190907.Delete(x => x.DC_CODE == dcCode);
				// 刪除F190105資料
				repoF190105.Delete(x => x.DC_CODE == dcCode);

				result.IsSuccessed = true;
      }
      
      return result;

    }
  }
}

