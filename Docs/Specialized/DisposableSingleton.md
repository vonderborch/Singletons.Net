# DisposableSingleton

## Overview
Provides a singleton implementation for types that implement `IDisposable`. The singleton instance can be reset and disposed, ensuring proper resource management.

## When to Use
- When your singleton holds unmanaged resources or needs explicit cleanup.
- When you want to be able to reset or dispose the singleton instance.

## Usage Example
```csharp
// Access the singleton instance
var instance = DisposableSingleton<MyDisposableService>.Instance;

// Dispose and reset the singleton
DisposableSingleton<MyDisposableService>.Reset();
```

## API Notes
- The type parameter must implement `IDisposable` and have a parameterless constructor.
- Calling `Reset()` disposes the current instance and allows a new one to be created on next access. 