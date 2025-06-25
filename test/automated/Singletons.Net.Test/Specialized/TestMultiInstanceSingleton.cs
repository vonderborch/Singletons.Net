using System;
using NUnit.Framework;
using Singletons.Net.Specialized;

namespace Singletons.Net.Test.Specialized;

[TestFixture]
public class TestMultiInstanceSingleton
{
    [SetUp]
    public void Setup()
    {
        // Reset any static state between tests
        ResetSingletonState();
    }

    [Test]
    public void MultiInstanceSingleton_GetInstance_ShouldReturnSameInstanceForSameKey()
    {
        // Arrange
        MultiInstanceSingleton<string, TestObject>.DefaultFactory = () => new TestObject { Id = 1, Name = "Test" };

        // Act
        var instance1 = MultiInstanceSingleton<string, TestObject>.GetInstance("key1");
        var instance2 = MultiInstanceSingleton<string, TestObject>.GetInstance("key1");

        // Assert
        Assert.That(instance1, Is.Not.Null);
        Assert.That(instance2, Is.Not.Null);
        Assert.That(instance1, Is.SameAs(instance2));
    }

    [Test]
    public void MultiInstanceSingleton_GetInstance_ShouldReturnDifferentInstancesForDifferentKeys()
    {
        // Arrange
        MultiInstanceSingleton<string, TestObject>.DefaultFactory = () => new TestObject { Id = 1, Name = "Test" };

        // Act
        var instance1 = MultiInstanceSingleton<string, TestObject>.GetInstance("key1");
        var instance2 = MultiInstanceSingleton<string, TestObject>.GetInstance("key2");

        // Assert
        Assert.That(instance1, Is.Not.Null);
        Assert.That(instance2, Is.Not.Null);
        Assert.That(instance1, Is.Not.SameAs(instance2));
    }

    [Test]
    public void MultiInstanceSingleton_GetInstance_WithFactory_ShouldReturnSameInstanceForSameKey()
    {
        // Arrange
        var factoryCallCount = 0;
        Func<TestObject> factory = () =>
        {
            factoryCallCount++;
            return new TestObject { Id = factoryCallCount, Name = $"Test-{factoryCallCount}" };
        };

        // Act
        var instance1 = MultiInstanceSingleton<string, TestObject>.GetInstance("key1", factory);
        var instance2 = MultiInstanceSingleton<string, TestObject>.GetInstance("key1", factory);

        // Assert
        Assert.That(instance1, Is.Not.Null);
        Assert.That(instance2, Is.Not.Null);
        Assert.That(instance1, Is.SameAs(instance2));
        Assert.That(factoryCallCount, Is.EqualTo(1), "Factory should be called exactly once for the same key");
    }

    [Test]
    public void MultiInstanceSingleton_GetInstance_ShouldBeLazyInitialized()
    {
        // Arrange
        var factoryCallCount = 0;
        Func<TestObject> factory = () =>
        {
            factoryCallCount++;
            return new TestObject { Id = factoryCallCount, Name = $"Test-{factoryCallCount}" };
        };

        // Act
        var instance = MultiInstanceSingleton<string, TestObject>.GetInstance("key1", factory);

        // Assert
        Assert.That(instance, Is.Not.Null);
        Assert.That(factoryCallCount, Is.EqualTo(1), "Factory should be called exactly once");
    }

    [Test]
    public void MultiInstanceSingleton_GetInstance_ShouldBeThreadSafe()
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

        var results = new System.Collections.Concurrent.ConcurrentBag<TestObject>();

        // Act
        System.Threading.Tasks.Parallel.For(0, 10, _ =>
        {
            results.Add(MultiInstanceSingleton<string, TestObject>.GetInstance("key1", factory));
        });

