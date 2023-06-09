using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;

namespace Wms3pl.WebServices.Process.P19.ExDataSources
{
	public partial class P19ExDataSource
	{

		public IQueryable<ExecuteResult> ExecuteResults
		{
			get { return new List<ExecuteResult>().AsQueryable(); }
		}

		public IQueryable<F1954> F1924s
		{
			get { return new List<F1954>().AsQueryable(); }
		}

		#region 範例用，以後移除
		public IQueryable<F1929> F1929s
		{
			get { return new List<F1929>().AsQueryable(); }
		}

		public IQueryable<F1929WithF1909Test> F1929WithF1909Tests
		{
			get { return new List<F1929WithF1909Test>().AsQueryable(); }
		}
		#endregion 範例用，以後移除

		#region 系統共用
		public IQueryable<FunctionShowInfo> FunctionShowInfos
		{
			get { return new List<FunctionShowInfo>().AsQueryable(); }
		}
		#endregion 系統共用

		#region P1905 - 權限功能
		public IQueryable<P1905GroupFunctionMapping> P1905GroupFunctionMappings
		{
			get { return new List<P1905GroupFunctionMapping>().AsQueryable(); }
		}

		public IQueryable<F1924Data> F1924Datas
		{
			get { return new List<F1924Data>().AsQueryable(); }
		}

		public IQueryable<F190101Data> F190101Datas
		{
			get { return new List<F190101Data>().AsQueryable(); }
		}

		/// <summary>
		/// 供ExDataService使用, 若沒加上則會在回傳到Client時於Client出現錯誤
		/// </summary>
		public IQueryable<F1912> F1912s
		{
			get { return new List<F1912>().AsQueryable(); }
		}

		public IQueryable<FunctionCodeName> FunctionCodeNames
		{
			get { return new List<FunctionCodeName>().AsQueryable(); }
		}
		#endregion

		#region P1906
		public IQueryable<EmpWithFuncionName> EmpWithFuncionNames
		{
			get { return new List<EmpWithFuncionName>().AsQueryable(); }
		}
		public IQueryable<GetUserPassword> GetUserPasswords
		{
			get { return new List<GetUserPassword>().AsQueryable(); }
		}
		#endregion

		#region P190103 商品材積
		public IQueryable<F1905Data> F1905Datas
		{
			get { return new List<F1905Data>().AsQueryable(); }
		}
		#endregion

		#region P190103 商品包裝維護
		public IQueryable<F190301Data> F190301Datas
		{
			get { return new List<F190301Data>().AsQueryable(); }
		}
		#endregion


		#region P1947 出貨碼頭分配相關
		public IQueryable<F1947WithF194701> F1947WithF194701s
		{
			get { return new List<F1947WithF194701>().AsQueryable(); }
		}
		#endregion

		#region P1601020000 使用者被設定的作業區(倉別清單)
		public IQueryable<UserWarehouse> UserWarehouses
		{
			get { return new List<UserWarehouse>().AsQueryable(); }
		}
		#endregion




		public IQueryable<F1954Ex> F1954Exs
		{
			get { return new List<F1954Ex>().AsQueryable(); }
		}

		#region P910304000 標籤資訊
		public IQueryable<F197001Data> F197001Datas
		{
			get { return new List<F197001Data>().AsQueryable(); }
		}
		#endregion

		#region 商品召回
		public IQueryable<DistributionData> DistributionDatas
		{
			get { return new List<DistributionData>().AsQueryable(); }
		}
		#endregion

		public IQueryable<F1909EX> F1909EXs
		{
			get { return new List<F1909EX>().AsQueryable(); }
		}

		#region P7105020000 作業計價
		public IQueryable<F199002Data> F199002Datas
		{
			get { return new List<F199002Data>().AsQueryable(); }
		}
		#endregion

		#region 作業群組儲位
		public IQueryable<F1912LocData> F1912LocDatas
		{
			get { return new List<F1912LocData>().AsQueryable(); }
		}
		#endregion

		public IQueryable<F1934EX> F1934EXs
		{
			get { return new List<F1934EX>().AsQueryable(); }
		}

