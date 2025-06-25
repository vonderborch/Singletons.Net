using System.Collections.Concurrent;
using System.Reflection;

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
        TestSingleton instance1 = TestSingleton.Instance;
        TestSingleton instance2 = TestSingleton.Instance;

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
        TestSingletonWithCounter instance = TestSingletonWithCounter.Instance;

        // Assert
        Assert.That(instance, Is.Not.Null);
        Assert.That(TestSingletonWithCounter.Counter, Is.EqualTo(1), "Constructor should be called exactly once");
    }

    [Test]
    public void SingletonBase_Instance_ShouldBeThreadSafe()
    {
        // Arrange
        ConcurrentBag<TestSingleton> results = new();

        // Act
        Parallel.For(0, 10, _ => { results.Add(TestSingleton.Instance); });

        // Assert
        TestSingleton firstInstance = results.First();
        Assert.That(firstInstance, Is.Not.Null);
        Assert.That(results.Count, Is.EqualTo(10));
        Assert.That(results.All(x => ReferenceEquals(x, firstInstance)), Is.True,
            "All instances should be the same across threads");
    }

    [Test]
    public void SingletonBase_DifferentTypes_ShouldHaveSeparateInstances()
    {
        // Act
        TestSingleton instance1 = TestSingleton.Instance;
        TestSingleton2 instance2 = TestSingleton2.Instance;

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
        TestSingletonWithPrivateConstructor instance = TestSingletonWithPrivateConstructor.Instance;

        // Assert
        Assert.That(instance, Is.Not.Null);
        Assert.That(instance.GetType(), Is.EqualTo(typeof(TestSingletonWithPrivateConstructor)));
    }

    [Test]
    public void SingletonBase_Instance_ShouldHandleComplexObjects()
    {
        // Act
        TestSingletonWithComplexState instance = TestSingletonWithComplexState.Instance;

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
        TestSingletonWithState instance1 = TestSingletonWithState.Instance;
        instance1.Counter = 42;

        // Act
        TestSingletonWithState instance2 = TestSingletonWithState.Instance;

        // Assert
        Assert.That(instance2.Counter, Is.EqualTo(42), "State should be maintained between accesses");
    }

    private void ResetSingletonState()
    {
        // Use reflection to reset the private static Lazy field
        FieldInfo? field = typeof(TestSingleton).GetField("Lazy",
            BindingFlags.NonPublic | BindingFlags.Static);
        field?.SetValue(null, null);

        FieldInfo? field2 = typeof(TestSingleton2).GetField("Lazy",
            BindingFlags.NonPublic | BindingFlags.Static);
        field2?.SetValue(null, null);

        FieldInfo? field3 = typeof(TestSingletonWithCounter).GetField("Lazy",
            BindingFlags.NonPublic | BindingFlags.Static);
        field3?.SetValue(null, null);

        FieldInfo? field4 = typeof(TestSingletonWithPrivateConstructor).GetField("Lazy",
            BindingFlags.NonPublic | BindingFlags.Static);
        field4?.SetValue(null, null);

        FieldInfo? field5 = typeof(TestSingletonWithComplexState).GetField("Lazy",
            BindingFlags.NonPublic | BindingFlags.Static);
        field5?.SetValue(null, null);

        FieldInfo? field6 = typeof(TestSingletonWithState).GetField("Lazy",
            BindingFlags.NonPublic | BindingFlags.Static);
        field6?.SetValue(null, null);
    }

    // Test classes
    public class TestSingleton : SingletonBase<TestSingleton>
    {
    }

    public class TestSingleton2 : SingletonBase<TestSingleton2>
    {
    }

    public class TestSingletonWithCounter : SingletonBase<TestSingletonWithCounter>
    {
        public static int Counter;

        public TestSingletonWithCounter()
        {
            Counter++;
        }
    }

    public class TestSingletonWithPrivateConstructor : SingletonBase<TestSingletonWithPrivateConstructor>
    {
        private TestSingletonWithPrivateConstructor()
        {
        }
    }

    public class TestSingletonWithComplexState : SingletonBase<TestSingletonWithComplexState>
    {
        public TestSingletonWithComplexState()
        {
            this.Id = 1;
            this.Name = "Test";
            this.Properties["Key1"] = "Value1";
            this.Properties["Key2"] = 42;
        }

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Dictionary<string, object> Properties { get; set; } = new();
    }

    public class TestSingletonWithState : SingletonBase<TestSingletonWithState>
    {
        public int Counter { get; set; }
    }
}
