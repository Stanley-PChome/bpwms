using Microsoft.EntityFrameworkCore;


namespace Wms3pl.DBCore
{
	public partial class ModelBuilderConfigure
	{
		public static void Init(ModelBuilder modelBuilder)
		{
      SetF00ModelRelation(modelBuilder);
      SetF01ModelRelation(modelBuilder);
      SetF02ModelRelation(modelBuilder);
      SetF05ModelRelation(modelBuilder);
      SetF06ModelRelation(modelBuilder);
      SetF07ModelRelation(modelBuilder);
      SetF14ModelRelation(modelBuilder);
      SetF15ModelRelation(modelBuilder);
      SetF16ModelRelation(modelBuilder);
      SetF19ModelRelation(modelBuilder);
      SetF20ModelRelation(modelBuilder);
      SetF25ModelRelation(modelBuilder);
      SetF50ModelRelation(modelBuilder);
      SetF51ModelRelation(modelBuilder);
      SetF70ModelRelation(modelBuilder);
      SetF91ModelRelation(modelBuilder);
      SetScheduleModelRelation(modelBuilder);
      SetViewModelRelation(modelBuilder);
    }
  }
}
