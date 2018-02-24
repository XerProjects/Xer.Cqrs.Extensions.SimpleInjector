using System.Collections.Generic;
using System.Reflection;
using SimpleInjector;

namespace Xer.Cqrs.Extensions.SimpleInjector
{
    public interface ICqrsEventHandlerSelector
    {
        ICqrsEventHandlerSelector ByInterface(Assembly assembly);
        ICqrsEventHandlerSelector ByInterface(Assembly assembly, Lifestyle lifestyle);
        ICqrsEventHandlerSelector ByInterface(IEnumerable<Assembly> assemblies);
        ICqrsEventHandlerSelector ByInterface(IEnumerable<Assembly> assemblies, Lifestyle lifestyle);
        ICqrsEventHandlerSelector ByAttribute(Assembly assembly);
        ICqrsEventHandlerSelector ByAttribute(Assembly assembly, Lifestyle lifestyle);
        ICqrsEventHandlerSelector ByAttribute(IEnumerable<Assembly> assemblies);
        ICqrsEventHandlerSelector ByAttribute(IEnumerable<Assembly> assemblies, Lifestyle lifestyle);
    }
}