using Singletons.Net.Specialized;

namespace Singletons.Net.Test.Specialized;

[TestFixture]
public class TestResettableSingleton
{
    [SetUp]
    public void Setup()
    {
        ResettableSingleton<TestObject>.Reset();
    }

    [Test]
    public void ResettableSingleton_ShouldReturnSameInstanceUntilReset()
    {
        TestObject instance1 = ResettableSingleton<TestObject>.Instance;
        TestObject instance2 = ResettableSingleton<TestObject>.Instance;
        Assert.That(instance1, Is.SameAs(instance2));
    }

    [Test]
    public void ResettableSingleton_ShouldCreateNewInstanceAfterReset()
    {
        TestObject instance1 = ResettableSingleton<TestObject>.Instance;
        ResettableSingleton<TestObject>.Reset();
        TestObject instance2 = ResettableSingleton<TestObject>.Instance;
        Assert.That(instance1, Is.Not.SameAs(instance2));
    }

    public class TestObject
    {
    }
}
