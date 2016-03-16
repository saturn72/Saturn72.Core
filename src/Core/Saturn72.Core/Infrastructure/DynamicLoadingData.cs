using Saturn72.Extensions.Common;

namespace Saturn72.Core.Infrastructure
{
    /// <summary>
    ///     Contains appdomain dynamic loading data
    /// </summary>
    public class DynamicLoadingData
    {
        /// <summary>
        ///     Creates new instance of <see cref="DynamicLoadingData" />
        /// </summary>
        /// <param name="rootDirectory">Root directory as absolute or relative path.</param>
        /// <param name="shadowCopyDirectory">Shadow copy directory as absolute or relative path.</param>
        public DynamicLoadingData(string rootDirectory, string shadowCopyDirectory)
        {
            RootDirectory = IoUtil.RelativePathToAbsolutePath(rootDirectory);
            ShadowCopyDirectory = IoUtil.RelativePathToAbsolutePath(shadowCopyDirectory);
        }

        public string RootDirectory { get; }
        public string ShadowCopyDirectory { get; }
    }
}