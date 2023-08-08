using Microsoft.EntityFrameworkCore;
using Wms3pl.Datas.F02;

namespace Wms3pl.DBCore
{
	public partial class ModelBuilderConfigure
	{
		private static void SetF02ModelRelation(ModelBuilder modelBuilder)
		{
      modelBuilder.Entity<F020103>().HasKey(key => new { key.ARRIVE_DATE, key.PURCHASE_NO, key.DC_CODE, key.GUP_CODE, key.CUST_CODE, key.SERIAL_NO });
      modelBuilder.Entity<F020104>().HasKey(key => new { key.BEGIN_DATE, key.END_DATE, key.PIER_CODE, key.DC_CODE });
      modelBuilder.Entity<F0202>().HasKey(key => new { key.ORDER_NO, key.GUP_CODE, key.CUST_CODE, key.DC_CODE });
      modelBuilder.Entity<F020201>().HasKey(key => new { key.RT_NO, key.RT_SEQ, key.DC_CODE, key.GUP_CODE, key.CUST_CODE });
      modelBuilder.Entity<F02020101>().HasKey(key => new { key.PURCHASE_NO, key.PURCHASE_SEQ, key.DC_CODE, key.GUP_CODE, key.CUST_CODE });
      modelBuilder.Entity<F02020102>().HasKey(key => new { key.PURCHASE_NO, key.PURCHASE_SEQ, key.CHECK_NO, key.DC_CODE, key.GUP_CODE, key.CUST_CODE, key.RT_NO });
      modelBuilder.Entity<F02020103>().HasKey(key => new { key.CUR_DATE, key.CUST_CODE, key.GUP_CODE, key.DC_CODE });
      modelBuilder.Entity<F02020104>().HasKey(key => new { key.PURCHASE_NO, key.PURCHASE_SEQ, key.LOG_SEQ, key.DC_CODE, key.GUP_CODE, key.CUST_CODE, key.RT_NO });
      modelBuilder.Entity<F02020105>().HasKey(key => new { key.RT_NO, key.UPLOAD_TYPE, key.UPLOAD_NO, key.DC_CODE, key.GUP_CODE, key.CUST_CODE });
      modelBuilder.Entity<F02020106>().HasKey(key => new { key.UPLOAD_TYPE });
      modelBuilder.Entity<F02020107>().HasKey(key => new { key.RT_NO, key.PURCHASE_NO, key.ALLOCATION_NO, key.DC_CODE, key.GUP_CODE, key.CUST_CODE });
      modelBuilder.Entity<F02020108>().HasKey(key => new { key.ID });
      modelBuilder.Entity<F02020109>().HasKey(key => new { key.ID });
      modelBuilder.Entity<F020301>().HasKey(key => new { key.FILE_NAME, key.DC_CODE, key.GUP_CODE, key.CUST_CODE });
      modelBuilder.Entity<F020302>().HasKey(key => new { key.FILE_NAME, key.SERIAL_NO, key.DC_CODE, key.GUP_CODE, key.CUST_CODE });
      modelBuilder.Entity<F020202>().HasKey(key => new { key.ID });
      modelBuilder.Entity<F020203>().HasKey(key => new { key.DC_CODE, key.GUP_CODE, key.CUST_CODE, key.ITEM_CODE, key.RT_DATE });
      modelBuilder.Entity<F0205>().HasKey(key => new { key.ID });
      modelBuilder.Entity<F020501>().HasKey(key => new { key.ID });
      modelBuilder.Entity<F020502>().HasKey(key => new { key.ID });
      modelBuilder.Entity<F020503>().HasKey(key => new { key.ID });
      modelBuilder.Entity<F020504>().HasKey(key => new { key.ID });
      modelBuilder.Entity<F02050401>().HasKey(key => new { key.ID });
      modelBuilder.Entity<F02050402>().HasKey(key => new { key.ID });
      modelBuilder.Entity<F020601>().HasKey(key => new { key.ID });
      modelBuilder.Entity<F020603>().HasKey(key => new { key.ID });
    }
  }
}
