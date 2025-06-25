using System.Collections.Concurrent;
using System.Reflection;
using Singletons.Net.Specialized;

namespace Singletons.Net.Test.Specialized;

[TestFixture]
public class TestGenericSingleton
{
    [SetUp]
    public void Setup()
    {
        // Reset any static state between tests
        ResetSingletonState();
    }

    [Test]
    public void Singleton_FromInstance_ShouldCreateSingletonWithProvidedInstance()
    {
        // Arrange
        TestObject testObject = new() { Id = 1, Name = "Test" };

        // Act
        GenericSingleton<TestObject> singleton = GenericSingleton<TestObject>.FromInstance(testObject);

        // Assert
        Assert.That(singleton, Is.Not.Null);
        Assert.That(GenericSingleton<TestObject>.Instance, Is.SameAs(testObject));
    }

    [Test]
    public void Singleton_FromInstance_ShouldThrowExceptionWhenInstanceIsNull()
    {
        // Arrange & Act & Assert
        Assert.That(() => GenericSingleton<TestObject>.FromInstance(null!), Throws.ArgumentNullException);
    }

    [Test]
    public void Singleton_FromInstance_ShouldThrowExceptionWhenAlreadyInitialized()
    {
        // Arrange
        TestObject testObject1 = new() { Id = 1, Name = "Test1" };
        TestObject testObject2 = new() { Id = 2, Name = "Test2" };
        GenericSingleton<TestObject>.FromInstance(testObject1);

        // Act & Assert
        Assert.That(() => GenericSingleton<TestObject>.FromInstance(testObject2),
            Throws.InvalidOperationException.With.Message.Contains("already set"));
    }

    [Test]
    public void Singleton_FromInstance_WithOverwrite_ShouldAllowOverwriting()
    {
        // Arrange
        TestObject testObject1 = new() { Id = 1, Name = "Test1" };
        TestObject testObject2 = new() { Id = 2, Name = "Test2" };
        GenericSingleton<TestObject>.FromInstance(testObject1);

        // Act
        GenericSingleton<TestObject> singleton = GenericSingleton<TestObject>.FromInstance(testObject2, true);

        // Assert
        Assert.That(singleton, Is.Not.Null);
        Assert.That(GenericSingleton<TestObject>.Instance, Is.SameAs(testObject2));
    }

    [Test]
    public void Singleton_Instance_Get_ShouldReturnSetInstance()
    {
        // Arrange
        TestObject testObject = new() { Id = 1, Name = "Test" };
        GenericSingleton<TestObject>.Instance = testObject;

        // Act
        TestObject retrievedInstance = GenericSingleton<TestObject>.Instance;

        // Assert
        Assert.That(retrievedInstance, Is.SameAs(testObject));
    }

    [Test]
    public void Singleton_Instance_Get_ShouldThrowExceptionWhenNotInitialized()
    {
        // Arrange & Act & Assert
        Assert.That(() => GenericSingleton<TestObject>.Instance,
            Throws.InvalidOperationException.With.Message.Contains("not set"));
    }

    [Test]
    public void Singleton_Instance_Set_ShouldSetInstance()
    {
        // Arrange
        TestObject testObject = new() { Id = 1, Name = "Test" };

        // Act
        GenericSingleton<TestObject>.Instance = testObject;

        // Assert
        Assert.That(GenericSingleton<TestObject>.Instance, Is.SameAs(testObject));
    }

    [Test]
    public void Singleton_Instance_Set_ShouldThrowExceptionWhenAlreadySet()
    {
        // Arrange
        TestObject testObject1 = new() { Id = 1, Name = "Test1" };
        TestObject testObject2 = new() { Id = 2, Name = "Test2" };
        GenericSingleton<TestObject>.Instance = testObject1;

        // Act & Assert
        Assert.That(() => GenericSingleton<TestObject>.Instance = testObject2,
            Throws.InvalidOperationException.With.Message.Contains("already set"));
    }

