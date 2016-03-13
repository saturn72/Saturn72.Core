namespace Saturn72.Modules.EntityFramework
{
    /// <summary>
    ///     Represents database provider <see cref="IDatabaseProvider" />
    /// </summary>
    public interface IDatabaseProvider
    {
        /// <summary>
        ///     Sets database initializer
        /// </summary>
        void SetDatabaseInitializer();
    }
}