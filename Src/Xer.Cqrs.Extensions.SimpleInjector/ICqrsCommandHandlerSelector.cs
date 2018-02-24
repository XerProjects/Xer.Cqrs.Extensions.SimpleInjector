using System.Collections.Generic;
using System.Reflection;
using SimpleInjector;

namespace Xer.Cqrs.Extensions.SimpleInjector
{
    public interface ICqrsCommandHandlerSelector
    {
        ICqrsCommandHandlerSelector ByInterface();
        ICqrsCommandHandlerSelector ByInterface(Lifestyle lifestyle);
        ICqrsCommandHandlerSelector ByAttribute();
        ICqrsCommandHandlerSelector ByAttribute(Lifestyle lifestyle);
    }
}