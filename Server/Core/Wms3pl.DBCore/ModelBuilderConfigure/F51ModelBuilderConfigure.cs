using Microsoft.EntityFrameworkCore;
using Wms3pl.Datas.F51;

namespace Wms3pl.DBCore
{
  public partial class ModelBuilderConfigure
  {
     private static void SetF51ModelRelation(ModelBuilder modelBuilder)
     {
        modelBuilder.Entity<F5101>().HasKey(key => new { key.CAL_DATE,key.ITEM_CODE,key.CUST_CODE,key.GUP_CODE,key.DC_CODE });
        modelBuilder.Entity<F510101>().HasKey(key => new { key.CAL_DATE,key.LOG_SEQ,key.WMS_NO,key.ITEM_CODE,key.CUST_CODE,key.GUP_CODE,key.DC_CODE });
        modelBuilder.Entity<F510102>().HasKey(key => new { key.CAL_DATE,key.LOC_CODE,key.ITEM_CODE,key.VALID_DATE,key.ENTER_DATE,key.MAKE_NO,key.DC_CODE,key.GUP_CODE,key.CUST_CODE,key.SERIAL_NO,key.VNR_CODE,key.BOX_CTRL_NO,key.PALLET_CTRL_NO });
        modelBuilder.Entity<F510104>().HasKey(key => new { key.CAL_DATE,key.ORDER_NO,key.ORDER_SEQ,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
				modelBuilder.Entity<F510105>().HasKey(key => new { key.ID });
				modelBuilder.Entity<F51010501>().HasKey(key => new { key.ID });
        modelBuilder.Entity<F5102>().HasKey(key => new { key.CAL_DATE,key.LOC_TYPE_ID,key.TMPR_TYPE,key.CUST_CODE,key.GUP_CODE,key.DC_CODE });
        modelBuilder.Entity<F5103>().HasKey(key => new { key.CAL_DATE,key.SEQ_NO,key.CUST_CODE,key.GUP_CODE,key.DC_CODE });
        modelBuilder.Entity<F5104>().HasKey(key => new { key.CAL_DATE,key.SEQ_NO,key.CUST_CODE,key.GUP_CODE,key.DC_CODE });
        modelBuilder.Entity<F5105>().HasKey(key => new { key.CAL_DATE,key.SEQ_NO,key.CUST_CODE,key.GUP_CODE,key.DC_CODE });
        modelBuilder.Entity<F5106>().HasKey(key => new { key.CAL_DATE,key.SEQ_NO,key.CUST_CODE,key.GUP_CODE,key.DC_CODE });
        modelBuilder.Entity<F5107>().HasKey(key => new { key.CAL_DATE,key.SEQ_NO,key.CUST_CODE,key.GUP_CODE,key.DC_CODE });
        modelBuilder.Entity<F5108>().HasKey(key => new { key.CAL_DATE,key.SEQ_NO,key.CUST_CODE,key.GUP_CODE,key.DC_CODE });
        modelBuilder.Entity<F5109>().HasKey(key => new { key.CAL_DATE,key.SEQ_NO,key.CUST_CODE,key.GUP_CODE,key.DC_CODE });
        modelBuilder.Entity<F511001>().HasKey(key => new { key.DC_CODE,key.GUP_CODE,key.CUST_CODE,key.WMS_NO,key.ITEM_CODE });
        modelBuilder.Entity<F511002>().HasKey(key => new { key.DC_CODE,key.GUP_CODE,key.CUST_CODE,key.WMS_NO,key.ITEM_CODE,key.VALID_DATE });
        modelBuilder.Entity<F5111>().HasKey(key => new { key.DC_CODE,key.GUP_CODE,key.CUST_CODE,key.CAL_TYPE,key.RANGE_LEVEL });
        modelBuilder.Entity<F5112>().HasKey(key => new { key.DC_CODE,key.GUP_CODE,key.CUST_CODE,key.CAL_DATE,key.DELV_NO });
     }
  }
}
