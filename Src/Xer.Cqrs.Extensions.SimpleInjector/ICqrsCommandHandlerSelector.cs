using System.Collections.Generic;
using System.Reflection;
using SimpleInjector;

namespace Xer.Cqrs.Extensions.SimpleInjector
{
    public interface ICqrsCommandHandlerSelector
    {
        ICqrsCommandHandlerSelector ByInterface(params Assembly[] assemblies);
        ICqrsCommandHandlerSelector ByInterface(Lifestyle lifestyle, params Assembly[] assemblies);
        ICqrsCommandHandlerSelector ByAttribute(params Assembly[] assemblies);
        ICqrsCommandHandlerSelector ByAttribute(Lifestyle lifestyle, params Assembly[] assemblies);
    }
}