    [Test]
    public void Singleton_ThreadSafety_ShouldHandleConcurrentAccess()
    {
        // Arrange
        TestObject testObject = new() { Id = 1, Name = "Test" };
        ConcurrentBag<TestObject> results = new();

        // Act
        GenericSingleton<TestObject>.Instance = testObject;
        Parallel.For(0, 10, _ => { results.Add(GenericSingleton<TestObject>.Instance); });

        // Assert
        TestObject firstInstance = results.First();
        Assert.That(firstInstance, Is.Not.Null);
        Assert.That(results.Count, Is.EqualTo(10), "Should have collected 10 instances");
        Assert.That(results.All(x => ReferenceEquals(x, firstInstance)), Is.True,
            "All instances should be the same across threads");
    }

    [Test]
    public void Singleton_DifferentTypes_ShouldHaveSeparateInstances()
    {
        // Arrange
        TestObject testObject1 = new() { Id = 1, Name = "Test1" };
        TestObject2 testObject2 = new() { Id = 2, Name = "Test2" };

        // Act
        GenericSingleton<TestObject>.Instance = testObject1;
        GenericSingleton<TestObject2>.Instance = testObject2;

        // Assert
        Assert.That(GenericSingleton<TestObject>.Instance, Is.SameAs(testObject1));
        Assert.That(GenericSingleton<TestObject2>.Instance, Is.SameAs(testObject2));
        Assert.That(GenericSingleton<TestObject>.Instance.GetType(),
            Is.Not.EqualTo(GenericSingleton<TestObject2>.Instance.GetType()));
        Assert.That(ReferenceEquals(GenericSingleton<TestObject>.Instance, GenericSingleton<TestObject2>.Instance),
            Is.False);
    }

    [Test]
    public void Singleton_ComplexObject_ShouldWorkWithComplexTypes()
    {
        // Arrange
        ComplexTestObject complexObject = new()
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
        GenericSingleton<ComplexTestObject>.Instance = complexObject;
        ComplexTestObject retrievedInstance = GenericSingleton<ComplexTestObject>.Instance;

        // Assert
        Assert.That(retrievedInstance, Is.SameAs(complexObject));
        Assert.That(retrievedInstance.Properties["Key1"], Is.EqualTo("Value1"));
        Assert.That(retrievedInstance.Properties["Key2"], Is.EqualTo(42));
    }

    [Test]
    public void Singleton_ResetAndReinitialize_ShouldWorkCorrectly()
    {
        // Arrange
        TestObject testObject1 = new() { Id = 1, Name = "Test1" };
        TestObject testObject2 = new() { Id = 2, Name = "Test2" };

        // Act - First initialization
        GenericSingleton<TestObject>.Instance = testObject1;
        TestObject firstInstance = GenericSingleton<TestObject>.Instance;

        // Reset state
        ResetSingletonState();

        // Act - Second initialization
        GenericSingleton<TestObject>.Instance = testObject2;
        TestObject secondInstance = GenericSingleton<TestObject>.Instance;

        // Assert
        Assert.That(firstInstance, Is.SameAs(testObject1));
        Assert.That(secondInstance, Is.SameAs(testObject2));
        Assert.That(firstInstance, Is.Not.SameAs(secondInstance));
    }

    [Test]
    public void Singleton_SetFactory_ShouldCreateInstanceUsingFactory()
    {
        // Arrange
        var factoryCallCount = 0;
        Func<TestObject> factory = () =>
        {
            factoryCallCount++;
            return new TestObject { Id = factoryCallCount, Name = $"Factory-{factoryCallCount}" };
        };

        // Act
        GenericSingleton<TestObject>.SetFactory(factory);
        TestObject instance = GenericSingleton<TestObject>.Instance;

        // Assert
        Assert.That(instance, Is.Not.Null);
        Assert.That(instance.Id, Is.EqualTo(1));
        Assert.That(instance.Name, Is.EqualTo("Factory-1"));
        Assert.That(factoryCallCount, Is.EqualTo(1), "Factory should be called exactly once");
    }

    [Test]
    public void Singleton_SetFactory_ShouldThrowExceptionWhenFactoryIsNull()
    {
        // Arrange & Act & Assert
        Assert.That(() => GenericSingleton<TestObject>.SetFactory(null!), Throws.ArgumentNullException);
    }

    [Test]
    public void Singleton_SetFactory_ShouldThrowExceptionWhenAlreadyInitialized()
    {
        // Arrange
        TestObject testObject = new() { Id = 1, Name = "Test" };
        GenericSingleton<TestObject>.Instance = testObject;

        Func<TestObject> factory = () => new TestObject { Id = 2, Name = "Factory" };

        // Act & Assert
        Assert.That(() => GenericSingleton<TestObject>.SetFactory(factory),
            Throws.InvalidOperationException.With.Message.Contains("already set"));
    }

