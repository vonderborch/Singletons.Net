using System.Collections.Concurrent;
using System.Reflection;
using Singletons.Net.Specialized;

namespace Singletons.Net.Test.Specialized;

[TestFixture]
public class TestWeakSingleton
{
    [SetUp]
    public void Setup()
    {
        // Reset any static state between tests
        ResetSingletonState();
    }

    [Test]
    public void WeakSingleton_GetInstance_ShouldReturnSameInstance()
    {
        // Arrange
        WeakSingleton<TestObject>.DefaultFactory = () => new TestObject { Id = 1, Name = "Test" };

        // Act
        TestObject instance1 = WeakSingleton<TestObject>.GetInstance();
        TestObject instance2 = WeakSingleton<TestObject>.GetInstance();

        // Assert
        Assert.That(instance1, Is.Not.Null);
        Assert.That(instance2, Is.Not.Null);
        Assert.That(instance1, Is.SameAs(instance2));
    }

    [Test]
    public void WeakSingleton_GetInstance_WithFactory_ShouldReturnSameInstance()
    {
        // Arrange
        var factoryCallCount = 0;
        Func<TestObject> factory = () =>
        {
            factoryCallCount++;
            return new TestObject { Id = factoryCallCount, Name = $"Test-{factoryCallCount}" };
        };

        // Act
        TestObject instance1 = WeakSingleton<TestObject>.GetInstance(factory);
        TestObject instance2 = WeakSingleton<TestObject>.GetInstance(factory);

        // Assert
        Assert.That(instance1, Is.Not.Null);
        Assert.That(instance2, Is.Not.Null);
        Assert.That(instance1, Is.SameAs(instance2));
        Assert.That(factoryCallCount, Is.EqualTo(1), "Factory should be called exactly once");
    }

    [Test]
    public void WeakSingleton_GetInstance_ShouldBeLazyInitialized()
    {
        // Arrange
        var factoryCallCount = 0;
        Func<TestObject> factory = () =>
        {
            factoryCallCount++;
            return new TestObject { Id = factoryCallCount, Name = $"Test-{factoryCallCount}" };
        };

        // Act
        TestObject instance = WeakSingleton<TestObject>.GetInstance(factory);

        // Assert
        Assert.That(instance, Is.Not.Null);
        Assert.That(factoryCallCount, Is.EqualTo(1), "Factory should be called exactly once");
    }

