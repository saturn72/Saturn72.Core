using Owin;

namespace Saturn72.Modules.Owin
{
    public interface IOwinMiddleware
    {
        void Configure(IAppBuilder appBuilder);
        int Order { get; }
    }
}