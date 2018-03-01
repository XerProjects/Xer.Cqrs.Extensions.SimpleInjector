using System.Reflection;
using SimpleInjector;

namespace Xer.Cqrs.Extensions.SimpleInjector
{
    public interface ICqrsEventHandlerSelector
    {
        ICqrsEventHandlerSelector ByInterface(params Assembly[] assemblies);
        ICqrsEventHandlerSelector ByInterface(Lifestyle lifestyle, params Assembly[] assemblies);
        ICqrsEventHandlerSelector ByAttribute(params Assembly[] assemblies);
        ICqrsEventHandlerSelector ByAttribute(Lifestyle lifestyle, params Assembly[] assemblies);
    }
}