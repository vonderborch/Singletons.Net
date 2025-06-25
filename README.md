# Singletons.Net

![Logo](https://raw.githubusercontent.com/vonderborch/Singletons.Net/refs/heads/main/logo.png)

A library containing a variety of Singleton approaches that can be leveraged.

## Installation

### Nuget

[![NuGet version (Singletons.Net)](https://img.shields.io/nuget/v/Singletons.Net.svg?style=flat-square)](https://www.nuget.org/packages/Singletons.Net/)

The recommended installation approach is to use the available nuget
package: [Singletons.Net](https://www.nuget.org/packages/Singletons.Net/)

### Clone

Alternatively, you can clone this repo and reference the Singletons.Net project in your project.

## Features

- A variety of different Singletons that can be used in a .Net project
- A base singleton class that can be used to implement new singletons

## Available Singletons

| Singleton Type         | Description                                                               | Documentation                                                        | Minimum Version | Maximum Version |
|------------------------|---------------------------------------------------------------------------|----------------------------------------------------------------------|-----------------|-----------------|
| SingletonBase          | Base class for implementing thread-safe, lazy singletons via inheritance. | [SingletonBase](Docs/Singletons/SingletonBase.md)                    | 1.0.0.0         |                 |
| AsyncSingleton         | Async, thread-safe singleton for types needing async initialization.      | [AsyncSingleton](Docs/Specialized/AsyncSingleton.md)                 | 1.0.0.0         |                 |
| DisposableSingleton    | Singleton for IDisposable types, supports reset/dispose.                  | [DisposableSingleton](Docs/Specialized/DisposableSingleton.md)       | 1.0.0.0         |                 |
| GenericSingleton       | Thread-safe, generic singleton for any reference type.                    | [GenericSingleton](Docs/Specialized/GenericSingleton.md)             | 1.0.0.0         |                 |
| MultiInstanceSingleton | Manages multiple singleton instances, each identified by a unique key.    | [MultiInstanceSingleton](Docs/Specialized/MultiInstanceSingleton.md) | 1.0.0.0         |                 |
| ReadOnlySingleton      | Singleton that is immutable after creation.                               | [ReadOnlySingleton](Docs/Specialized/ReadOnlySingleton.md)           | 1.0.0.0         |                 |
| ResettableSingleton    | Singleton that can be explicitly reset.                                   | [ResettableSingleton](Docs/Specialized/ResettableSingleton.md)       | 1.0.0.0         |                 |
| ScopedSingleton        | Provides singleton instances per scope key.                               | [ScopedSingleton](Docs/Specialized/ScopedSingleton.md)               | 1.0.0.0         |                 |
| SingletonRegistry      | Central registry for managing singleton instances by type.                | [SingletonRegistry](Docs/Specialized/SingletonRegistry.md)           | 1.0.0.0         |                 |
| ThreadLocalSingleton   | Singleton instance per thread.                                            | [ThreadLocalSingleton](Docs/Specialized/ThreadLocalSingleton.md)     | 1.0.0.0         |                 |
| WeakSingleton          | Singleton with weak reference, allows GC if no strong refs exist.         | [WeakSingleton](Docs/Specialized/WeakSingleton.md)                   | 1.0.0.0         |                 |

## Development

1. Clone or fork the repo
2. Create a new branch
3. Code!
4. Push your changes and open a PR
5. Once approved, they'll be merged in
6. Profit!

## Future Plans

See list of issues under the Milestones: https://github.com/vonderborch/Singletons.Net/milestones
