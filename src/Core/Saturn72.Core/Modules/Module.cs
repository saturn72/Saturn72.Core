namespace Saturn72.Core.Modules
{
    /// <summary>
    ///     Represents system module
    /// </summary>
    public class Module
    {
        public Module(string type, bool active, string description, string name)
        {
            Type = type;
            Description = description;
            Name = name;
            Active = active;
        }

        /// <summary>
        ///     Gets module full name
        /// </summary>
        public string Type { get; }

        /// <summary>
        ///     Gets value indicating if the module is active
        /// </summary>
        public bool Active { get; }

        /// <summary>
        ///     Gets module description
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets module name
        /// </summary>
        public string Name { get;  }

        public void Start()
        {
            throw new System.NotImplementedException();
        }
    }
}