using Microsoft.EntityFrameworkCore;
using Wms3pl.Datas.F91;

namespace Wms3pl.DBCore
{
  public partial class ModelBuilderConfigure
  {
     private static void SetF91ModelRelation(ModelBuilder modelBuilder)
     {
        modelBuilder.Entity<F910001>().HasKey(key => new { key.PROCESS_ID });
        modelBuilder.Entity<F910003>().HasKey(key => new { key.ITEM_TYPE_ID });
        modelBuilder.Entity<F91000301>().HasKey(key => new { key.ITEM_TYPE_ID,key.ACC_ITEM_KIND_ID,key.DELV_ACC_TYPE });
        modelBuilder.Entity<F91000302>().HasKey(key => new { key.ITEM_TYPE_ID,key.ACC_UNIT });
        modelBuilder.Entity<F910004>().HasKey(key => new { key.PRODUCE_NO,key.DC_CODE });
        modelBuilder.Entity<F910005>().HasKey(key => new { key.ACTION_NO });
        modelBuilder.Entity<F910101>().HasKey(key => new { key.BOM_NO,key.CUST_CODE,key.GUP_CODE });
        modelBuilder.Entity<F910102>().HasKey(key => new { key.BOM_NO,key.MATERIAL_CODE,key.CUST_CODE,key.GUP_CODE });
        modelBuilder.Entity<F910201>().HasKey(key => new { key.PROCESS_NO,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F910203>().HasKey(key => new { key.PROCESS_NO,key.PRODUCE_NO,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F910204>().HasKey(key => new { key.PROCESS_NO,key.ACTION_NO,key.ORDER_BY,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F910205>().HasKey(key => new { key.PROCESS_NO,key.PICK_NO,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F91020501>().HasKey(key => new { key.PICK_NO,key.PICK_SEQ,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F91020502>().HasKey(key => new { key.PICK_NO,key.ALLOCATION_NO,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F910206>().HasKey(key => new { key.PROCESS_NO,key.BACK_NO,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F91020601>().HasKey(key => new { key.PROCESS_NO,key.BACK_NO,key.SERIAL_NO,key.ITEM_CODE,key.ALLOCATION_NO,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F910207>().HasKey(key => new { key.DC_CODE,key.GUP_CODE,key.CUST_CODE,key.PROCESS_ITEM });
        modelBuilder.Entity<F910208>().HasKey(key => new { key.ID });
        modelBuilder.Entity<F910209>().HasKey(key => new { key.DC_CODE,key.GUP_CODE,key.CUST_CODE,key.PROCESS_NO,key.PICK_LOC,key.VALID_DATE,key.ENTER_DATE,key.MAKE_NO,key.BOX_CTRL_NO,key.PALLET_CTRL_NO,key.ALLOCATION_NO,key.SERIAL_NO });
        modelBuilder.Entity<F910301>().HasKey(key => new { key.CONTRACT_NO,key.DC_CODE,key.GUP_CODE });
        modelBuilder.Entity<F910302>().HasKey(key => new { key.CONTRACT_NO,key.CONTRACT_SEQ,key.DC_CODE,key.GUP_CODE });
        modelBuilder.Entity<F910401>().HasKey(key => new { key.QUOTE_NO,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F910402>().HasKey(key => new { key.QUOTE_NO,key.PROCESS_ID,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F910403>().HasKey(key => new { key.QUOTE_NO,key.ITEM_CODE,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F910404>().HasKey(key => new { key.QUOTE_NO,key.UPLOAD_NO,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F910501>().HasKey(key => new { key.DEVICE_IP,key.DC_CODE });
        modelBuilder.Entity<F910502>().HasKey(key => new { key.PROCESS_NO,key.LOG_SEQ,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F910503>().HasKey(key => new { key.PROCESS_NO,key.LOG_SEQ,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F910504>().HasKey(key => new { key.PROCESS_NO,key.LOG_SEQ,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F910505>().HasKey(key => new { key.PROCESS_NO,key.LOG_SEQ,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F910506>().HasKey(key => new { key.PROCESS_NO,key.LOG_SEQ,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F910507>().HasKey(key => new { key.PROCESS_NO,key.LOG_SEQ,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F910508>().HasKey(key => new { key.PROCESS_NO,key.LOG_SEQ,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
        modelBuilder.Entity<F910509>().HasKey(key => new { key.PROCESS_NO,key.LOG_SEQ,key.DC_CODE,key.GUP_CODE,key.CUST_CODE });
     }
  }
}
