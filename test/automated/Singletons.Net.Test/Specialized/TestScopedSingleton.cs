using System.Collections.Concurrent;
using System.Reflection;
using Singletons.Net.Specialized;

namespace Singletons.Net.Test.Specialized;

[TestFixture]
public class TestScopedSingleton
{
    [SetUp]
    public void Setup()
    {
        // Create new instances for each test to ensure clean state
        this._scopedSingleton = new ScopedSingleton<TestObject>();
        this._complexScopedSingleton = new ScopedSingleton<ComplexTestObject>();

        // Reset any static state between tests
        ResetSingletonState();
    }

    private ScopedSingleton<TestObject> _scopedSingleton = null!;
    private ScopedSingleton<ComplexTestObject> _complexScopedSingleton = null!;

    [Test]
    public void ScopedSingleton_GetInstance_ShouldReturnSameInstanceForSameScope()
    {
        // Arrange
        ScopedSingleton<TestObject>.DefaultFactory = () => new TestObject { Id = 1, Name = "Test" };

        // Act
        TestObject instance1 = this._scopedSingleton.GetInstance("scope1");
        TestObject instance2 = this._scopedSingleton.GetInstance("scope1");

        // Assert
        Assert.That(instance1, Is.Not.Null);
        Assert.That(instance2, Is.Not.Null);
        Assert.That(instance1, Is.SameAs(instance2));
    }

    [Test]
    public void ScopedSingleton_GetInstance_ShouldReturnDifferentInstancesForDifferentScopes()
    {
        // Arrange
        ScopedSingleton<TestObject>.DefaultFactory = () => new TestObject { Id = 1, Name = "Test" };

        // Act
        TestObject instance1 = this._scopedSingleton.GetInstance("scope1");
        TestObject instance2 = this._scopedSingleton.GetInstance("scope2");

        // Assert
        Assert.That(instance1, Is.Not.Null);
        Assert.That(instance2, Is.Not.Null);
        Assert.That(instance1, Is.Not.SameAs(instance2));
    }

    [Test]
    public void ScopedSingleton_GetInstance_WithFactory_ShouldReturnSameInstanceForSameScope()
    {
        // Arrange
        var factoryCallCount = 0;
        Func<TestObject> factory = () =>
        {
            factoryCallCount++;
            return new TestObject { Id = factoryCallCount, Name = $"Test-{factoryCallCount}" };
        };

        // Act
        TestObject instance1 = this._scopedSingleton.GetInstance("scope1", factory);
        TestObject instance2 = this._scopedSingleton.GetInstance("scope1", factory);

        // Assert
        Assert.That(instance1, Is.Not.Null);
        Assert.That(instance2, Is.Not.Null);
        Assert.That(instance1, Is.SameAs(instance2));
        Assert.That(factoryCallCount, Is.EqualTo(1), "Factory should be called exactly once for the same scope");
    }

    [Test]
    public void ScopedSingleton_GetInstance_ShouldBeLazyInitialized()
    {
        // Arrange
        var factoryCallCount = 0;
        Func<TestObject> factory = () =>
        {
            factoryCallCount++;
            return new TestObject { Id = factoryCallCount, Name = $"Test-{factoryCallCount}" };
        };

        // Act
        TestObject instance = this._scopedSingleton.GetInstance("scope1", factory);

        // Assert
        Assert.That(instance, Is.Not.Null);
        Assert.That(factoryCallCount, Is.EqualTo(1), "Factory should be called exactly once");
    }

    [Test]
    public void ScopedSingleton_GetInstance_ShouldBeThreadSafe()
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
        Parallel.For(0, 10, _ => { results.Add(this._scopedSingleton.GetInstance("scope1", factory)); });

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
    public void ScopedSingleton_GetInstance_ShouldThrowExceptionWhenNoFactoryProvided()
    {
        // Act & Assert
        InvalidOperationException? exception =
            Assert.Throws<InvalidOperationException>(() => this._scopedSingleton.GetInstance("scope1"));

        Assert.That(exception.Message, Is.EqualTo("No instance factory provided"));
    }

