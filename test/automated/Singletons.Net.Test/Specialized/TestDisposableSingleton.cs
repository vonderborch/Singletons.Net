using Singletons.Net.Specialized;

namespace Singletons.Net.Test.Specialized;

[TestFixture]
public class TestDisposableSingleton
{
    [SetUp]
    public void Setup()
    {
        DisposableSingleton<TestDisposable>.Reset();
    }

    [Test]
    public void DisposableSingleton_ShouldReturnSameInstanceUntilReset()
    {
        TestDisposable instance1 = DisposableSingleton<TestDisposable>.Instance;
        TestDisposable instance2 = DisposableSingleton<TestDisposable>.Instance;
        Assert.That(instance1, Is.SameAs(instance2));
    }

    [Test]
    public void DisposableSingleton_ShouldCreateNewInstanceAfterReset()
    {
        TestDisposable instance1 = DisposableSingleton<TestDisposable>.Instance;
        DisposableSingleton<TestDisposable>.Reset();
        TestDisposable instance2 = DisposableSingleton<TestDisposable>.Instance;
        Assert.That(instance1, Is.Not.SameAs(instance2));
        Assert.That(instance1.IsDisposed, Is.True);
    }

    public class TestDisposable : IDisposable
    {
        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            this.IsDisposed = true;
        }
    }
}
