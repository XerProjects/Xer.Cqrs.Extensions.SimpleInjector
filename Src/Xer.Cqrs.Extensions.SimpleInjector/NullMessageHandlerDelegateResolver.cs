using System;
using Xer.Delegator;

namespace Xer.Cqrs.Extensions.SimpleInjector
{
    internal class NullMessageHandlerDelegateResolver : IMessageHandlerResolver
    {
        private static readonly Lazy<NullMessageHandlerDelegateResolver> _singleton = new Lazy<NullMessageHandlerDelegateResolver>(() => new NullMessageHandlerDelegateResolver());
        public static readonly NullMessageHandlerDelegateResolver Instance = _singleton.Value;

        private NullMessageHandlerDelegateResolver() { }

        public MessageHandlerDelegate ResolveMessageHandler(Type messageType) => NullMessageHandlerDelegate.Instance;
    }
}