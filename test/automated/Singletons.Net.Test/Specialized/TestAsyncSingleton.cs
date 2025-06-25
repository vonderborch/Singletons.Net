using System.Collections.Concurrent;
using System.Reflection;
using Singletons.Net.Specialized;

namespace Singletons.Net.Test.Specialized;

[TestFixture]
public class TestAsyncSingleton
{
    [SetUp]
    public void Setup()
    {
        // Reset any static state between tests
        ResetSingletonState();
    }

    [Test]
    public async Task AsyncSingleton_GetInstanceAsync_ShouldReturnSameInstance()
    {
        // Arrange
        AsyncSingleton<TestObject>.DefaultFactory = () => Task.FromResult(new TestObject { Id = 1, Name = "Test" });

        // Act
        TestObject instance1 = await AsyncSingleton<TestObject>.GetInstanceAsync();
        TestObject instance2 = await AsyncSingleton<TestObject>.GetInstanceAsync();

        // Assert
        Assert.That(instance1, Is.Not.Null);
        Assert.That(instance2, Is.Not.Null);
        Assert.That(instance1, Is.SameAs(instance2));
    }

    [Test]
    public async Task AsyncSingleton_GetInstanceAsync_WithFactory_ShouldReturnSameInstance()
    {
        // Arrange
        var factoryCallCount = 0;
        Func<Task<TestObject>> factory = async () =>
        {
            factoryCallCount++;
            await Task.Delay(10); // Simulate async work
            return new TestObject { Id = factoryCallCount, Name = $"Test-{factoryCallCount}" };
        };

        // Act
        TestObject instance1 = await AsyncSingleton<TestObject>.GetInstanceAsync(factory);
        TestObject instance2 = await AsyncSingleton<TestObject>.GetInstanceAsync(factory);

        // Assert
        Assert.That(instance1, Is.Not.Null);
        Assert.That(instance2, Is.Not.Null);
        Assert.That(instance1, Is.SameAs(instance2));
        Assert.That(factoryCallCount, Is.EqualTo(1), "Factory should be called exactly once");
    }

    [Test]
    public async Task AsyncSingleton_GetInstanceAsync_ShouldBeLazyInitialized()
    {
        // Arrange
        var factoryCallCount = 0;
        Func<Task<TestObject>> factory = async () =>
        {
            factoryCallCount++;
            await Task.Delay(10);
            return new TestObject { Id = factoryCallCount, Name = $"Test-{factoryCallCount}" };
        };

        // Act
        TestObject instance = await AsyncSingleton<TestObject>.GetInstanceAsync(factory);

        // Assert
        Assert.That(instance, Is.Not.Null);
        Assert.That(factoryCallCount, Is.EqualTo(1), "Factory should be called exactly once");
    }

    [Test]
    public async Task AsyncSingleton_GetInstanceAsync_ShouldBeThreadSafe()
    {
        // Arrange
        var factoryCallCount = 0;
        var lockObject = new object();
        Func<Task<TestObject>> factory = async () =>
        {
            lock (lockObject)
            {
                factoryCallCount++;
            }

            await Task.Delay(10);
            return new TestObject { Id = factoryCallCount, Name = $"Test-{factoryCallCount}" };
        };

        ConcurrentBag<TestObject> results = new();

        // Act
        await Task.WhenAll(Enumerable.Range(0, 10).Select(async _ =>
        {
            TestObject instance = await AsyncSingleton<TestObject>.GetInstanceAsync(factory);
            results.Add(instance);
        }));

        // Assert
        TestObject firstInstance = results.First();
        Assert.That(firstInstance, Is.Not.Null);
        Assert.That(results.Count, Is.EqualTo(10));
        Assert.That(results.All(x => ReferenceEquals(x, firstInstance)), Is.True,
            "All instances should be the same across threads");
        Assert.That(factoryCallCount, Is.EqualTo(1),
            "Factory should be called exactly once even with concurrent access");
    }

    [Test]
    public async Task AsyncSingleton_GetInstanceAsync_ShouldThrowExceptionWhenNoFactoryProvided()
    {
        // Act & Assert
        InvalidOperationException? exception = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await AsyncSingleton<TestObject>.GetInstanceAsync());

