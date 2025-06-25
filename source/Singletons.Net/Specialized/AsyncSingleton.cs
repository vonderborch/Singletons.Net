namespace Singletons.Net.Specialized;

/// <summary>
///     Provides a thread-safe, asynchronous, and lazy-loaded singleton implementation for a given type.
///     This can be used in cases where a singleton needs async initialization (database connections, file loading, API
///     calls, etc.).
/// </summary>
/// <typeparam name="T">The type to serve as a singleton. Must be a reference type with a parameterless constructor.</typeparam>
public sealed class AsyncSingleton<T> where T : class, new()
{
    /// <summary>
    ///     Provides a lightweight synchronization primitive to control access to a resource or pool of resources in an
    ///     asynchronous, thread-safe manner within the <see cref="AsyncSingleton{T}" /> implementation.
    /// </summary>
    private static readonly SemaphoreSlim Semaphore = new(1, 1);

    /// <summary>
    ///     Stores the singleton instance of type <typeparamref name="T" /> for the <see cref="AsyncSingleton{T}" /> class.
    /// </summary>
    private static T? _instance;

    /// <summary>
    ///     Provides a thread-safe, asynchronous, and lazy-loaded singleton instance for a specified type
    ///     <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">
    ///     The type to serve as a singleton. Must be a reference type with a parameterless constructor.
    /// </typeparam>
    static AsyncSingleton()
    {
    }

    /// <summary>
    ///     Represents a property that can be set to provide a custom asynchronous factory method for creating
    ///     the singleton instance of type <typeparamref name="T" />.
    ///     This property is optional and can be used to override the default parameterless constructor for the type.
    /// </summary>
    public static Func<Task<T>>? DefaultFactory { get; set; }

    /// <summary>
    ///     Retrieves the singleton instance of type <typeparamref name="T" /> asynchronously, ensuring thread safety
    ///     and optional lazy initialization using a provided instance factory.
    /// </summary>
    /// <param name="instanceFactory">
    ///     An optional factory function to asynchronously create the singleton instance of type <typeparamref name="T" />.
    ///     If not provided, the method will expect that an instance factory has been set previously.
    /// </param>
    /// <returns>
    ///     A task representing the asynchronous operation, which on completion contains the singleton instance of type
    ///     <typeparamref name="T" />.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when no valid instance factory is provided and the instance has not already been initialized.
    /// </exception>
    public static async Task<T> GetInstanceAsync(Func<Task<T>>? instanceFactory = null)
    {
        if (_instance != null)
        {
            return _instance;
        }

        await Semaphore.WaitAsync();
        try
        {
            if (_instance != null)
            {
                return _instance;
            }

            if (DefaultFactory is null && instanceFactory is null)
            {
                throw new InvalidOperationException("No instance factory provided");
            }

            Task<T> task = instanceFactory is not null ? instanceFactory() : DefaultFactory!();
            _instance = await task ?? throw new InvalidOperationException("Factory returned null instance");
            return _instance;
        }
        finally
        {
            Semaphore.Release();
        }
    }
}
