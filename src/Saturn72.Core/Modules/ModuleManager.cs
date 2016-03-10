using System;
using Saturn72.Extensions;

namespace Saturn72.Core.Modules
{
    public class ModuleManager
    {
        public static void Start(Module module)
        {
            if (!module.Active)
                return;

            module.Start();
            Console.Out.WriteLine(">> Module {0} was started".AsFormat(module.Name));
        }
    }
}