        Assert.That(exception.Message, Is.EqualTo("No instance factory provided"));
    }

    [Test]
    public async Task AsyncSingleton_GetInstanceAsync_ShouldThrowExceptionWhenFactoryReturnsNull()
    {
        // Arrange
        Func<Task<TestObject>> factory = async () =>
        {
            await Task.Delay(10);
            return null!;
        };

        // Act & Assert
        InvalidOperationException? exception = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await AsyncSingleton<TestObject>.GetInstanceAsync(factory));

        Assert.That(exception.Message, Is.EqualTo("Factory returned null instance"));
    }

    [Test]
    public async Task AsyncSingleton_GetInstanceAsync_ShouldHandleComplexAsyncOperations()
    {
        // Arrange
        Func<Task<ComplexTestObject>> factory = async () =>
        {
            await Task.Delay(10); // Simulate async work
            return new ComplexTestObject
            {
                Id = 1,
                Name = "Complex Test",
                Properties = new Dictionary<string, object>
                {
                    { "Key1", "Value1" },
                    { "Key2", 42 }
                }
            };
        };

        // Act
        ComplexTestObject instance = await AsyncSingleton<ComplexTestObject>.GetInstanceAsync(factory);

        // Assert
        Assert.That(instance, Is.Not.Null);
        Assert.That(instance.Id, Is.EqualTo(1));
        Assert.That(instance.Name, Is.EqualTo("Complex Test"));
        Assert.That(instance.Properties["Key1"], Is.EqualTo("Value1"));
        Assert.That(instance.Properties["Key2"], Is.EqualTo(42));
    }

    [Test]
    public async Task AsyncSingleton_GetInstanceAsync_ShouldHandleExceptionInFactory()
    {
        // Arrange
        Func<Task<TestObject>> factory = async () =>
        {
            await Task.Delay(10);
            throw new InvalidOperationException("Factory exception");
        };

        // Act & Assert
        InvalidOperationException? exception = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await AsyncSingleton<TestObject>.GetInstanceAsync(factory));

        Assert.That(exception.Message, Is.EqualTo("Factory exception"));
    }

    [Test]
    public async Task AsyncSingleton_GetInstanceAsync_ShouldHandleLongRunningAsyncOperations()
    {
        // Arrange
        var factoryCallCount = 0;
        Func<Task<TestObject>> factory = async () =>
        {
            factoryCallCount++;
            await Task.Delay(100); // Simulate longer async work
            return new TestObject { Id = factoryCallCount, Name = $"Test-{factoryCallCount}" };
        };

        // Act
        Task<TestObject>[] tasks = Enumerable.Range(0, 5).Select(async _ =>
            await AsyncSingleton<TestObject>.GetInstanceAsync(factory)).ToArray();

        TestObject[] instances = await Task.WhenAll(tasks);

        // Assert
        TestObject firstInstance = instances.First();
        Assert.That(instances.All(x => ReferenceEquals(x, firstInstance)), Is.True, "All instances should be the same");
        Assert.That(factoryCallCount, Is.EqualTo(1), "Factory should be called exactly once");
    }

    [Test]
    public async Task AsyncSingleton_GetInstanceAsync_ShouldWorkWithDefaultFactory()
    {
        // Arrange
        AsyncSingleton<TestObject>.DefaultFactory =
            () => Task.FromResult(new TestObject { Id = 1, Name = "Default Test" });

        // Act
        TestObject instance1 = await AsyncSingleton<TestObject>.GetInstanceAsync();
        TestObject instance2 = await AsyncSingleton<TestObject>.GetInstanceAsync();

        // Assert
        Assert.That(instance1, Is.Not.Null);
        Assert.That(instance2, Is.Not.Null);
        Assert.That(instance1, Is.SameAs(instance2));
        Assert.That(instance1.Id, Is.EqualTo(1));
        Assert.That(instance1.Name, Is.EqualTo("Default Test"));
    }

    [Test]
    public async Task AsyncSingleton_GetInstanceAsync_ShouldPreferProvidedFactoryOverDefaultFactory()
    {
        // Arrange
        AsyncSingleton<TestObject>.DefaultFactory =
            () => Task.FromResult(new TestObject { Id = 1, Name = "Default Test" });

        Func<Task<TestObject>> providedFactory = async () =>
        {
            await Task.Delay(10);
            return new TestObject { Id = 2, Name = "Provided Test" };
        };

        // Act
        TestObject instance = await AsyncSingleton<TestObject>.GetInstanceAsync(providedFactory);

        // Assert
        Assert.That(instance, Is.Not.Null);
        Assert.That(instance.Id, Is.EqualTo(2));
        Assert.That(instance.Name, Is.EqualTo("Provided Test"));
    }

    private void ResetSingletonState()
    {
        // Use reflection to reset the private static field
        FieldInfo? field = typeof(AsyncSingleton<TestObject>).GetField("_instance",
            BindingFlags.NonPublic | BindingFlags.Static);
        field?.SetValue(null, null);

        FieldInfo? field2 = typeof(AsyncSingleton<ComplexTestObject>).GetField("_instance",
            BindingFlags.NonPublic | BindingFlags.Static);
        field2?.SetValue(null, null);

        // Reset DefaultFactory
        PropertyInfo? defaultFactoryProperty = typeof(AsyncSingleton<TestObject>).GetProperty("DefaultFactory",
            BindingFlags.Public | BindingFlags.Static);
        defaultFactoryProperty?.SetValue(null, null);

        PropertyInfo? defaultFactoryProperty2 = typeof(AsyncSingleton<ComplexTestObject>).GetProperty("DefaultFactory",
            BindingFlags.Public | BindingFlags.Static);
        defaultFactoryProperty2?.SetValue(null, null);
    }

    // Test classes
    public class TestObject
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class ComplexTestObject
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Dictionary<string, object> Properties { get; set; } = new();
    }
}
