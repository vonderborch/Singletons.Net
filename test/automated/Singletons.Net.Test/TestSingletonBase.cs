using NUnit.Framework;
using Singletons.Net;

namespace Singletons.Net.Test;

[TestFixture]
public class TestSingletonBase
{
    [SetUp]
    public void Setup()
    {
        // Reset any static state between tests
        ResetSingletonState();
    }

    [Test]
    public void SingletonBase_Instance_ShouldReturnSameInstance()
    {
        // Act
        var instance1 = TestSingleton.Instance;
        var instance2 = TestSingleton.Instance;

        // Assert
        Assert.That(instance1, Is.Not.Null);
        Assert.That(instance2, Is.Not.Null);
        Assert.That(instance1, Is.SameAs(instance2));
    }

    [Test]
    public void SingletonBase_Instance_ShouldBeLazyInitialized()
    {
        // Arrange
        TestSingletonWithCounter.Counter = 0;

        // Act
        var instance = TestSingletonWithCounter.Instance;

        // Assert
        Assert.That(instance, Is.Not.Null);
        Assert.That(TestSingletonWithCounter.Counter, Is.EqualTo(1), "Constructor should be called exactly once");
    }

    [Test]
    public void SingletonBase_Instance_ShouldBeThreadSafe()
    {
        // Arrange
        var results = new System.Collections.Concurrent.ConcurrentBag<TestSingleton>();

        // Act
        System.Threading.Tasks.Parallel.For(0, 10, _ =>
        {
            results.Add(TestSingleton.Instance);
        });

        // Assert
        var firstInstance = results.First();
        Assert.That(firstInstance, Is.Not.Null);
        Assert.That(results.Count, Is.EqualTo(10));
        Assert.That(results.All(x => ReferenceEquals(x, firstInstance)), Is.True, "All instances should be the same across threads");
    }

    [Test]
    public void SingletonBase_DifferentTypes_ShouldHaveSeparateInstances()
    {
        // Act
        var instance1 = TestSingleton.Instance;
        var instance2 = TestSingleton2.Instance;

        // Assert
        Assert.That(instance1, Is.Not.Null);
        Assert.That(instance2, Is.Not.Null);
        Assert.That(instance1.GetType(), Is.Not.EqualTo(instance2.GetType()));
        Assert.That(ReferenceEquals(instance1, instance2), Is.False);
    }

    [Test]
    public void SingletonBase_Instance_ShouldCreateInstanceWithPrivateConstructor()
    {
        // Act
        var instance = TestSingletonWithPrivateConstructor.Instance;

        // Assert
        Assert.That(instance, Is.Not.Null);
        Assert.That(instance.GetType(), Is.EqualTo(typeof(TestSingletonWithPrivateConstructor)));
    }

    [Test]
    public void SingletonBase_Instance_ShouldHandleComplexObjects()
    {
        // Act
        var instance = TestSingletonWithComplexState.Instance;

        // Assert
        Assert.That(instance, Is.Not.Null);
        Assert.That(instance.Id, Is.EqualTo(1));
        Assert.That(instance.Name, Is.EqualTo("Test"));
        Assert.That(instance.Properties, Is.Not.Null);
        Assert.That(instance.Properties.Count, Is.EqualTo(2));
    }

    [Test]
    public void SingletonBase_Instance_ShouldMaintainState()
    {
        // Arrange
        var instance1 = TestSingletonWithState.Instance;
        instance1.Counter = 42;

        // Act
        var instance2 = TestSingletonWithState.Instance;

        // Assert
        Assert.That(instance2.Counter, Is.EqualTo(42), "State should be maintained between accesses");
    }

    private void ResetSingletonState()
    {
        // Use reflection to reset the private static Lazy field
        var field = typeof(TestSingleton).GetField("Lazy", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        field?.SetValue(null, null);

        var field2 = typeof(TestSingleton2).GetField("Lazy", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        field2?.SetValue(null, null);

        var field3 = typeof(TestSingletonWithCounter).GetField("Lazy", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        field3?.SetValue(null, null);

        var field4 = typeof(TestSingletonWithPrivateConstructor).GetField("Lazy", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        field4?.SetValue(null, null);

        var field5 = typeof(TestSingletonWithComplexState).GetField("Lazy", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        field5?.SetValue(null, null);

        var field6 = typeof(TestSingletonWithState).GetField("Lazy", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        field6?.SetValue(null, null);
    }

    // Test classes
    public class TestSingleton : SingletonBase<TestSingleton>
    {
        public TestSingleton() { }
    }

    public class TestSingleton2 : SingletonBase<TestSingleton2>
    {
        public TestSingleton2() { }
    }

    public class TestSingletonWithCounter : SingletonBase<TestSingletonWithCounter>
    {
        public static int Counter = 0;

        public TestSingletonWithCounter()
        {
            Counter++;
        }
    }

    public class TestSingletonWithPrivateConstructor : SingletonBase<TestSingletonWithPrivateConstructor>
    {
        private TestSingletonWithPrivateConstructor() { }
    }

    public class TestSingletonWithComplexState : SingletonBase<TestSingletonWithComplexState>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Dictionary<string, object> Properties { get; set; } = new();

        public TestSingletonWithComplexState()
        {
            Id = 1;
            Name = "Test";
            Properties["Key1"] = "Value1";
            Properties["Key2"] = 42;
        }
    }

    public class TestSingletonWithState : SingletonBase<TestSingletonWithState>
    {
        public int Counter { get; set; }

        public TestSingletonWithState() { }
    }
} 