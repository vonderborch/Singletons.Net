namespace Singletons.Net.Specialized;

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Represents a thread-safe singleton factory that provides scoped singleton
/// instances based on a specified scope key. Each scope key maps to a single
/// unique instance of the class.
/// </summary>
/// <typeparam name="T">The type of the singleton instance.</typeparam>
public class ScopedSingleton<T> where T : class
{
    /// <summary>
    /// Stores and manages singleton instances that are scoped to a particular key.
    /// This dictionary maps scope keys to their corresponding singleton instances,
    /// ensuring unique instances for each scope key.
    /// </summary>
    private readonly ConcurrentDictionary<object, T> _instances = new();

    /// <summary>
    /// A semaphore used to synchronize access to critical sections of code
    /// in the <see cref="ScopedSingleton{T}"/> class. This ensures thread-safe
    /// creation and management of singleton instances for a given scope.
    /// </summary>
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    /// <summary>
    /// Represents the default asynchronous factory method used to create
    /// new singleton instances when none exist for a specific scope key.
    /// </summary>
    public static Func<T>? DefaultFactory { get; set; }

    /// <summary>
    /// Provides functionality for managing thread-safe scoped singleton instances associated with specific keys.
    /// Each unique scope key corresponds to one singleton instance.
    /// </summary>
    /// <typeparam name="T">The type of the singleton instance.</typeparam>
    static ScopedSingleton()
    {
    }

    /// <summary>
    /// Gets the singleton instance associated with the specified scope key.
    /// </summary>
    /// <param name="scopeKey">The key that defines the scope for this singleton instance.</param>
    /// <param name="instanceFactory">A factory function to create the instance if it doesn't exist.</param>
    /// <returns>The singleton instance for the specified scope.</returns>
    public T GetInstance(object scopeKey, Func<T>? instanceFactory = null)
    {
        if (scopeKey is null)
        {
            throw new ArgumentNullException(nameof(scopeKey));
        }
        // Fast path - instance already exists
        if (_instances.TryGetValue(scopeKey, out var instance))
        {            
            return instance;
        }

        // Slow path - need to create instance
        _semaphore.Wait();
        try
        {            
            // Double-check locking pattern
            if (_instances.TryGetValue(scopeKey, out instance))
            {                
                return instance;
            }

            // Create new instance
            if (DefaultFactory is null && instanceFactory is null)
            {
                throw new InvalidOperationException("No instance factory provided");
            }

            instanceFactory ??= DefaultFactory;
            instance = instanceFactory();

            this._instances[scopeKey] = instance ?? throw new InvalidOperationException("Factory returned null instance");
            return this._instances[scopeKey];
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Gets the singleton instance associated with the specified scope key asynchronously.
    /// </summary>
    /// <param name="scopeKey">The key that defines the scope for this singleton instance.</param>
    /// <param name="instanceFactory">An asynchronous factory function to create the instance if it doesn't exist.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the singleton instance for the specified scope.</returns>
    public async Task<T> GetInstanceAsync(object scopeKey, Func<Task<T>>? instanceFactory = null)
    {        
        // Fast path - instance already exists
        if (_instances.TryGetValue(scopeKey, out var instance))
        {            
            return instance;
        }

        // Slow path - need to create instance
        await _semaphore.WaitAsync();
        try
        {            
            // Double-check locking pattern
            if (_instances.TryGetValue(scopeKey, out instance))
            {                
                return instance;
            }
            
            // Create new instance
            if (DefaultFactory is null && instanceFactory is null)
            {
                throw new InvalidOperationException("No instance factory provided");
            }
            

            if (instanceFactory is not null)
            {
                instance = await instanceFactory() ?? throw new InvalidOperationException("Factory returned null instance");
            }
            else
            {
                Task<T> AwaitableDefaultFactory() => Task.FromResult(DefaultFactory!());
                instance = await AwaitableDefaultFactory() ?? throw new InvalidOperationException("Default factory returned null instance");
            }
            this._instances[scopeKey] = instance;
            return this._instances[scopeKey];
        }
        finally
        {            
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Removes the singleton instance associated with the specified scope key.
    /// </summary>
    /// <param name="scopeKey">The key that defines the scope for the singleton instance to remove.</param>
    /// <returns>True if the instance was successfully removed; otherwise, false.</returns>
    public bool RemoveInstance(object scopeKey)
    {        
        return _instances.TryRemove(scopeKey, out _);
    }

    /// <summary>
    /// Checks if a singleton instance exists for the specified scope key.
    /// </summary>
    /// <param name="scopeKey">The key that defines the scope to check.</param>
    /// <returns>True if an instance exists for the specified scope; otherwise, false.</returns>
    public bool HasInstance(object scopeKey)
    {        
        return _instances.ContainsKey(scopeKey);
    }

    /// <summary>
    /// Clears all singleton instances from all scopes.
    /// </summary>
    public void ClearAllInstances()
    {        
        _semaphore.Wait();
        try
        {            
            _instances.Clear();
        }
        finally
        {            
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Clears all singleton instances from all scopes asynchronously.
    /// </summary>
    public async Task ClearAllInstancesAsync()
    {        
        await _semaphore.WaitAsync();
        try
        {            
            _instances.Clear();
        }
        finally
        {            
            _semaphore.Release();
        }
    }
}
