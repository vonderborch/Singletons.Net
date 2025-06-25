# SingletonRegistry

## Overview
A central registry for managing singleton instances by type. Allows registering, retrieving, and removing singleton instances in a thread-safe manner.

## When to Use
- When you want a global registry for singleton instances.
- When you need to manage singletons dynamically at runtime.

## Usage Example
```csharp
// Register a singleton instance
SingletonRegistry.Register(new MyService());

// Retrieve the singleton instance
var instance = SingletonRegistry.Get<MyService>();

// Try to get a singleton instance safely
if (SingletonRegistry.TryGet<MyService>(out var myService))
{
    // Use myService
}

// Remove a singleton instance
SingletonRegistry.Remove<MyService>();

// Clear all registered singletons
SingletonRegistry.Clear();
```

## API Notes
- Throws if you try to get an instance that is not registered.
- Thread-safe for all operations. 