    [Test]
    public void Singleton_SetFactory_WithOverwrite_ShouldAllowOverwriting()
    {
        // Arrange
        TestObject testObject = new() { Id = 1, Name = "Test" };
        GenericSingleton<TestObject>.Instance = testObject;

        var factoryCallCount = 0;
        Func<TestObject> factory = () =>
        {
            factoryCallCount++;
            return new TestObject { Id = factoryCallCount, Name = $"Factory-{factoryCallCount}" };
        };

        // Act
        GenericSingleton<TestObject>.SetFactory(factory, true);
        TestObject instance = GenericSingleton<TestObject>.Instance;

        // Assert
        Assert.That(instance, Is.Not.Null);
        Assert.That(instance.Id, Is.EqualTo(1));
        Assert.That(instance.Name, Is.EqualTo("Factory-1"));
        Assert.That(factoryCallCount, Is.EqualTo(1), "Factory should be called exactly once");
    }

    [Test]
    public void Singleton_SetFactory_ShouldBeLazyInitialized()
    {
        // Arrange
        var factoryCallCount = 0;
        Func<TestObject> factory = () =>
        {
            factoryCallCount++;
            return new TestObject { Id = factoryCallCount, Name = $"Factory-{factoryCallCount}" };
        };

        // Act - Set factory but don't access instance yet
        GenericSingleton<TestObject>.SetFactory(factory);

        // Assert - Factory should not be called yet
        Assert.That(factoryCallCount, Is.EqualTo(0), "Factory should not be called until instance is accessed");

        // Act - Now access the instance
        TestObject instance = GenericSingleton<TestObject>.Instance;

        // Assert - Factory should be called now
        Assert.That(factoryCallCount, Is.EqualTo(1), "Factory should be called when instance is accessed");
        Assert.That(instance, Is.Not.Null);
    }

    [Test]
    public void Singleton_SetFactory_ShouldReturnSameInstanceOnMultipleAccesses()
    {
        // Arrange
        var factoryCallCount = 0;
        Func<TestObject> factory = () =>
        {
            factoryCallCount++;
            return new TestObject { Id = factoryCallCount, Name = $"Factory-{factoryCallCount}" };
        };

        GenericSingleton<TestObject>.SetFactory(factory);

        // Act
        TestObject instance1 = GenericSingleton<TestObject>.Instance;
        TestObject instance2 = GenericSingleton<TestObject>.Instance;
        TestObject instance3 = GenericSingleton<TestObject>.Instance;

        // Assert
        Assert.That(instance1, Is.SameAs(instance2));
        Assert.That(instance2, Is.SameAs(instance3));
        Assert.That(factoryCallCount, Is.EqualTo(1), "Factory should be called exactly once");
    }

    [Test]
    public void Singleton_SetFactory_ThreadSafety_ShouldHandleConcurrentAccess()
    {
        // Arrange
        var factoryCallCount = 0;
        var lockObject = new object();
        Func<TestObject> factory = () =>
        {
            lock (lockObject)
            {
                factoryCallCount++;
                return new TestObject { Id = factoryCallCount, Name = $"Factory-{factoryCallCount}" };
            }
        };

        GenericSingleton<TestObject>.SetFactory(factory);
        ConcurrentBag<TestObject> results = new();

        // Act
        Parallel.For(0, 10, _ => { results.Add(GenericSingleton<TestObject>.Instance); });

        // Assert
        TestObject firstInstance = results.First();
        Assert.That(firstInstance, Is.Not.Null);
        Assert.That(results.Count, Is.EqualTo(10), "Should have collected 10 instances");
        Assert.That(results.All(x => ReferenceEquals(x, firstInstance)), Is.True,
            "All instances should be the same across threads");
        Assert.That(factoryCallCount, Is.EqualTo(1),
            "Factory should be called exactly once even with concurrent access");
    }

