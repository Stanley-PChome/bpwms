using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;

namespace Wms3pl.WebServices.Process.P01.ExDataSources
{
	public partial class P01ExDataSource
	{
		public IQueryable<ExecuteResult> ExecuteResults
		{
			get { return new List<ExecuteResult>().AsQueryable(); }
		}

		#region P010201 進倉單維護

		public IQueryable<F010201Data> F010201Datas
		{
			get { return new List<F010201Data>().AsQueryable(); }
		}

		public IQueryable<F010202Data> F010202Datas
		{
			get { return new List<F010202Data>().AsQueryable(); }
		}

		public IQueryable<F010201MainData> F010201MainDatas
		{
			get { return new List<F010201MainData>().AsQueryable(); }
		}
		#endregion

		public IQueryable<F010101ShopNoList> F010101ShopNoLists
		{
			get { return new List<F010101ShopNoList>().AsQueryable(); }
		}

		public IQueryable<F010101Data> F010101Datas
		{
			get { return new List<F010101Data>().AsQueryable(); }
		}

		public IQueryable<F010102Data> F010102Datas
		{
			get { return new List<F010102Data>().AsQueryable(); }
		}

		public IQueryable<F010101ReportData> F010101ReportDatas
		{
			get { return new List<F010101ReportData>().AsQueryable(); }
		}

		public IQueryable<F010201QueryData> F010201QueryDatas
		{
			get { return new List<F010201QueryData>().AsQueryable(); }
		}

		public IQueryable<F010201ImportData> F010201ImportDatas
		{
			get { return new List<F010201ImportData>().AsQueryable(); }
		}

		public IQueryable<F1913WithF1912Qty> F1913WithF1912Qtys
		{
			get { return new List<F1913WithF1912Qty>().AsQueryable(); }
		}

		#region 進倉單維護-列印棧板標籤資料
		public IQueryable<P010201PalletData> P010201PalletDatas
		{
			get { return new List<P010201PalletData>().AsQueryable(); }
		}
        #endregion

		public IQueryable<UserCloseStockParam> UserCloseStockParams
		{
			get { return new List<UserCloseStockParam>().AsQueryable(); }
		}

	}
}
