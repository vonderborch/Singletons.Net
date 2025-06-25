# WeakSingleton

## Overview
Provides a weakly referenced singleton implementation for the specified type. The singleton instance can be garbage collected if no strong references exist, freeing resources when not in use.

## When to Use
- When you want a singleton that does not prevent garbage collection.
- For caching or resource management scenarios where the singleton should not be kept alive unnecessarily.

## Usage Example
```csharp
// Set the default factory
WeakSingleton<MyService>.DefaultFactory = () => new MyService();

// Get or create the singleton instance
var instance = WeakSingleton<MyService>.GetInstance();

// Or provide a factory per call
var instance2 = WeakSingleton<MyService>.GetInstance(() => new MyService());
```

## API Notes
- Throws if no factory is provided and `DefaultFactory` is not set.
- The instance may be garbage collected if no strong references exist.
- Thread-safe for concurrent access. 