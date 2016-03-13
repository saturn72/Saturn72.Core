using System.Data.Entity;

namespace Saturn72.Modules.EntityFramework.Initializers
{
    public class CreateDatabaseAndTableIfNotExists<TContext> : CreateTablesIfNotExist<TContext>
        where TContext : DbContext
    {
        public CreateDatabaseAndTableIfNotExists(string[] mandatoryTables, string[] sqlCommands)
            : base(mandatoryTables, sqlCommands)
        {
        }

        public override void InitializeDatabase(TContext context)
        {
            context.Database.CreateIfNotExists();
            base.InitializeDatabase(context);
        }
    }
}