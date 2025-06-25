# GenericSingleton

## Overview
A thread-safe, generic singleton implementation for any reference type. Ensures only one instance of the specified type is created and can be accessed globally.

## When to Use
- When you need a classic singleton for any reference type.
- When you want to control initialization (by instance or factory).

## Usage Example
```csharp
// Set the singleton instance directly
GenericSingleton<MyService>.Instance = new MyService();

// Or use a factory for lazy initialization
GenericSingleton<MyService>.SetFactory(() => new MyService());

// Access the singleton instance
var instance = GenericSingleton<MyService>.Instance;
```

## API Notes
- Throws if you try to set or get the instance before initialization.
- You can overwrite the instance/factory if needed by passing `overwrite: true`. 