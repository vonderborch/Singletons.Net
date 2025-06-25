namespace Singletons.Net.Specialized;

/// <summary>
///     Represents a thread-safe generic Singleton implementation for a specified type.
/// </summary>
/// <typeparam name="T">
///     The type for which the Singleton pattern is implemented.
/// </typeparam>
public sealed class GenericSingleton<T>
{
    /// <summary>
    ///     Holds the lazily-initialized singleton instance for the specified type.
    ///     This field is used internally to store the instance created or assigned for the singleton.
    /// </summary>
    private static Lazy<T>? _instance;

    /// <summary>
    ///     Indicates whether the singleton instance has been initialized.
    ///     This property is used internally to check if the singleton instance for the specified type has already been created
    ///     or assigned.
    /// </summary>
    private static bool IsInitialized => _instance is not null;

    /// <summary>
    ///     Provides access to the singleton instance of the specified type.
    ///     If the instance has not been initialized, attempting to get it will throw an exception.
    ///     Setting the instance is only allowed if it has not already been initialized.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when attempting to get the instance before it has been initialized,
    ///     or when attempting to set the instance after it has already been initialized.
    /// </exception>
    public static T Instance
    {
        get
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException($"Singleton instance for type {typeof(T)} not set");
            }

            return _instance!.Value;
        }
        set
        {
            if (IsInitialized)
            {
                throw new InvalidOperationException($"Singleton instance for type {typeof(T)} already set");
            }

            _instance = new Lazy<T>(() => value);
        }
    }

    /// <summary>
    ///     Configures the singleton instance with the given source instance.
    ///     Throws an exception if the instance is already initialized, unless overwrite is specified.
    /// </summary>
    /// <param name="sourceInstance">
    ///     The instance to set as the singleton's instance. Cannot be null.
    /// </param>
    /// <param name="overwrite">
    ///     Optional boolean indicating if the existing singleton instance should be overwritten.
    ///     Defaults to false.
    /// </param>
    /// <returns>
    ///     A new instance of the Singleton class of the specified type.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the singleton instance is already initialized and overwrite is not set to true.
    /// </exception>
    public static GenericSingleton<T> FromInstance(T sourceInstance, bool overwrite = false)
    {
        if (IsInitialized && !overwrite)
        {
            throw new InvalidOperationException($"Singleton instance for type {typeof(T)} already set");
        }

        ArgumentNullException.ThrowIfNull(sourceInstance, nameof(sourceInstance));
        _instance = new Lazy<T>(() => sourceInstance);
        return new GenericSingleton<T>();
    }

    /// <summary>
    ///     Configures the singleton instance with the given factory function.
    ///     Throws an exception if the instance is already initialized, unless overwrite is specified.
    /// </summary>
    /// <param name="factory">
    ///     The factory function to create the singleton instance. Cannot be null.
    /// </param>
    /// <param name="overwrite">
    ///     Optional boolean indicating if the existing singleton instance should be overwritten.
    ///     Defaults to false.
    /// </param>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the singleton instance is already initialized and overwrite is not set to true.
    /// </exception>
    public static void SetFactory(Func<T> factory, bool overwrite = false)
    {
        if (IsInitialized && !overwrite)
        {
            throw new InvalidOperationException($"Singleton instance for type {typeof(T)} already set");
        }

        _instance = new Lazy<T>(factory);
    }
}
