# ThreadLocalSingleton

## Overview
Provides a singleton instance per thread for the specified type. Each thread gets its own unique instance.

## When to Use
- When you need a singleton per thread (e.g., thread-specific caches, state, or resources).
- For scenarios where thread isolation is required.

## Usage Example
```csharp
// Access the singleton instance for the current thread
var instance = ThreadLocalSingleton<MyService>.Instance;
```

## API Notes
- The type parameter must be a reference type with a parameterless constructor.
- Each thread gets its own instance; instances are not shared across threads. 