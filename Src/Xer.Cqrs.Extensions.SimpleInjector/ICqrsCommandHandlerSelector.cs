using System.Collections.Generic;
using System.Reflection;
using SimpleInjector;

namespace Xer.Cqrs.Extensions.SimpleInjector
{
    public interface ICqrsCommandHandlerSelector
    {
        ICqrsCommandHandlerSelector ByInterface(Assembly assembly);
        ICqrsCommandHandlerSelector ByInterface(Assembly assembly, Lifestyle lifestyle);
        ICqrsCommandHandlerSelector ByInterface(IEnumerable<Assembly> assemblies);
        ICqrsCommandHandlerSelector ByInterface(IEnumerable<Assembly> assemblies, Lifestyle lifestyle);
        ICqrsCommandHandlerSelector ByAttribute(Assembly assembly);
        ICqrsCommandHandlerSelector ByAttribute(Assembly assembly, Lifestyle lifestyle);
        ICqrsCommandHandlerSelector ByAttribute(IEnumerable<Assembly> assemblies);
        ICqrsCommandHandlerSelector ByAttribute(IEnumerable<Assembly> assemblies, Lifestyle lifestyle);
    }
}