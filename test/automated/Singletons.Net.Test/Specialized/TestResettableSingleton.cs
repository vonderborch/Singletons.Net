using NUnit.Framework;
using Singletons.Net.Specialized;

namespace Singletons.Net.Test.Specialized;

[TestFixture]
public class TestResettableSingleton
{
    [SetUp]
    public void Setup() => ResettableSingleton<TestObject>.Reset();

    [Test]
    public void ResettableSingleton_ShouldReturnSameInstanceUntilReset()
    {
        var instance1 = ResettableSingleton<TestObject>.Instance;
        var instance2 = ResettableSingleton<TestObject>.Instance;
        Assert.That(instance1, Is.SameAs(instance2));
    }

    [Test]
    public void ResettableSingleton_ShouldCreateNewInstanceAfterReset()
    {
        var instance1 = ResettableSingleton<TestObject>.Instance;
        ResettableSingleton<TestObject>.Reset();
        var instance2 = ResettableSingleton<TestObject>.Instance;
        Assert.That(instance1, Is.Not.SameAs(instance2));
    }

    public class TestObject { }
} 