    [Test]
    public void ScopedSingleton_GetInstance_ShouldHandleComplexObjects()
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
        ComplexTestObject instance = this._complexScopedSingleton.GetInstance("scope1", factory);

        // Assert
        Assert.That(instance, Is.Not.Null);
        Assert.That(instance.Id, Is.EqualTo(1));
        Assert.That(instance.Name, Is.EqualTo("Complex Test"));
        Assert.That(instance.Properties["Key1"], Is.EqualTo("Value1"));
        Assert.That(instance.Properties["Key2"], Is.EqualTo(42));
    }

    [Test]
    public void ScopedSingleton_GetInstance_ShouldHandleExceptionInFactory()
    {
        // Arrange
        Func<TestObject> factory = () => throw new InvalidOperationException("Factory exception");

        // Act & Assert
        InvalidOperationException? exception =
            Assert.Throws<InvalidOperationException>(() => this._scopedSingleton.GetInstance("scope1", factory));

        Assert.That(exception.Message, Is.EqualTo("Factory exception"));
    }

    [Test]
    public void ScopedSingleton_GetInstance_ShouldWorkWithDefaultFactory()
    {
        // Arrange
        ScopedSingleton<TestObject>.DefaultFactory = () => new TestObject { Id = 1, Name = "Default Test" };

        // Act
        TestObject instance1 = this._scopedSingleton.GetInstance("scope1");
        TestObject instance2 = this._scopedSingleton.GetInstance("scope1");

        // Assert
        Assert.That(instance1, Is.Not.Null);
        Assert.That(instance2, Is.Not.Null);
        Assert.That(instance1, Is.SameAs(instance2));
        Assert.That(instance1.Id, Is.EqualTo(1));
        Assert.That(instance1.Name, Is.EqualTo("Default Test"));
    }

    [Test]
    public void ScopedSingleton_GetInstance_ShouldPreferProvidedFactoryOverDefaultFactory()
    {
        // Arrange
        ScopedSingleton<TestObject>.DefaultFactory = () => new TestObject { Id = 1, Name = "Default Test" };

        Func<TestObject> providedFactory = () => new TestObject { Id = 2, Name = "Provided Test" };

        // Act
        TestObject instance = this._scopedSingleton.GetInstance("scope1", providedFactory);

        // Assert
        Assert.That(instance, Is.Not.Null);
        Assert.That(instance.Id, Is.EqualTo(2));
        Assert.That(instance.Name, Is.EqualTo("Provided Test"));
    }

    [Test]
    public void ScopedSingleton_RemoveInstance_ShouldRemoveInstance()
    {
        // Arrange
        ScopedSingleton<TestObject>.DefaultFactory = () => new TestObject { Id = 1, Name = "Test" };
        TestObject instance1 = this._scopedSingleton.GetInstance("scope1");

        // Act
        var removed = this._scopedSingleton.RemoveInstance("scope1");
        TestObject instance2 = this._scopedSingleton.GetInstance("scope1");

        // Assert
        Assert.That(removed, Is.True, "Instance should be successfully removed");
        Assert.That(instance1, Is.Not.Null);
        Assert.That(instance2, Is.Not.Null);
        Assert.That(instance1, Is.Not.SameAs(instance2), "New instance should be created after removal");
    }

    [Test]
    public void ScopedSingleton_RemoveInstance_ShouldNotAffectOtherScopes()
    {
        // Arrange
        ScopedSingleton<TestObject>.DefaultFactory = () => new TestObject { Id = 1, Name = "Test" };
        TestObject instance1 = this._scopedSingleton.GetInstance("scope1");
        TestObject instance2 = this._scopedSingleton.GetInstance("scope2");

        // Act
        var removed = this._scopedSingleton.RemoveInstance("scope1");
        TestObject instance1AfterRemoval = this._scopedSingleton.GetInstance("scope1");
        TestObject instance2AfterRemoval = this._scopedSingleton.GetInstance("scope2");

        // Assert
        Assert.That(removed, Is.True, "Instance should be successfully removed");
        Assert.That(instance1, Is.Not.SameAs(instance1AfterRemoval), "Instance for scope1 should be recreated");
        Assert.That(instance2, Is.SameAs(instance2AfterRemoval), "Instance for scope2 should remain the same");
    }

    [Test]
    public void ScopedSingleton_HasInstance_ShouldReturnTrueWhenInstanceExists()
    {
        // Arrange
        ScopedSingleton<TestObject>.DefaultFactory = () => new TestObject { Id = 1, Name = "Test" };

        // Act
        var hasInstanceBefore = this._scopedSingleton.HasInstance("scope1");
        TestObject instance = this._scopedSingleton.GetInstance("scope1");
        var hasInstanceAfter = this._scopedSingleton.HasInstance("scope1");

        // Assert
        Assert.That(hasInstanceBefore, Is.False, "Should not have instance before creation");
        Assert.That(hasInstanceAfter, Is.True, "Should have instance after creation");
        Assert.That(instance, Is.Not.Null);
    }

    [Test]
    public void ScopedSingleton_HasInstance_ShouldReturnFalseWhenInstanceDoesNotExist()
    {
        // Act
        var hasInstance = this._scopedSingleton.HasInstance("scope1");

        // Assert
        Assert.That(hasInstance, Is.False, "Should not have instance when none exists");
    }

    [Test]
    public void ScopedSingleton_ClearAllInstances_ShouldRemoveAllInstances()
    {
        // Arrange
        ScopedSingleton<TestObject>.DefaultFactory = () => new TestObject { Id = 1, Name = "Test" };
        TestObject instance1 = this._scopedSingleton.GetInstance("scope1");
        TestObject instance2 = this._scopedSingleton.GetInstance("scope2");

        // Act
        this._scopedSingleton.ClearAllInstances();
        var hasInstance1 = this._scopedSingleton.HasInstance("scope1");
        var hasInstance2 = this._scopedSingleton.HasInstance("scope2");

        // Assert
        Assert.That(hasInstance1, Is.False, "Instance for scope1 should be removed");
        Assert.That(hasInstance2, Is.False, "Instance for scope2 should be removed");
    }

    [Test]
    public async Task ScopedSingleton_GetInstanceAsync_ShouldReturnSameInstanceForSameScope()
    {
        // Arrange
        ScopedSingleton<TestObject>.DefaultFactory = () => new TestObject { Id = 1, Name = "Test" };

        // Act
        TestObject instance1 = await this._scopedSingleton.GetInstanceAsync("scope1");
        TestObject instance2 = await this._scopedSingleton.GetInstanceAsync("scope1");

        // Assert
        Assert.That(instance1, Is.Not.Null);
        Assert.That(instance2, Is.Not.Null);
        Assert.That(instance1, Is.SameAs(instance2));
    }

    [Test]
    public async Task ScopedSingleton_GetInstanceAsync_WithFactory_ShouldReturnSameInstanceForSameScope()
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
        TestObject instance1 = await this._scopedSingleton.GetInstanceAsync("scope1", factory);
        TestObject instance2 = await this._scopedSingleton.GetInstanceAsync("scope1", factory);

        // Assert
        Assert.That(instance1, Is.Not.Null);
        Assert.That(instance2, Is.Not.Null);
        Assert.That(instance1, Is.SameAs(instance2));
        Assert.That(factoryCallCount, Is.EqualTo(1), "Factory should be called exactly once for the same scope");
    }

    [Test]
    public async Task ScopedSingleton_GetInstanceAsync_ShouldBeThreadSafe()
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
            TestObject instance = await this._scopedSingleton.GetInstanceAsync("scope1", factory);
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
    public async Task ScopedSingleton_GetInstanceAsync_ShouldThrowExceptionWhenNoFactoryProvided()
    {
        // Act & Assert
        InvalidOperationException? exception = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await this._scopedSingleton.GetInstanceAsync("scope1"));

        Assert.That(exception.Message, Is.EqualTo("No instance factory provided"));
    }

    [Test]
    public async Task ScopedSingleton_GetInstanceAsync_ShouldHandleExceptionInFactory()
    {
        // Arrange
        Func<Task<TestObject>> factory = async () =>
        {
            await Task.Delay(10);
            throw new InvalidOperationException("Factory exception");
        };

        // Act & Assert
        InvalidOperationException? exception = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await this._scopedSingleton.GetInstanceAsync("scope1", factory));

        Assert.That(exception.Message, Is.EqualTo("Factory exception"));
    }

    [Test]
    public async Task ScopedSingleton_ClearAllInstancesAsync_ShouldRemoveAllInstances()
    {
        // Arrange
        ScopedSingleton<TestObject>.DefaultFactory = () => new TestObject { Id = 1, Name = "Test" };
        TestObject instance1 = await this._scopedSingleton.GetInstanceAsync("scope1");
        TestObject instance2 = await this._scopedSingleton.GetInstanceAsync("scope2");

        // Act
        await this._scopedSingleton.ClearAllInstancesAsync();
        var hasInstance1 = this._scopedSingleton.HasInstance("scope1");
        var hasInstance2 = this._scopedSingleton.HasInstance("scope2");

        // Assert
        Assert.That(hasInstance1, Is.False, "Instance for scope1 should be removed");
        Assert.That(hasInstance2, Is.False, "Instance for scope2 should be removed");
    }

    [Test]
    public void ScopedSingleton_GetInstance_ShouldThrowOnNullScopeKey()
    {
        // Arrange
        ScopedSingleton<TestObject>.DefaultFactory = () => new TestObject { Id = 1, Name = "Test" };

        // Act/Assert
        Assert.Throws<ArgumentNullException>(() => this._scopedSingleton.GetInstance(null!));
    }

    [Test]
    public void ScopedSingleton_GetInstance_ShouldHandleComplexScopeKeys()
    {
        // Arrange
        ScopedSingleton<TestObject>.DefaultFactory = () => new TestObject { Id = 1, Name = "Test" };

        ComplexScopeKey scopeKey1 = new() { Id = 1, Name = "Scope1" };
        ComplexScopeKey scopeKey2 = new() { Id = 2, Name = "Scope2" };

        // Act
        TestObject instance1 = this._scopedSingleton.GetInstance(scopeKey1);
        TestObject instance2 = this._scopedSingleton.GetInstance(scopeKey2);
        instance2.Id = 2;
        instance2.Name = "Test2";
        TestObject instance3 = this._scopedSingleton.GetInstance(scopeKey1);

        // Assert
        Assert.That(instance1, Is.Not.Null);
        Assert.That(instance2, Is.Not.Null);
        Assert.That(instance3, Is.Not.Null);
        Assert.That(instance1, Is.SameAs(instance3), "Same scope key should return same instance");
        Assert.That(instance1, Is.Not.SameAs(instance2), "Different scope keys should return different instances");
    }

    [Test]
    public void ScopedSingleton_GetInstance_ShouldHandleNullFactory()
    {
        // Arrange
        Func<TestObject>? factory = null;

        // Act & Assert
        InvalidOperationException? exception =
            Assert.Throws<InvalidOperationException>(() => this._scopedSingleton.GetInstance("scope1", factory));

        Assert.That(exception.Message, Is.EqualTo("No instance factory provided"));
    }

    [Test]
    public void ScopedSingleton_GetInstance_ShouldHandleNullDefaultFactory()
    {
        // Arrange
        ScopedSingleton<TestObject>.DefaultFactory = null;

        // Act & Assert
        InvalidOperationException? exception =
            Assert.Throws<InvalidOperationException>(() => this._scopedSingleton.GetInstance("scope1"));

        Assert.That(exception.Message, Is.EqualTo("No instance factory provided"));
    }

    private void ResetSingletonState()
    {
        // Reset DefaultFactory
        PropertyInfo? defaultFactoryProperty = typeof(ScopedSingleton<TestObject>).GetProperty("DefaultFactory",
            BindingFlags.Public | BindingFlags.Static);
        defaultFactoryProperty?.SetValue(null, null);

        PropertyInfo? defaultFactoryProperty2 = typeof(ScopedSingleton<ComplexTestObject>).GetProperty("DefaultFactory",
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

    public record ComplexScopeKey
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
