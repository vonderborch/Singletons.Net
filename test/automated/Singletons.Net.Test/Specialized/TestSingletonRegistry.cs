using Singletons.Net.Specialized;

namespace Singletons.Net.Test.Specialized;

[TestFixture]
public class TestSingletonRegistry
{
    [SetUp]
    public void Setup()
    {
        SingletonRegistry.Clear();
    }

    [Test]
    public void SingletonRegistry_ShouldRegisterAndGetInstance()
    {
        TestObject obj = new();
        SingletonRegistry.Register(obj);
        Assert.That(SingletonRegistry.Get<TestObject>(), Is.SameAs(obj));
    }

    [Test]
    public void SingletonRegistry_TryGet_ShouldReturnTrueIfRegistered()
    {
        TestObject obj = new();
        SingletonRegistry.Register(obj);
        var result = SingletonRegistry.TryGet<TestObject>(out TestObject? retrieved);
        Assert.That(result, Is.True);
        Assert.That(retrieved, Is.SameAs(obj));
    }

    [Test]
    public void SingletonRegistry_TryGet_ShouldReturnFalseIfNotRegistered()
    {
        var result = SingletonRegistry.TryGet<TestObject>(out TestObject? retrieved);
        Assert.That(result, Is.False);
        Assert.That(retrieved, Is.Null);
    }

    [Test]
    public void SingletonRegistry_Remove_ShouldRemoveInstance()
    {
        TestObject obj = new();
        SingletonRegistry.Register(obj);
        SingletonRegistry.Remove<TestObject>();
        Assert.Throws<InvalidOperationException>(() => SingletonRegistry.Get<TestObject>());
    }

    [Test]
    public void SingletonRegistry_Clear_ShouldRemoveAllInstances()
    {
        SingletonRegistry.Register(new TestObject());
        SingletonRegistry.Register(new AnotherObject());
        SingletonRegistry.Clear();
        Assert.Throws<InvalidOperationException>(() => SingletonRegistry.Get<TestObject>());
        Assert.Throws<InvalidOperationException>(() => SingletonRegistry.Get<AnotherObject>());
    }

    public class TestObject
    {
    }

    public class AnotherObject
    {
    }
}
