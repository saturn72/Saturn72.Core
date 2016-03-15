namespace Saturn72.Core.Domain.Localization
{
    public class LocaleStringResource : BaseEntity
    {
        /// <summary>
        ///     Gets or sets the resource name
        /// </summary>
        public string ResourceName { get; set; }

        /// <summary>
        ///     Gets or sets the resource value
        /// </summary>
        public string ResourceValue { get; set; }
    }
}