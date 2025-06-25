namespace Singletons.Net.Specialized;

/// <summary>
///     Provides a singleton instance that can be explicitly reset.
/// </summary>
/// <typeparam name="T">The type of the singleton instance. Must have a parameterless constructor.</typeparam>
public class ResettableSingleton<T> where T : class, new()
{
    /// <summary>
    ///     Holds the singleton instance of the specified type. This field is used to manage the
    ///     lifecycle of the instance and supports resetting the instance when required.
    /// </summary>
    private static T? _instance;

    /// <summary>
    ///     An object used to synchronize access to the singleton instance, ensuring thread safety.
    /// </summary>
    private static readonly object Lock = new();

    /// <summary>
    ///     Provides a singleton instance that can be explicitly reset.
    /// </summary>
    /// <typeparam name="T">The type of the singleton instance. Must have a parameterless constructor.</typeparam>
    static ResettableSingleton()
    {
    }

    /// <summary>
    ///     Gets the singleton instance.
    /// </summary>
    public static T Instance
    {
        get
        {
            lock (Lock)
            {
                return _instance ??= new T();
            }
        }
    }

    /// <summary>
    ///     Resets the singleton instance. A new instance will be created on next access.
    /// </summary>
    public static void Reset()
    {
        lock (Lock)
        {
            _instance = null;
        }
    }
}
