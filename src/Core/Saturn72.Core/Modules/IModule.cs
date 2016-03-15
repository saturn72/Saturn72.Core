namespace Saturn72.Core.Modules
{
    public interface IModule
    {
        /// <summary>
        /// Loads the module
        /// </summary>
        void Load();

        /// <summary>
        /// starts the module
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the module
        /// </summary>
        void Stop();

        /// <summary>
        /// Module startup order
        /// </summary>
        int StartupOrder { get;  }

        /// <summary>
        /// MOdule's stop order
        /// </summary>
        int StopOrder { get; }
    }
}
