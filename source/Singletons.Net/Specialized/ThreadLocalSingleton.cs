namespace Singletons.Net.Specialized;

using System.Threading;

/// <summary>
/// Provides a singleton instance per thread for the specified type.
/// </summary>
/// <typeparam name="T">The type of the singleton instance. Must be a reference type with a parameterless constructor.</typeparam>
public sealed class ThreadLocalSingleton<T> where T : class, new()
{
    /// <summary>
    /// Stores the thread-local singleton instance for the specified type.
    /// </summary>
    private static readonly ThreadLocal<T> _instance = new(() => new T());

    /// <summary>
    /// Provides a thread-local singleton implementation for ensuring that each thread has its own unique instance
    /// of the specified type. The specified type must be a reference type with a public, parameterless constructor.
    /// </summary>
    /// <typeparam name="T">
    /// The type for which a singleton instance will be provided at a thread-local scope. Must be a class with a parameterless constructor.
    /// </typeparam>
    static ThreadLocalSingleton()
    {
    }

    /// <summary>
    /// Gets the singleton instance for the current thread.
    /// </summary>
    public static T Instance => _instance.Value!;
} 
