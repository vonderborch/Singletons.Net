# ResettableSingleton

## Overview
Provides a singleton instance that can be explicitly reset. A new instance will be created on next access after reset.

## When to Use
- When you need to reset the singleton instance during the application's lifetime.
- For scenarios where the singleton's state may need to be refreshed.

## Usage Example
```csharp
// Access the singleton instance
var instance = ResettableSingleton<MyService>.Instance;

// Reset the singleton instance
ResettableSingleton<MyService>.Reset();

// Next access creates a new instance
var newInstance = ResettableSingleton<MyService>.Instance;
```

## API Notes
- The type parameter must have a parameterless constructor.
- Thread-safe access and reset. 