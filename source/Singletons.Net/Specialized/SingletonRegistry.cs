using System.Collections.Concurrent;

namespace Singletons.Net.Specialized;

/// <summary>
///     Central registry for managing singleton instances by type.
/// </summary>
public static class SingletonRegistry
{
    /// <summary>
    ///     A thread-safe storage for associating types with their corresponding singleton
    ///     instances. Acts as the underlying dictionary for managing singletons within
    ///     the <see cref="SingletonRegistry" />.
    /// </summary>
    private static readonly ConcurrentDictionary<Type, object> Instances = new();

    /// <summary>
    ///     Clears all registered singleton instances.
    /// </summary>
    public static void Clear()
    {
        Instances.Clear();
    }

    /// <summary>
    ///     Gets the singleton instance for the specified type, or throws if not registered.
    /// </summary>
    public static T Get<T>() where T : class
    {
        if (Instances.TryGetValue(typeof(T), out var value) && value is T t)
        {
            return t;
        }

        throw new InvalidOperationException($"No singleton registered for type {typeof(T)}");
    }

    /// <summary>
    ///     Registers a singleton instance for the specified type.
    /// </summary>
    public static void Register<T>(T instance) where T : class
    {
        Instances[typeof(T)] = instance ?? throw new ArgumentNullException(nameof(instance));
    }

    /// <summary>
    ///     Removes the singleton instance for the specified type.
    /// </summary>
    public static void Remove<T>() where T : class
    {
        Instances.TryRemove(typeof(T), out _);
    }

    /// <summary>
    ///     Tries to get the singleton instance for the specified type.
    /// </summary>
    public static bool TryGet<T>(out T? instance) where T : class
    {
        if (Instances.TryGetValue(typeof(T), out var value) && value is T t)
        {
            instance = t;
            return true;
        }

        instance = null;
        return false;
    }
}
