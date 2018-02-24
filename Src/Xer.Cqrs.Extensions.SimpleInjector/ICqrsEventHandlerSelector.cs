using System.Collections.Generic;
using System.Reflection;
using SimpleInjector;

namespace Xer.Cqrs.Extensions.SimpleInjector
{
    public interface ICqrsEventHandlerSelector
    {
        ICqrsEventHandlerSelector ByInterface();
        ICqrsEventHandlerSelector ByInterface(Lifestyle lifestyle);
        ICqrsEventHandlerSelector ByAttribute();
        ICqrsEventHandlerSelector ByAttribute(Lifestyle lifestyle);
    }
}