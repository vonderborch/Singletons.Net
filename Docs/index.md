# Singletons.Net Library

This library provides a variety of singleton patterns for .NET, each tailored for different use cases such as async initialization, scoping, disposability, thread-local storage, and more. Use these implementations to ensure correct, thread-safe, and flexible singleton management in your applications.

## Singleton Types

| Singleton Type         | Description                                                               | Documentation                                                   | Minimum Version | Maximum Version |
|------------------------|---------------------------------------------------------------------------|-----------------------------------------------------------------|-----------------|-----------------|
| SingletonBase          | Base class for implementing thread-safe, lazy singletons via inheritance. | [SingletonBase](Singletons/SingletonBase.md)                    | 1.0.0.0         |                 |
| AsyncSingleton         | Async, thread-safe singleton for types needing async initialization.      | [AsyncSingleton](Specialized/AsyncSingleton.md)                 | 1.0.0.0         |                 |
| DisposableSingleton    | Singleton for IDisposable types, supports reset/dispose.                  | [DisposableSingleton](Specialized/DisposableSingleton.md)       | 1.0.0.0         |                 |
| GenericSingleton       | Thread-safe, generic singleton for any reference type.                    | [GenericSingleton](Specialized/GenericSingleton.md)             | 1.0.0.0         |                 |
| MultiInstanceSingleton | Manages multiple singleton instances, each identified by a unique key.    | [MultiInstanceSingleton](Specialized/MultiInstanceSingleton.md) | 1.0.0.0         |                 |
| ReadOnlySingleton      | Singleton that is immutable after creation.                               | [ReadOnlySingleton](Specialized/ReadOnlySingleton.md)           | 1.0.0.0         |                 |
| ResettableSingleton    | Singleton that can be explicitly reset.                                   | [ResettableSingleton](Specialized/ResettableSingleton.md)       | 1.0.0.0         |                 |
| ScopedSingleton        | Provides singleton instances per scope key.                               | [ScopedSingleton](Specialized/ScopedSingleton.md)               | 1.0.0.0         |                 |
| SingletonRegistry      | Central registry for managing singleton instances by type.                | [SingletonRegistry](Specialized/SingletonRegistry.md)           | 1.0.0.0         |                 |
| ThreadLocalSingleton   | Singleton instance per thread.                                            | [ThreadLocalSingleton](Specialized/ThreadLocalSingleton.md)     | 1.0.0.0         |                 |
| WeakSingleton          | Singleton with weak reference, allows GC if no strong refs exist.         | [WeakSingleton](Specialized/WeakSingleton.md)                   | 1.0.0.0         |                 |

---

Click a singleton type above to view its detailed documentation and usage examples. 
