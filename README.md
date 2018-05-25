# Xer.Cqrs.Extensions.SimpleInjector

Extension for SimpleInjector's `Container` to allow easy registration of command handlers and event handlers.

```csharp
public void ConfigureServices(SimpleInjector.Container container)
{
    // Register all CQRS components.
    container.RegisterCqrs(typeof(CommandHandler).Assembly, 
                           typeof(EventHandler).Assembly);
}
```
