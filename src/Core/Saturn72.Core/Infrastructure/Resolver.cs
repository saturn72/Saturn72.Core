namespace Saturn72.Core.Infrastructure
{
    public class Resolver
    {
        public static ITypeFinder TypeFinder => Resolve<ITypeFinder>();

        protected static TService Resolve<TService>(object key = null) where TService : class
        {
            return EngineContext.Current.Resolve<TService>(key);
        }

    }
}
