# SingletonBase

## Overview
Provides a thread-safe base class for implementing the singleton design pattern. Inherit from this class to easily create a singleton for your own type, with lazy initialization and thread safety.

## When to Use
- When you want to implement a singleton for your own class with minimal boilerplate.
- When you need a thread-safe, lazily initialized singleton instance.

## Usage Example
```csharp
public class MyService : SingletonBase<MyService>
{
    // Private constructor to prevent external instantiation
    private MyService() { }
    // Add your service logic here
}

// Access the singleton instance
var instance = MyService.Instance;
```

## API Notes
- Inherit from `SingletonBase<T>` where `T` is your class type.
- The base class uses reflection to create an instance, so your class can have a private constructor.
- The singleton instance is created lazily and is thread-safe. 