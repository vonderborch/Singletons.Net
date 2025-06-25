using System.Collections.Concurrent;

namespace Singletons.Net.Specialized;

/// <summary>
///     A generic class that manages multiple singleton instances, each identified by a unique key.
///     Allows for thread-safe retrieval and removal of instances.
/// </summary>
/// <typeparam name="TKey">The type of the key used to uniquely identify instances. Must be non-nullable.</typeparam>
/// <typeparam name="TValue">The type of the singleton instance. Must be a reference type.</typeparam>
public sealed class MultiInstanceSingleton<TKey, TValue>
    where TKey : notnull
    where TValue : class
{
    /// <summary>
    ///     A thread-safe collection that stores singleton instances associated with specific keys.
    ///     The values are lazily initialized and managed using the <see cref="Lazy{T}" /> class.
    /// </summary>
    private static readonly ConcurrentDictionary<TKey, Lazy<TValue>> _instances = new();

    /// <summary>
    ///     Gets or sets the default factory method used to create instances of type <typeparamref name="TValue" />.
    ///     When accessing or creating a singleton instance without specifying a custom factory method, this default factory
    ///     will be used.
    /// </summary>
    public static Func<TValue>? DefaultFactory { get; set; }

    /// <summary>
    ///     Retrieves the singleton instance associated with the specified key, creating it if it does not already exist.
    /// </summary>
    /// <param name="key">The key associated with the singleton instance.</param>
    /// <param name="instanceFactory">
    ///     An optional function to create the instance if it does not exist. If not provided, the
    ///     default factory is used.
    /// </param>
    /// <returns>The singleton instance associated with the specified key.</returns>
    /// <exception cref="InvalidOperationException">Thrown if no factory is provided and the default factory is not set.</exception>
    public static TValue GetInstance(TKey key, Func<TValue>? instanceFactory = null)
    {
        instanceFactory ??= DefaultFactory;
        if (instanceFactory == null)
        {
            throw new InvalidOperationException("No instance factory provided");
        }

        return _instances.GetOrAdd(key, k => new Lazy<TValue>(instanceFactory!)).Value;
    }

    /// <summary>
    ///     Removes the instance associated with the specified key from the collection of singleton instances.
    /// </summary>
    /// <param name="key">The key associated with the instance to be removed.</param>
    public static void RemoveInstance(TKey key)
    {
        _instances.TryRemove(key, out _);
    }
}
