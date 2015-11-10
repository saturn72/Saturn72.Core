using System.Runtime.CompilerServices;

namespace Automation.Core.Infrastructure
{
    public class EngineContext
    {
        public static IEngine Current
        {
            get
            {
                return Singleton<IEngine>.Instance ?? (Singleton<IEngine>.Instance = Initialize(false));
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static IEngine Initialize(bool forceRestart)
        {
            if (Singleton<IEngine>.Instance == null || forceRestart)
            {
                Singleton<IEngine>.Instance = CreateEngineInstance();
                Singleton<IEngine>.Instance.Initialize();
            }
            return Singleton<IEngine>.Instance;
        }

        private static IEngine CreateEngineInstance()
        {
            return new Engine();
        }
    }
}