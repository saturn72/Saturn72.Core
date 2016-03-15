namespace Saturn72.Core.Modules
{
    public static class ModuleExtensions
    {
        /// <summary>
        /// Restarts the module
        /// </summary>
        /// <param name="module">System module <see cref="IModule"/></param>
        public static void Restart(this IModule module)
        {
            module.Stop();
            module.Start();
        }
    }
}