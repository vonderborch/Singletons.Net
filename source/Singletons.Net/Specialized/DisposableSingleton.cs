namespace Singletons.Net.Specialized;

/// <summary>
///     Provides a singleton instance that implements IDisposable and can be reset/disposed.
/// </summary>
/// <typeparam name="T">
///     The type of the singleton instance. Must implement IDisposable and have a parameterless
///     constructor.
/// </typeparam>
public sealed class DisposableSingleton<T> where T : class, IDisposable, new()
{
    /// <summary>
    ///     Holds the singleton instance of the specified type. It is lazily initialized and may be null if not yet accessed.
    /// </summary>
    private static T? _instance;

    /// <summary>
    ///     A synchronization object used to ensure thread-safe access to the singleton instance.
    /// </summary>
    private static readonly object Lock = new();

    /// <summary>
    ///     Provides a singleton instance that implements IDisposable and can be reset/disposed.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the singleton instance. Must implement IDisposable and have a parameterless
    ///     constructor.
    /// </typeparam>
    static DisposableSingleton()
    {
    }

    /// <summary>
    ///     Gets the singleton instance. If disposed, a new instance is created.
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
                }

                return _instance;
            }
        }
    }

    /// <summary>
    ///     Disposes and resets the singleton instance.
    /// </summary>
    public static void Reset()
    {
        lock (Lock)
        {
            _instance?.Dispose();
            _instance = null;
        }
    }
}
