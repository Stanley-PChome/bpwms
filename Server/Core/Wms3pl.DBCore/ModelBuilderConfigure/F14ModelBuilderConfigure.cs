using Microsoft.EntityFrameworkCore;
using Wms3pl.Datas.F14;

namespace Wms3pl.DBCore
{
  public partial class ModelBuilderConfigure
  {
     private static void SetF14ModelRelation(ModelBuilder modelBuilder)
     {
        modelBuilder.Entity<F140101>().HasKey(key => new { key.INVENTORY_NO,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F14010101>().HasKey(key => new { key.INVENTORY_NO,key.LOC_CODE,key.ITEM_CODE,key.VALID_DATE,key.ENTER_DATE,key.SERIAL_NO,key.DC_CODE,key.GUP_CODE,key.CUST_CODE,key.BOX_CTRL_NO,key.PALLET_CTRL_NO,key.MAKE_NO });
        modelBuilder.Entity<F140102>().HasKey(key => new { key.INVENTORY_NO,key.WAREHOUSE_ID,key.DC_CODE,key.GUP_CODE,key.CUST_CODE,key.AREA_CODE });
        modelBuilder.Entity<F140103>().HasKey(key => new { key.INVENTORY_NO,key.ITEM_CODE,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F140104>().HasKey(key => new { key.INVENTORY_NO,key.LOC_CODE,key.ITEM_CODE,key.VALID_DATE,key.ENTER_DATE,key.DC_CODE,key.GUP_CODE,key.CUST_CODE,key.BOX_CTRL_NO,key.PALLET_CTRL_NO,key.MAKE_NO });
        modelBuilder.Entity<F140105>().HasKey(key => new { key.INVENTORY_NO,key.LOC_CODE,key.ITEM_CODE,key.VALID_DATE,key.ENTER_DATE,key.DC_CODE,key.GUP_CODE,key.CUST_CODE,key.BOX_CTRL_NO,key.PALLET_CTRL_NO,key.MAKE_NO });
        modelBuilder.Entity<F140106>().HasKey(key => new { key.INVENTORY_NO,key.LOC_CODE,key.ITEM_CODE,key.VALID_DATE,key.ENTER_DATE,key.DC_CODE,key.GUP_CODE,key.CUST_CODE,key.BOX_CTRL_NO,key.PALLET_CTRL_NO,key.MAKE_NO });
        modelBuilder.Entity<F140107>().HasKey(key => new { key.INVENTORY_NO,key.LOC_CODE,key.ITEM_CODE,key.VALID_DATE,key.ENTER_DATE,key.DC_CODE,key.GUP_CODE,key.CUST_CODE,key.BOX_CTRL_NO,key.PALLET_CTRL_NO,key.MAKE_NO });
        modelBuilder.Entity<F140110>().HasKey(key => new { key.INVENTORY_NO,key.ITEM_CODE,key.LOC_CODE,key.ISSECOND,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F140111>().HasKey(key => new { key.INVENTORY_NO,key.SEQ,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F140113>().HasKey(key => new { key.INVENTORY_NO,key.ITEM_CODE,key.LOC_CODE,key.ISSECOND,key.DC_CODE,key.GUP_CODE,key.CUST_CODE,key.INVENTORY_SEQ });
     }
  }
}