        // Assert
        var firstInstance = results.First();
        Assert.That(firstInstance, Is.Not.Null);
        Assert.That(results.Count, Is.EqualTo(10));
        Assert.That(results.All(x => ReferenceEquals(x, firstInstance)), Is.True, "All instances should be the same across threads");
        Assert.That(factoryCallCount, Is.EqualTo(1), "Factory should be called exactly once even with concurrent access");
    }

    [Test]
    public void MultiInstanceSingleton_GetInstance_ShouldThrowExceptionWhenNoFactoryProvided()
    {
        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            MultiInstanceSingleton<string, TestObject>.GetInstance("key1"));

        Assert.That(exception.Message, Is.EqualTo("No instance factory provided"));
    }

    [Test]
    public void MultiInstanceSingleton_GetInstance_ShouldHandleComplexObjects()
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
        var instance = MultiInstanceSingleton<string, ComplexTestObject>.GetInstance("key1", factory);

        // Assert
        Assert.That(instance, Is.Not.Null);
        Assert.That(instance.Id, Is.EqualTo(1));
        Assert.That(instance.Name, Is.EqualTo("Complex Test"));
        Assert.That(instance.Properties["Key1"], Is.EqualTo("Value1"));
        Assert.That(instance.Properties["Key2"], Is.EqualTo(42));
    }

    [Test]
    public void MultiInstanceSingleton_GetInstance_ShouldHandleExceptionInFactory()
    {
        // Arrange
        Func<TestObject> factory = () => throw new InvalidOperationException("Factory exception");

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            MultiInstanceSingleton<string, TestObject>.GetInstance("key1", factory));

        Assert.That(exception.Message, Is.EqualTo("Factory exception"));
    }

    [Test]
    public void MultiInstanceSingleton_GetInstance_ShouldWorkWithDefaultFactory()
    {
        // Arrange
        MultiInstanceSingleton<string, TestObject>.DefaultFactory = () => new TestObject { Id = 1, Name = "Default Test" };

        // Act
        var instance1 = MultiInstanceSingleton<string, TestObject>.GetInstance("key1");
        var instance2 = MultiInstanceSingleton<string, TestObject>.GetInstance("key1");

        // Assert
        Assert.That(instance1, Is.Not.Null);
        Assert.That(instance2, Is.Not.Null);
        Assert.That(instance1, Is.SameAs(instance2));
        Assert.That(instance1.Id, Is.EqualTo(1));
        Assert.That(instance1.Name, Is.EqualTo("Default Test"));
    }

    [Test]
    public void MultiInstanceSingleton_GetInstance_ShouldPreferProvidedFactoryOverDefaultFactory()
    {
        // Arrange
        MultiInstanceSingleton<string, TestObject>.DefaultFactory = () => new TestObject { Id = 1, Name = "Default Test" };

        Func<TestObject> providedFactory = () => new TestObject { Id = 2, Name = "Provided Test" };

        // Act
        var instance = MultiInstanceSingleton<string, TestObject>.GetInstance("key1", providedFactory);

        // Assert
        Assert.That(instance, Is.Not.Null);
        Assert.That(instance.Id, Is.EqualTo(2));
        Assert.That(instance.Name, Is.EqualTo("Provided Test"));
    }

    [Test]
    public void MultiInstanceSingleton_RemoveInstance_ShouldRemoveInstance()
    {
        // Arrange
        MultiInstanceSingleton<string, TestObject>.DefaultFactory = () => new TestObject { Id = 1, Name = "Test" };
        var instance1 = MultiInstanceSingleton<string, TestObject>.GetInstance("key1");

        // Act
        MultiInstanceSingleton<string, TestObject>.RemoveInstance("key1");
        var instance2 = MultiInstanceSingleton<string, TestObject>.GetInstance("key1");

        // Assert
        Assert.That(instance1, Is.Not.Null);
        Assert.That(instance2, Is.Not.Null);
        Assert.That(instance1, Is.Not.SameAs(instance2), "New instance should be created after removal");
    }

    [Test]
    public void MultiInstanceSingleton_RemoveInstance_ShouldNotAffectOtherKeys()
    {
        // Arrange
        MultiInstanceSingleton<string, TestObject>.DefaultFactory = () => new TestObject { Id = 1, Name = "Test" };
        var instance1 = MultiInstanceSingleton<string, TestObject>.GetInstance("key1");
        var instance2 = MultiInstanceSingleton<string, TestObject>.GetInstance("key2");

        // Act
        MultiInstanceSingleton<string, TestObject>.RemoveInstance("key1");
        var instance1AfterRemoval = MultiInstanceSingleton<string, TestObject>.GetInstance("key1");
        var instance2AfterRemoval = MultiInstanceSingleton<string, TestObject>.GetInstance("key2");

        // Assert
        Assert.That(instance1, Is.Not.SameAs(instance1AfterRemoval), "Instance for key1 should be recreated");
        Assert.That(instance2, Is.SameAs(instance2AfterRemoval), "Instance for key2 should remain the same");
    }

    [Test]
    public void MultiInstanceSingleton_GetInstance_ShouldHandleDifferentKeyTypes()
    {
        // Arrange
        MultiInstanceSingleton<int, TestObject>.DefaultFactory = () => new TestObject { Id = 1, Name = "Test" };

        // Act
        var instance1 = MultiInstanceSingleton<int, TestObject>.GetInstance(1);
        var instance2 = MultiInstanceSingleton<int, TestObject>.GetInstance(2);
        var instance3 = MultiInstanceSingleton<int, TestObject>.GetInstance(1);

        // Assert
        Assert.That(instance1, Is.Not.Null);
        Assert.That(instance2, Is.Not.Null);
        Assert.That(instance3, Is.Not.Null);
        Assert.That(instance1, Is.SameAs(instance3), "Same key should return same instance");
        Assert.That(instance1, Is.Not.SameAs(instance2), "Different keys should return different instances");
    }

    [Test]
    public void MultiInstanceSingleton_GetInstance_ShouldHandleNullKey()
    {
        MultiInstanceSingleton<string, TestObject>.DefaultFactory = () => new TestObject { Id = 1, Name = "Test" };
        Assert.Throws<ArgumentNullException>(() => MultiInstanceSingleton<string, TestObject>.GetInstance(null!));
    }

    [Test]
    public void MultiInstanceSingleton_GetInstance_ShouldHandleNullFactory()
    {
        // Arrange
        Func<TestObject>? factory = null;

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            MultiInstanceSingleton<string, TestObject>.GetInstance("key1", factory));

        Assert.That(exception.Message, Is.EqualTo("No instance factory provided"));
    }

    [Test]
    public void MultiInstanceSingleton_GetInstance_ShouldHandleNullDefaultFactory()
    {
        // Arrange
        MultiInstanceSingleton<string, TestObject>.DefaultFactory = null;

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            MultiInstanceSingleton<string, TestObject>.GetInstance("key1"));

        Assert.That(exception.Message, Is.EqualTo("No instance factory provided"));
    }

    [Test]
    public void MultiInstanceSingleton_GetInstance_ShouldHandleComplexKeyTypes()
    {
        // Arrange
        MultiInstanceSingleton<ComplexKey, TestObject>.DefaultFactory = () => new TestObject { Id = 1, Name = "Test" };

        var key1 = new ComplexKey { Id = 1, Name = "Key1" };
        var key2 = new ComplexKey { Id = 2, Name = "Key2" };

        // Act
        var instance1 = MultiInstanceSingleton<ComplexKey, TestObject>.GetInstance(key1);
        var instance2 = MultiInstanceSingleton<ComplexKey, TestObject>.GetInstance(key2);
        var instance3 = MultiInstanceSingleton<ComplexKey, TestObject>.GetInstance(key1);

        // Assert
        Assert.That(instance1, Is.Not.Null);
        Assert.That(instance2, Is.Not.Null);
        Assert.That(instance3, Is.Not.Null);
        Assert.That(instance1, Is.SameAs(instance3), "Same key should return same instance");
        Assert.That(instance1, Is.Not.SameAs(instance2), "Different keys should return different instances");
    }

    [Test]
    public void MultiInstanceSingleton_GetInstance_ShouldThrowOnNullKey()
    {
        MultiInstanceSingleton<string, TestObject>.DefaultFactory = () => new TestObject { Id = 1, Name = "Test" };
        Assert.Throws<ArgumentNullException>(() => MultiInstanceSingleton<string, TestObject>.GetInstance(null!));
    }

    private void ResetSingletonState()
    {
        // Use reflection to clear the private static dictionary for all type combinations used in the tests
        var types = new[]
        {
            typeof(MultiInstanceSingleton<string, TestObject>),
            typeof(MultiInstanceSingleton<string, ComplexTestObject>),
            typeof(MultiInstanceSingleton<int, TestObject>),
            typeof(MultiInstanceSingleton<ComplexKey, TestObject>)
        };
        foreach (var t in types)
        {
            var field = t.GetField("_instances", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var dict = field?.GetValue(null) as System.Collections.IDictionary;
            dict?.Clear();
            var defaultFactoryProperty = t.GetProperty("DefaultFactory", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            defaultFactoryProperty?.SetValue(null, null);
        }
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

    public record ComplexKey
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
} 