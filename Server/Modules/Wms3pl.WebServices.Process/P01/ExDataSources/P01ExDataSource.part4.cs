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
        //進貨蒐集單維護_匯出品號    
        public IQueryable<F010301ITEM_CODE> F010301ITEM_CODEEXC
        {
            get { return new List<F010301ITEM_CODE>().AsQueryable(); }
        }

        //進貨蒐集單維護_匯出進貨清冊    
        public IQueryable<F010301BOX_NO> F010301BOX_NOEXC
        {
            get { return new List<F010301BOX_NO>().AsQueryable(); }
        }

        //進貨蒐集單維護_匯出進貨清冊 箱明細   
        public IQueryable<F010301BOX_NO_ITEM> F010301BOX_NOITRMEXC
        {
            get { return new List<F010301BOX_NO_ITEM>().AsQueryable(); }
        }

        //進貨蒐集單維護_匯出進貨清冊 彙整總表   
        public IQueryable<F010301BOX_NO_ITEMTOTAL> F010301BOX_NOITRMEXCTO
        {
            get { return new List<F010301BOX_NO_ITEMTOTAL>().AsQueryable(); }
        }

        //進貨蒐集單維護_匯出進貨清冊 彙整總表   
        public IQueryable<F010301BOX_NO_ITEMTOTALodd> F010301BOX_NOITRMEXCTOodd
        {
            get { return new List<F010301BOX_NO_ITEMTOTALodd>().AsQueryable(); }
        }

        //進貨蒐集單維護_匯出進貨清冊 單箱明細   
        public IQueryable<F010301BOX_NO_ITEMTOTALoddONE> F010301BOX_NOITRMEXCTOoddONE
        {
            get { return new List<F010301BOX_NO_ITEMTOTALoddONE>().AsQueryable(); }
        }

        //進貨蒐集單維護_匯出進貨清冊 單箱明細 1  
        public IQueryable<F010301BOX_NO_ITEMTOTALoddONE1> F010301BOX_NOITRMEXCTOoddONE1
        {
            get { return new List<F010301BOX_NO_ITEMTOTALoddONE1>().AsQueryable(); }
        }

        //進貨蒐集單維護_產出進倉單  
        public IQueryable<F010301STOCK_NO> F010301STOCKNO
        {
            get { return new List<F010301STOCK_NO>().AsQueryable(); }
        }

        //進貨蒐集單維護_產出調撥單  
        public IQueryable<F010301ALLOCATION_NO> F010301ImportF150201
        {
            get { return new List<F010301ALLOCATION_NO>().AsQueryable(); }
        }


    }
}
