# MultiInstanceSingleton

## Overview
Manages multiple singleton instances, each identified by a unique key. Ensures thread-safe retrieval and removal of instances, with lazy initialization per key.

## When to Use
- When you need a singleton per key (e.g., per tenant, per configuration, etc.).
- When you want to manage the lifecycle of keyed singletons.

## Usage Example
```csharp
// Set the default factory for all keys
MultiInstanceSingleton<string, MyService>.DefaultFactory = () => new MyService();

// Get or create a singleton instance for a specific key
var instance = MultiInstanceSingleton<string, MyService>.GetInstance("tenant1");

// Or provide a factory per call
var instance2 = MultiInstanceSingleton<string, MyService>.GetInstance("tenant2", () => new MyService());

// Remove a singleton instance for a key
MultiInstanceSingleton<string, MyService>.RemoveInstance("tenant1");
```

## API Notes
- Throws if no factory is provided and `DefaultFactory` is not set.
- Each key gets its own singleton instance. 