		public IQueryable<F1947JoinF194701> F1947JoinF194701EXs
		{
			get { return new List<F1947JoinF194701>().AsQueryable(); }
		}

		public IQueryable<F19470101Datas> F19470101DatasEXs
		{
			get { return new List<F19470101Datas>().AsQueryable(); }
		}

		#region P7105050000 派車計價
		public IQueryable<F199005Data> F199005Datas
		{
			get { return new List<F199005Data>().AsQueryable(); }
		}

		public IQueryable<F91000302Data> F91000302Datas
		{
			get { return new List<F91000302Data>().AsQueryable(); }
		}

		public IQueryable<F194702Data> F194702Datas
		{
			get { return new List<F194702Data>().AsQueryable(); }
		}

		public IQueryable<F1948Data> F1948Datas
		{
			get { return new List<F1948Data>().AsQueryable(); }
		}
		#endregion

		public IQueryable<F190102JoinF000904> F190102JoinF000904DatasEXs
		{
			get { return new List<F190102JoinF000904>().AsQueryable(); }
		}

		public IQueryable<F1912LocVolumn> F1912LocVolumns
		{
			get { return new List<F1912LocVolumn>().AsQueryable(); }
		}

		public IQueryable<F0003Ex> F0003Exs
		{
			get { return new List<F0003Ex>().AsQueryable(); }
		}

		#region P710705
		public IQueryable<P710705BackWarehouseInventory> P710705BackWarehouseInventorys
		{
			get { return new List<P710705BackWarehouseInventory>().AsQueryable(); }
		}
		public IQueryable<P710705MergeExecution> P710705MergeExecutions
		{
			get { return new List<P710705MergeExecution>().AsQueryable(); }
		}
		public IQueryable<P710705Availability> P710705Availabilitys
		{
			get { return new List<P710705Availability>().AsQueryable(); }
		}
		public IQueryable<P710705ChangeDetail> P710705ChangeDetails
		{
			get { return new List<P710705ChangeDetail>().AsQueryable(); }
		}
		public IQueryable<P710705WarehouseDetail> P710705WarehouseDetails
		{
			get { return new List<P710705WarehouseDetail>().AsQueryable(); }
		}

		#endregion

		public IQueryable<F91000302SearchData> F91000302SearchDatas
		{
			get { return new List<F91000302SearchData>().AsQueryable(); }
		}

		public IQueryable<P192019Item> P192019Items
		{
			get { return new List<P192019Item>().AsQueryable(); }
		}

		public IQueryable<F1952Ex> F1952Exs
		{
			get { return new List<F1952Ex>().AsQueryable(); }
		}

		public IQueryable<F1912WareHouseData> F1912WareHouseDatas
		{
			get { return new List<F1912WareHouseData>().AsQueryable(); }
		}

		public IQueryable<WareHouseIdByWareHouseType> WareHouseIdByWareHouseTypes
		{
			get { return new List<WareHouseIdByWareHouseType>().AsQueryable(); }
		}
		public IQueryable<P192019Data> P192019Datas
		{
			get { return new List<P192019Data>().AsQueryable(); }
		}

        #region P190116 車次維護
        public IQueryable<F19471601Data> F19471601Datas
        {
            get { return new List<F19471601Data>().AsQueryable(); }
        }
        #endregion

        #region P1905130000 平台系統帳號管理
        public IQueryable<F0070LoginData> F0070AccountStatusDatas
        {
            get { return new List<F0070LoginData>().AsQueryable(); }
        }
		#endregion

		#region
		public IQueryable<F190105> F190105s
		{
			get { return new List<F190105>().AsQueryable(); }
		}

		public IQueryable<F190106> F190106s
		{
			get { return new List<F190106>().AsQueryable(); }
		}
		#endregion

		public IQueryable<F190106Data> F190106Datas
		{
			get { return new List<F190106Data>().AsQueryable(); }
		}

        #region P1901920000集貨場維護
        public IQueryable<F1945CollectionList> F1945CollectionLists
        {
            get { return new List<F1945CollectionList>().AsQueryable(); }
        }
        public IQueryable<F1945CellList> F1945CellLists
        {
            get { return new List<F1945CellList>().AsQueryable(); }
        }
        #endregion P1901920000集貨場維護

    }
}
