# ScopedSingleton

## Overview
Provides thread-safe singleton instances based on a specified scope key. Each unique scope key maps to a single unique instance.

## When to Use
- When you need a singleton per scope (e.g., per user, per request, per tenant).
- When you want to manage lifetimes of objects tied to a specific context.

## Usage Example
```csharp
var scopedSingleton = new ScopedSingleton<MyService>();

// Get or create a singleton instance for a specific scope
var instance = scopedSingleton.GetInstance("user1", () => new MyService());

// Remove a singleton instance for a scope
scopedSingleton.RemoveInstance("user1");
```

## API Notes
- Each scope key gets its own singleton instance.
- You can provide a default factory for all scopes via `DefaultFactory`.
- Thread-safe for concurrent access. 