    [Test]
    public void Singleton_SetFactory_ComplexObject_ShouldWorkWithComplexTypes()
    {
        // Arrange
        var factoryCallCount = 0;
        Func<ComplexTestObject> factory = () =>
        {
            factoryCallCount++;
            return new ComplexTestObject
            {
                Id = factoryCallCount,
                Name = $"Complex-{factoryCallCount}",
                Properties = new Dictionary<string, object>
                {
                    { "Key1", $"Value{factoryCallCount}" },
                    { "Key2", factoryCallCount * 10 }
                }
            };
        };

        // Act
        GenericSingleton<ComplexTestObject>.SetFactory(factory);
        ComplexTestObject instance = GenericSingleton<ComplexTestObject>.Instance;

        // Assert
        Assert.That(instance, Is.Not.Null);
        Assert.That(instance.Id, Is.EqualTo(1));
        Assert.That(instance.Name, Is.EqualTo("Complex-1"));
        Assert.That(instance.Properties["Key1"], Is.EqualTo("Value1"));
        Assert.That(instance.Properties["Key2"], Is.EqualTo(10));
        Assert.That(factoryCallCount, Is.EqualTo(1), "Factory should be called exactly once");
    }

    [Test]
    public void Singleton_SetFactory_AfterFromInstance_ShouldThrowException()
    {
        // Arrange
        TestObject testObject = new() { Id = 1, Name = "Test" };
        GenericSingleton<TestObject>.FromInstance(testObject);

        Func<TestObject> factory = () => new TestObject { Id = 2, Name = "Factory" };

        // Act & Assert
        Assert.That(() => GenericSingleton<TestObject>.SetFactory(factory),
            Throws.InvalidOperationException.With.Message.Contains("already set"));
    }

    [Test]
    public void Singleton_SetFactory_AfterFromInstance_WithOverwrite_ShouldAllowOverwriting()
    {
        // Arrange
        TestObject testObject = new() { Id = 1, Name = "Test" };
        GenericSingleton<TestObject>.FromInstance(testObject);

        var factoryCallCount = 0;
        Func<TestObject> factory = () =>
        {
            factoryCallCount++;
            return new TestObject { Id = factoryCallCount, Name = $"Factory-{factoryCallCount}" };
        };

        // Act
        GenericSingleton<TestObject>.SetFactory(factory, true);
        TestObject instance = GenericSingleton<TestObject>.Instance;

        // Assert
        Assert.That(instance, Is.Not.Null);
        Assert.That(instance.Id, Is.EqualTo(1));
        Assert.That(instance.Name, Is.EqualTo("Factory-1"));
        Assert.That(factoryCallCount, Is.EqualTo(1), "Factory should be called exactly once");
    }

    [Test]
    public void Singleton_SetFactory_ExceptionInFactory_ShouldPropagateException()
    {
        // Arrange
        Func<TestObject> factory = () => throw new InvalidOperationException("Factory error");

        // Act & Assert
        GenericSingleton<TestObject>.SetFactory(factory);
        Assert.That(() => GenericSingleton<TestObject>.Instance,
            Throws.InvalidOperationException.With.Message.Contains("Factory error"));
    }

    [Test]
    public void Singleton_SetFactory_ReturningNull_ShouldAllowNullInstance()
    {
        // Arrange
        Func<TestObject?> factory = () => null;

        // Act
        GenericSingleton<TestObject?>.SetFactory(factory);
        TestObject? instance = GenericSingleton<TestObject?>.Instance;

        // Assert
        Assert.That(instance, Is.Null);
    }

    // Helper method to reset singleton state for testing
    private void ResetSingletonState()
    {
        // Use reflection to reset the private static field
        FieldInfo? field = typeof(GenericSingleton<TestObject>).GetField("_instance",
            BindingFlags.NonPublic | BindingFlags.Static);
        field?.SetValue(null, null);

        FieldInfo? field2 = typeof(GenericSingleton<TestObject2>).GetField("_instance",
            BindingFlags.NonPublic | BindingFlags.Static);
        field2?.SetValue(null, null);

        FieldInfo? field3 = typeof(GenericSingleton<ComplexTestObject>).GetField("_instance",
            BindingFlags.NonPublic | BindingFlags.Static);
        field3?.SetValue(null, null);

        FieldInfo? field4 = typeof(GenericSingleton<TestObject?>).GetField("_instance",
            BindingFlags.NonPublic | BindingFlags.Static);
        field4?.SetValue(null, null);
    }

    // Test classes
    public class TestObject
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class TestObject2
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
