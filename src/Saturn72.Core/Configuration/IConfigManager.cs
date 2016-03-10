using System.Collections.Generic;
using Saturn72.Core.Infrastructure;
using Saturn72.Core.Modules;

namespace Saturn72.Core.Configuration
{
    public interface IConfigManager
    {
        /// <summary>
        ///     Gets system modules
        /// </summary>
        IEnumerable<Module> Modules { get; }

        /// <summary>
        ///     Gets app domain load data <see cref="AppDomainLoadData"/>
        /// </summary>
        AppDomainLoadData AppDomainLoadData { get; }

        /// <summary>
        ///     Loads configuration data
        /// </summary>
        /// <param name="data">Object contains required data for config loading</param>
        void Load(object data);
    }
}