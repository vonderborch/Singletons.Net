namespace Singletons.Net.Specialized;

using System;

/// <summary>
/// Provides a singleton instance that is immutable after creation.
/// </summary>
/// <typeparam name="T">The type of the singleton instance. Must have a parameterless constructor.</typeparam>
public sealed class ReadOnlySingleton<T> where T : class, new()
{
    /// <summary>
    /// Holds the singleton instance of type <typeparamref name="T"/>. The instance is
    /// immutable after being set or accessed for the first time.
    /// </summary>
    private static T? _instance;

    /// <summary>
    /// Serves as a synchronization mechanism to ensure thread safety when
    /// accessing or setting the singleton instance.
    /// </summary>
    private static readonly Lock Lock = new();

    /// <summary>
    /// Indicates whether the singleton instance has been set or accessed.
    /// </summary>
    private static bool IsInitialized;

    /// <summary>
    /// Provides a singleton instance that is immutable after creation.
    /// </summary>
    /// <typeparam name="T">The type of the singleton instance. Must have a parameterless constructor.</typeparam>
    static ReadOnlySingleton()
    {
    }

    /// <summary>
    /// Gets the singleton instance. If not set, creates a new instance.
    /// </summary>
    public static T Instance
    {
        get
        {
            lock (Lock)
            {
                if (_instance == null)
                {
                    _instance = new T();
                    IsInitialized = true;
                }
                return _instance;
            }
        }
    }

    /// <summary>
    /// Sets the singleton instance, but only if it has not been set or accessed yet.
    /// </summary>
    /// <param name="value">The value to set as the singleton instance.</param>
    public static void SetInstance(T value)
    {
        lock (Lock)
        {
            if (IsInitialized)
            {
                throw new InvalidOperationException("ReadOnlySingleton instance has already been set or accessed.");
            }
            _instance = value ?? throw new ArgumentNullException(nameof(value));
            IsInitialized = true;
        }
    }
} 