    [Test]
    public void WeakSingleton_GetInstance_ShouldBeThreadSafe()
    {
        // Arrange
        var factoryCallCount = 0;
        var lockObject = new object();
        Func<TestObject> factory = () =>
        {
            lock (lockObject)
            {
                factoryCallCount++;
            }

            return new TestObject { Id = factoryCallCount, Name = $"Test-{factoryCallCount}" };
        };

        ConcurrentBag<TestObject> results = new();

        // Act
        Parallel.For(0, 10, _ => { results.Add(WeakSingleton<TestObject>.GetInstance(factory)); });

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
    public void WeakSingleton_GetInstance_ShouldThrowExceptionWhenNoFactoryProvided()
    {
        // Act & Assert
        InvalidOperationException? exception = Assert.Throws<InvalidOperationException>(() =>
            WeakSingleton<TestObject>.GetInstance());

        Assert.That(exception.Message, Is.EqualTo("No instance factory provided"));
    }

    [Test]
    public void WeakSingleton_GetInstance_ShouldHandleComplexObjects()
    {
        // Arrange
        Func<ComplexTestObject> factory = () => new ComplexTestObject
        {
            Id = 1,
            Name = "Complex Test",
            Properties = new Dictionary<string, object>
            {
                { "Key1", "Value1" },
                { "Key2", 42 }
            }
        };

        // Act
        ComplexTestObject instance = WeakSingleton<ComplexTestObject>.GetInstance(factory);

        // Assert
        Assert.That(instance, Is.Not.Null);
        Assert.That(instance.Id, Is.EqualTo(1));
        Assert.That(instance.Name, Is.EqualTo("Complex Test"));
        Assert.That(instance.Properties["Key1"], Is.EqualTo("Value1"));
        Assert.That(instance.Properties["Key2"], Is.EqualTo(42));
    }

    [Test]
    public void WeakSingleton_GetInstance_ShouldHandleExceptionInFactory()
    {
        // Arrange
        Func<TestObject> factory = () => throw new InvalidOperationException("Factory exception");

        // Act & Assert
        InvalidOperationException? exception = Assert.Throws<InvalidOperationException>(() =>
            WeakSingleton<TestObject>.GetInstance(factory));

        Assert.That(exception.Message, Is.EqualTo("Factory exception"));
    }

    [Test]
    public void WeakSingleton_GetInstance_ShouldWorkWithDefaultFactory()
    {
        // Arrange
        WeakSingleton<TestObject>.DefaultFactory = () => new TestObject { Id = 1, Name = "Default Test" };

        // Act
        TestObject instance1 = WeakSingleton<TestObject>.GetInstance();
        TestObject instance2 = WeakSingleton<TestObject>.GetInstance();

        // Assert
        Assert.That(instance1, Is.Not.Null);
        Assert.That(instance2, Is.Not.Null);
        Assert.That(instance1, Is.SameAs(instance2));
        Assert.That(instance1.Id, Is.EqualTo(1));
        Assert.That(instance1.Name, Is.EqualTo("Default Test"));
    }

    [Test]
    public void WeakSingleton_GetInstance_ShouldPreferProvidedFactoryOverDefaultFactory()
    {
        // Arrange
        WeakSingleton<TestObject>.DefaultFactory = () => new TestObject { Id = 1, Name = "Default Test" };

        Func<TestObject> providedFactory = () => new TestObject { Id = 2, Name = "Provided Test" };

        // Act
        TestObject instance = WeakSingleton<TestObject>.GetInstance(providedFactory);

        // Assert
        Assert.That(instance, Is.Not.Null);
        Assert.That(instance.Id, Is.EqualTo(2));
        Assert.That(instance.Name, Is.EqualTo("Provided Test"));
    }

    [Test]
    public void WeakSingleton_GetInstance_ShouldAllowGarbageCollection()
    {
        // Use a unique type to avoid static state pollution
        WeakReference weakRef = CreateAndRelease();

        // Force multiple GC collections to ensure cleanup
        GC.Collect(2, GCCollectionMode.Forced, true);
        GC.WaitForPendingFinalizers();
        GC.Collect(2, GCCollectionMode.Forced, true);

        // Assert - Weak reference should be collected
        Assert.That(weakRef.IsAlive, Is.False, "Instance should be garbage collected when no strong references exist");

        // Additional verification - check that _weakInstance in the singleton was also collected
        FieldInfo? field = typeof(WeakSingleton<UniqueTestObject>).GetField("_weakInstance",
            BindingFlags.NonPublic | BindingFlags.Static);
        WeakReference<UniqueTestObject>? weakInstanceField = field?.GetValue(null) as WeakReference<UniqueTestObject>;
        Assert.That(weakInstanceField?.TryGetTarget(out _), Is.False,
            "Singleton's weak reference should not hold a target");

        // Local function to create and release the instance
        static WeakReference CreateAndRelease()
        {
            WeakSingleton<UniqueTestObject>.DefaultFactory = () => new UniqueTestObject();
            WeakReference weakRef;
            {
                UniqueTestObject instance = WeakSingleton<UniqueTestObject>.GetInstance();
                weakRef = new WeakReference(instance);
                Assert.That(weakRef.IsAlive, Is.True);
                Assert.That(instance, Is.Not.Null);
                GC.KeepAlive(instance); // Ensure the reference is alive until here
            }
            return weakRef;
        }
    }

    [Test]
    public void WeakSingleton_GetInstance_ShouldRecreateAfterGarbageCollection()
    {
        var factoryCallCount = 0;
        Func<UniqueTestObject2> factory = () =>
        {
            factoryCallCount++;
            return new UniqueTestObject2 { Id = factoryCallCount };
        };

        UniqueTestObject2? firstInstance;
        {
            firstInstance = WeakSingleton<UniqueTestObject2>.GetInstance(factory);
            Assert.That(firstInstance, Is.Not.Null);
            Assert.That(firstInstance.Id, Is.EqualTo(1));
            GC.KeepAlive(firstInstance);
        }

        // Clear the static _weakInstance field to break any lingering reference
        FieldInfo? field = typeof(WeakSingleton<UniqueTestObject2>).GetField("_weakInstance",
            BindingFlags.NonPublic | BindingFlags.Static);
        field?.SetValue(null, null);

        GC.Collect(2, GCCollectionMode.Forced, true);
        GC.WaitForPendingFinalizers();
        GC.Collect(2, GCCollectionMode.Forced, true);

        // Get a new instance
        UniqueTestObject2 secondInstance = WeakSingleton<UniqueTestObject2>.GetInstance(factory);

        Assert.That(secondInstance, Is.Not.Null, "Second instance should be created");
        Assert.That(secondInstance.Id, Is.EqualTo(2), "Factory should be called again");
        Assert.That(factoryCallCount, Is.EqualTo(2), "Factory should be called twice");
        Assert.That(!ReferenceEquals(firstInstance, secondInstance), "Instances should not be the same");
    }

    [Test]
    public void WeakSingleton_GetInstance_ShouldHandleNullFactory()
    {
        // Arrange
        Func<TestObject>? factory = null;

        // Act & Assert
        InvalidOperationException? exception = Assert.Throws<InvalidOperationException>(() =>
            WeakSingleton<TestObject>.GetInstance(factory));

        Assert.That(exception.Message, Is.EqualTo("No instance factory provided"));
    }

    [Test]
    public void WeakSingleton_GetInstance_ShouldHandleNullDefaultFactory()
    {
        // Arrange
        WeakSingleton<TestObject>.DefaultFactory = null;

        // Act & Assert
        InvalidOperationException? exception = Assert.Throws<InvalidOperationException>(() =>
            WeakSingleton<TestObject>.GetInstance());

        Assert.That(exception.Message, Is.EqualTo("No instance factory provided"));
    }

    private void ResetSingletonState()
    {
        // Use reflection to reset the private static field
        FieldInfo? field = typeof(WeakSingleton<TestObject>).GetField("_weakInstance",
            BindingFlags.NonPublic | BindingFlags.Static);
        field?.SetValue(null, null);

        FieldInfo? field2 = typeof(WeakSingleton<ComplexTestObject>).GetField("_weakInstance",
            BindingFlags.NonPublic | BindingFlags.Static);
        field2?.SetValue(null, null);

        // Reset DefaultFactory
        PropertyInfo? defaultFactoryProperty = typeof(WeakSingleton<TestObject>).GetProperty("DefaultFactory",
            BindingFlags.Public | BindingFlags.Static);
        defaultFactoryProperty?.SetValue(null, null);

        PropertyInfo? defaultFactoryProperty2 = typeof(WeakSingleton<ComplexTestObject>).GetProperty("DefaultFactory",
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

    // Helper class for this test only
    private class UniqueTestObject
    {
    }

    private class UniqueTestObject2
    {
        public int Id { get; set; }
    }
}
