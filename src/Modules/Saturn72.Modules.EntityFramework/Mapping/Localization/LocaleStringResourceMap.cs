using Saturn72.Core.Domain.Localization;

namespace Saturn72.Modules.EntityFramework.Mapping.Localization
{
    public class LocaleStringResourceMap : EfEntityTypeConfiguration<LocaleStringResource>
    {
        protected override void Initialize()
        {
            ToTable("LocaleStringResource");
            HasKey(p => p.Id);
            Property(x => x.ResourceName).IsRequired();
        }
    }
}