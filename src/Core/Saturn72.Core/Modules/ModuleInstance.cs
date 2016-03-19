namespace Saturn72.Core.Modules
{
    /// <summary>
    ///     Represents system module
    /// </summary>
    public class ModuleInstance
    {
        public ModuleInstance(string type, bool active)
        {
            Type = type;

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
    }
}