namespace Automation.Core.Data.Configuration
{
    /// <summary>
    ///     Represents a setting
    /// </summary>
    public class Setting : BaseEntity
    {
        public Setting()
        {
        }

        public Setting(string name, string value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        ///     Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the value
        /// </summary>
        public string Value { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}