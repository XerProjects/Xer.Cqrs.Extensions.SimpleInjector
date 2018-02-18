using System.Collections.Generic;
using System.Reflection;
using SimpleInjector;

namespace Xer.Cqrs.Extensions.SimpleInjector
{
    public interface ICqrsBuilder
    {
        ICqrsBuilder RegisterCommandHandlers(Assembly assembly);
        ICqrsBuilder RegisterCommandHandlers(Assembly assembly, Lifestyle lifeStyle);
        ICqrsBuilder RegisterCommandHandlersAttributes(Assembly assembly);
        ICqrsBuilder RegisterCommandHandlersAttributes(Assembly assembly, Lifestyle lifeStyle);
        ICqrsBuilder RegisterCommandHandlers(IEnumerable<Assembly> assemblies);
        ICqrsBuilder RegisterCommandHandlers(IEnumerable<Assembly> assemblies, Lifestyle lifeStyle);
        ICqrsBuilder RegisterCommandHandlersAttributes(IEnumerable<Assembly> assemblies);
        ICqrsBuilder RegisterCommandHandlersAttributes(IEnumerable<Assembly> assemblies, Lifestyle lifeStyle);
        ICqrsBuilder RegisterCommandDelegator();
        ICqrsBuilder RegisterEventHandlers(Assembly assembly);
        ICqrsBuilder RegisterEventHandlers(Assembly assembly, Lifestyle lifeStyle);
        ICqrsBuilder RegisterEventHandlersAttributes(Assembly assembly);
        ICqrsBuilder RegisterEventHandlersAttributes(Assembly assembly, Lifestyle lifeStyle);
        ICqrsBuilder RegisterEventHandlers(IEnumerable<Assembly> assemblies);
        ICqrsBuilder RegisterEventHandlers(IEnumerable<Assembly> assemblies, Lifestyle lifeStyle);
        ICqrsBuilder RegisterEventHandlersAttributes(IEnumerable<Assembly> assemblies);
        ICqrsBuilder RegisterEventHandlersAttributes(IEnumerable<Assembly> assemblies, Lifestyle lifeStyle);
        ICqrsBuilder RegisterEventDelegator();
    }
}