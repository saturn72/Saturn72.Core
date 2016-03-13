using Saturn72.Core.Data.Configuration;

namespace Saturn72.Modules.EntityFramework.Mapping.Configuration
{
    public class SettingsMap : EfEntityTypeConfiguration<Setting>
    {
        protected override void Initialize()
        {
            ToTable("Setting");
            HasKey(s => s.Id);

            Property(s => s.Name).IsRequired().HasMaxLength(2048);
            Property(s => s.Value).IsRequired().HasMaxLength(2048);
        }
    }
}