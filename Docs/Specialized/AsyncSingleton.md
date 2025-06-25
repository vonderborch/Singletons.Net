# AsyncSingleton

## Overview
Provides a thread-safe, asynchronous, and lazy-loaded singleton implementation for a given type. Useful when a singleton requires async initialization, such as database connections, file loading, or API calls.

## When to Use
- When your singleton needs to be initialized asynchronously.
- When you want to ensure only one instance is created, even with concurrent async access.

## Usage Example
```csharp
// Set up the async factory
AsyncSingleton<MyService>.DefaultFactory = async () =>
{
    await Task.Delay(100); // Simulate async work
    return new MyService();
};

// Retrieve the singleton instance asynchronously
var instance = await AsyncSingleton<MyService>.GetInstanceAsync();
```

You can also provide a factory per call:
```csharp
var instance = await AsyncSingleton<MyService>.GetInstanceAsync(async () => new MyService());
```

## API Notes
- If no factory is provided and `DefaultFactory` is not set, an exception is thrown.
- The singleton instance is created only once, even with concurrent calls.
- The type parameter must be a reference type with a parameterless constructor (unless a factory is provided). 