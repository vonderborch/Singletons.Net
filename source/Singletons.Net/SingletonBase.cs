namespace Singletons.Net;

/// <summary>
///     Provides a thread-safe implementation of the singleton design pattern that can be used as a base class.
/// </summary>
/// <typeparam name="T">The specific type that must inherit from SingletonBase.</typeparam>
public abstract class SingletonBase<T> where T : SingletonBase<T>
{
    /// <summary>
    ///     Initializes and provides a mechanism to create an instance of the derived type <typeparamref name="T" />.
    ///     Uses reflection to create a new instance of the specified type with private constructors.
    /// </summary>
    protected static Func<T> Factory = () => (Activator.CreateInstance(typeof(T), true) as T)!;

    /// <summary>
    ///     Provides a lazily initialized, thread-safe instance of type <typeparamref name="T" />.
    ///     Ensures the instance is only created when accessed for the first time, following the singleton pattern.
    /// </summary>
    protected static Lazy<T> Lazy = new(Factory);

    /// <summary>
    ///     Gets the singleton instance of the derived class.
    /// </summary>
    public static T Instance => Lazy.Value;
}
