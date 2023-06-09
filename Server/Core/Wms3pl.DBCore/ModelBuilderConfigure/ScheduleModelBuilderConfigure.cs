using Microsoft.EntityFrameworkCore;
using Wms3pl.Datas.Schedule;

namespace Wms3pl.DBCore
{
  public partial class ModelBuilderConfigure
  {
     private static void SetScheduleModelRelation(ModelBuilder modelBuilder)
     {
			  modelBuilder.Entity<PREFERENCE>().HasKey(key => new { key.EMP_ID });
				modelBuilder.Entity<SCHEDULE_JOB_RESULT>().HasKey(key => new { key.ID });
		}
  }
}
