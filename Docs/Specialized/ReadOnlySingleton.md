# ReadOnlySingleton

## Overview
Provides a singleton instance that is immutable after creation. Once set or accessed, the instance cannot be changed.

## When to Use
- When you want a singleton that cannot be changed after first use.
- For configuration or immutable state objects.

## Usage Example
```csharp
// Set the singleton instance (must be before first access)
ReadOnlySingleton<MyConfig>.SetInstance(new MyConfig());

// Access the singleton instance
var instance = ReadOnlySingleton<MyConfig>.Instance;
```

## API Notes
- Throws if you try to set the instance after it has been set or accessed.
- The type parameter must have a parameterless constructor. 