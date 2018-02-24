using System;
using System.Collections.Generic;
using System.Reflection;
using SimpleInjector;

namespace Xer.Cqrs.Extensions.SimpleInjector
{
    public interface ICqrsBuilder
    {
        ICqrsBuilder RegisterCommandHandlers(Action<ICqrsCommandHandlerSelector> selector);
        ICqrsBuilder RegisterEventHandlers(Action<ICqrsEventHandlerSelector> selector);
    }
}