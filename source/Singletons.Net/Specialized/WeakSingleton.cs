namespace Singletons.Net.Specialized;

/// <summary>
///     Provides a weakly referenced singleton implementation for the specified type.
///     This class ensures that only one instance of the specified type is created and retained,
///     but the instance can be garbage collected if no strong references are present.
/// </summary>
/// <typeparam name="T">The type of the singleton instance. Must be a reference type.</typeparam>
public class WeakSingleton<T> where T : class
{
    /// <summary>
    ///     A static weak reference to the singleton instance of type <typeparamref name="T" />.
    ///     This allows the instance to be garbage collected if no strong references to it exist,
    ///     ensuring that resources are freed when the singleton instance is no longer in use.
    /// </summary>
    private static WeakReference<T>? _weakInstance;

    /// <summary>
    ///     A static synchronization object used to ensure thread safety when accessing
    ///     or creating the singleton instance of type <typeparamref name="T" />.
    ///     Prevents race conditions during instance initialization.
    /// </summary>
    private static readonly object _lock = new();

    /// <summary>
    ///     Gets or sets the default factory delegate used to create an instance of type <typeparamref name="T" />
    ///     when no explicit factory is provided. This property is utilized as a fallback mechanism
    ///     to generate the singleton instance, ensuring that the required instance is available.
    /// </summary>
    public static Func<T>? DefaultFactory { protected get; set; }

    /// <summary>
    ///     Retrieves the singleton instance of type <typeparamref name="T" />. If the instance is not already created
    ///     or has been garbage collected, it creates a new instance using the provided factory delegate or the
    ///     default factory delegate.
    /// </summary>
    /// <param name="factory">
    ///     An optional factory delegate used to create the instance of type <typeparamref name="T" />.
    ///     If this is null, the DefaultFactory delegate is used instead. If both are null, an exception is thrown.
    /// </param>
    /// <returns>
    ///     The singleton instance of type <typeparamref name="T" />.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if no factory is provided and the DefaultFactory delegate is null.
    /// </exception>
    public static T GetInstance(Func<T>? factory = null)
    {
        if (_weakInstance?.TryGetTarget(out T? instance) == true)
        {
            return instance;
        }

        lock (_lock)
        {
            if (_weakInstance?.TryGetTarget(out instance) == true)
            {
                return instance;
            }

            if (DefaultFactory is null && factory is null)
            {
                throw new InvalidOperationException("No instance factory provided");
            }

            factory ??= DefaultFactory;
            instance = factory!() ?? throw new InvalidOperationException("Factory returned null instance");
            _weakInstance = new WeakReference<T>(instance);
            return instance;
        }
    }
}
