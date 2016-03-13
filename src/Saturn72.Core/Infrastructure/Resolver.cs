namespace Saturn72.Core.Infrastructure
{
    public class Resolver
    {
        protected static TService Resolve<TService>() where TService : class
        {
            return EngineContext.Current.Resolve<TService>();
        }
    }
}
