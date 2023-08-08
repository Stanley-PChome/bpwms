using Microsoft.EntityFrameworkCore;
using Wms3pl.Datas.F05;

namespace Wms3pl.DBCore
{
	public partial class ModelBuilderConfigure
	{
		private static void SetF05ModelRelation(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<F0500>().HasKey(key => new { key.WORK_CODE });
			modelBuilder.Entity<F050001>().HasKey(key => new { key.ORD_NO, key.GUP_CODE, key.CUST_CODE });
			modelBuilder.Entity<F05000101>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F050002>().HasKey(key => new { key.ORD_NO, key.ORD_SEQ, key.DC_CODE, key.GUP_CODE, key.CUST_CODE });
			modelBuilder.Entity<F05000201>().HasKey(key => new { key.CUST_ORD_NO, key.SERIAL_NO, key.GUP_CODE, key.CUST_CODE });
			modelBuilder.Entity<F050003>().HasKey(key => new { key.SEQ_NO });
			modelBuilder.Entity<F05000301>().HasKey(key => new { key.SEQ_NO, key.ITEM_CODE });
			modelBuilder.Entity<F050004>().HasKey(key => new { key.TICKET_ID, key.CUST_CODE, key.GUP_CODE, key.DC_CODE });
			modelBuilder.Entity<F050006>().HasKey(key => new { key.ZIP_CODE, key.DC_CODE, key.GUP_CODE, key.CUST_CODE });
			modelBuilder.Entity<F050007>().HasKey(key => new { key.ZIP_CODE, key.REGION_CODE, key.GUP_CODE, key.CUST_CODE });
			modelBuilder.Entity<F0501>().HasKey(key => new { key.DC_CODE, key.GUP_CODE, key.CUST_CODE, key.ITEM_CODE });
			modelBuilder.Entity<F0501_HISTORY>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F050101>().HasKey(key => new { key.ORD_NO, key.GUP_CODE, key.CUST_CODE, key.DC_CODE });
			modelBuilder.Entity<F05010103>().HasKey(key => new { key.LOG_ID });
			modelBuilder.Entity<F050102>().HasKey(key => new { key.ORD_NO, key.ORD_SEQ, key.DC_CODE, key.GUP_CODE, key.CUST_CODE });
			modelBuilder.Entity<F050103>().HasKey(key => new { key.ORD_NO, key.CUST_ORD_NO, key.DC_CODE, key.GUP_CODE, key.CUST_CODE });
			modelBuilder.Entity<F050104>().HasKey(key => new { key.ORD_NO, key.ORD_SEQ, key.DC_CODE, key.GUP_CODE, key.CUST_CODE, key.SERVICE_ITEM_CODE });
			modelBuilder.Entity<F05010301>().HasKey(key => new { key.ORD_NO, key.ORD_SEQ, key.DC_CODE, key.GUP_CODE, key.CUST_CODE });
			modelBuilder.Entity<F050301>().HasKey(key => new { key.ORD_NO, key.CUST_CODE, key.GUP_CODE, key.DC_CODE });
			modelBuilder.Entity<F05030101>().HasKey(key => new { key.ORD_NO, key.WMS_ORD_NO, key.DC_CODE, key.GUP_CODE, key.CUST_CODE });
			modelBuilder.Entity<F050302>().HasKey(key => new { key.ORD_NO, key.ORD_SEQ, key.DC_CODE, key.GUP_CODE, key.CUST_CODE });
			modelBuilder.Entity<F05030201>().HasKey(key => new { key.ORD_NO, key.ORD_SEQ, key.DC_CODE, key.GUP_CODE, key.CUST_CODE });
			modelBuilder.Entity<F05030202>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F050303>().HasKey(key => new { key.BATCH_NO, key.CUST_CODE, key.GUP_CODE, key.DC_CODE });
			modelBuilder.Entity<F050304>().HasKey(key => new { key.DC_CODE, key.GUP_CODE, key.CUST_CODE, key.ORD_NO });
			modelBuilder.Entity<F050801>().HasKey(key => new { key.WMS_ORD_NO, key.CUST_CODE, key.GUP_CODE, key.DC_CODE });
			modelBuilder.Entity<F050802>().HasKey(key => new { key.WMS_ORD_NO, key.WMS_ORD_SEQ, key.GUP_CODE, key.CUST_CODE, key.DC_CODE });
			modelBuilder.Entity<F050803>().HasKey(key => new { key.DC_CODE, key.GUP_CODE, key.CUST_CODE, key.WMS_ORD_NO });
			modelBuilder.Entity<F050804>().HasKey(key => new { key.DC_CODE, key.GUP_CODE, key.CUST_CODE, key.STICKER_NO });
			modelBuilder.Entity<F05080401>().HasKey(key => new { key.DC_CODE, key.GUP_CODE, key.CUST_CODE, key.STICKER_NO, key.STICKER_SEQ });
			modelBuilder.Entity<F050805>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F05080501>().HasKey(key => new { key.DC_CODE, key.GUP_CODE, key.CUST_CODE, key.CAL_NO, key.ORD_NO });
			modelBuilder.Entity<F05080502>().HasKey(key => new { key.DC_CODE, key.GUP_CODE, key.CUST_CODE, key.CAL_NO, key.ORD_NO, key.ORD_SEQ });
			modelBuilder.Entity<F05080503>().HasKey(key => new { key.DC_CODE, key.GUP_CODE, key.CUST_CODE, key.CAL_NO });
			modelBuilder.Entity<F05080504>().HasKey(key => new { key.DC_CODE, key.GUP_CODE, key.CUST_CODE, key.CAL_NO, key.WAREHOUSE_ID, key.AREA_CODE });
			modelBuilder.Entity<F050901>().HasKey(key => new { key.CONSIGN_ID });
			modelBuilder.Entity<F051201>().HasKey(key => new { key.PICK_ORD_NO, key.CUST_CODE, key.GUP_CODE, key.DC_CODE });
			modelBuilder.Entity<F05120101>().HasKey(key => new { key.DC_CODE, key.GUP_CODE, key.CUST_CODE, key.PICK_ORD_NO });
			modelBuilder.Entity<F051202>().HasKey(key => new { key.PICK_ORD_NO, key.PICK_ORD_SEQ, key.GUP_CODE, key.CUST_CODE, key.DC_CODE });
			modelBuilder.Entity<F051206>().HasKey(key => new { key.LACK_SEQ });
			modelBuilder.Entity<F0513>().HasKey(key => new { key.DELV_DATE, key.CUST_CODE, key.PICK_TIME, key.GUP_CODE, key.DC_CODE });
			modelBuilder.Entity<F0515>().HasKey(key => new { key.BATCH_NO, key.DC_CODE, key.GUP_CODE, key.CUST_CODE });
			modelBuilder.Entity<F051501>().HasKey(key => new { key.BATCH_PICK_NO, key.DC_CODE, key.GUP_CODE, key.CUST_CODE });
			modelBuilder.Entity<F051502>().HasKey(key => new { key.BATCH_PICK_NO, key.BATCH_PICK_SEQ, key.DC_CODE, key.GUP_CODE, key.CUST_CODE });
			modelBuilder.Entity<F051503>().HasKey(key => new { key.BATCH_NO, key.PICK_ORD_NO, key.DC_CODE, key.GUP_CODE, key.CUST_CODE });
			modelBuilder.Entity<F051504>().HasKey(key => new { key.BATCH_NO, key.RETAIL_CODE, key.DC_CODE, key.GUP_CODE, key.CUST_CODE });
			modelBuilder.Entity<F051601>().HasKey(key => new { key.BATCH_NO, key.ITEM_CODE, key.DC_CODE, key.GUP_CODE, key.CUST_CODE });
			modelBuilder.Entity<F051602>().HasKey(key => new { key.BATCH_NO, key.ITEM_CODE, key.RETAIL_CODE, key.WMS_ORD_NO, key.LOC_CODE, key.DC_CODE, key.GUP_CODE, key.CUST_CODE });
			modelBuilder.Entity<F051603>().HasKey(key => new { key.BATCH_NO, key.ITEM_CODE, key.RETAIL_CODE, key.WMS_ORD_NO, key.LOC_CODE, key.CARTON, key.DC_CODE, key.GUP_CODE, key.CUST_CODE });
			modelBuilder.Entity<F052901>().HasKey(key => new { key.WMS_ORD_NO, key.DC_CODE, key.GUP_CODE, key.CUST_CODE });
			modelBuilder.Entity<F052902>().HasKey(key => new { key.WMS_ORD_NO, key.ITEM_CODE, key.DC_CODE, key.GUP_CODE, key.CUST_CODE });
			modelBuilder.Entity<F055001>().HasKey(key => new { key.WMS_ORD_NO, key.PACKAGE_BOX_NO, key.DC_CODE, key.GUP_CODE, key.CUST_CODE });
			modelBuilder.Entity<F05500101>().HasKey(key => new { key.WMS_ORD_NO, key.LOG_SEQ, key.DC_CODE, key.GUP_CODE, key.CUST_CODE, key.PACKAGE_BOX_NO });
			modelBuilder.Entity<F05500102>().HasKey(key => new { key.SOURCE_TYPE, key.MSG_TYPE });
			modelBuilder.Entity<F05500103>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F055002>().HasKey(key => new { key.WMS_ORD_NO, key.PACKAGE_BOX_NO, key.PACKAGE_BOX_SEQ, key.DC_CODE, key.GUP_CODE, key.CUST_CODE });
			modelBuilder.Entity<F055003>().HasKey(key => new { key.WMS_ORD_NO, key.ITEM_CODE, key.PACKAGE_BOX_NO, key.DC_CODE, key.GUP_CODE, key.CUST_CODE });
			modelBuilder.Entity<F050305>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F050306>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F050306_HISTORY>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F055004>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F051203>().HasKey(key => new { key.DC_CODE, key.GUP_CODE, key.CUST_CODE, key.PICK_ORD_NO, key.TTL_PICK_SEQ });
			modelBuilder.Entity<F051301>().HasKey(key => new { key.DC_CODE, key.GUP_CODE, key.CUST_CODE, key.DELV_DATE, key.PICK_TIME, key.WMS_NO });
      modelBuilder.Entity<F051301_HISTORY>().HasKey(key => new { key.ID });
      modelBuilder.Entity<F05120601>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F052903>().HasKey(key => new { key.PICK_ORD_NO, key.WMS_ORD_NO, key.DC_CODE, key.GUP_CODE, key.CUST_CODE });
			modelBuilder.Entity<F05290301>().HasKey(key => new { key.PICK_ORD_NO, key.PICK_ORD_SEQ, key.DC_CODE, key.GUP_CODE, key.CUST_CODE });
			modelBuilder.Entity<F051401>().HasKey(key => new { key.DC_CODE, key.COLLECTION_CODE, key.CELL_CODE });
			modelBuilder.Entity<F051402>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F052904>().HasKey(key => new { key.DC_CODE, key.GUP_CODE, key.CUST_CODE, key.PICK_ORD_NO, key.CONTAINER_CODE });
			modelBuilder.Entity<F05290401>().HasKey(key => new { key.DC_CODE, key.GUP_CODE, key.CUST_CODE, key.PICK_ORD_NO, key.CONTAINER_CODE, key.ITEM_CODE });
			modelBuilder.Entity<F052905>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F05290501>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F055005>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F055006>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F055007>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F05080505>().HasKey(key => new { key.DC_CODE, key.GUP_CODE, key.CUST_CODE, key.CAL_NO, key.ORD_NO });
			modelBuilder.Entity<F05080506>().HasKey(key => new { key.DC_CODE, key.GUP_CODE, key.CUST_CODE, key.CAL_NO, key.ORD_NO, key.ITEM_CODE });
			modelBuilder.Entity<F056001>().HasKey(key => new { key.DC_CODE, key.GUP_CODE, key.CUST_CODE, key.WORKSTATION_CODE, key.BOX_CODE });
			modelBuilder.Entity<F056002>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F051801>().HasKey(key => new { key.DC_CODE, key.CONVENIENT_CODE, key.CELL_CODE });
			modelBuilder.Entity<F051802>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F051803>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F0530>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F0531>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F0532>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F053201>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F053202>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F053203>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F0533>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F0534>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F0535>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F0536>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F053601>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F053602>().HasKey(key => new { key.ID });
			modelBuilder.Entity<F0537>().HasKey(key => new { key.ID });
		}
	}
}
