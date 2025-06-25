using System;
using NUnit.Framework;
using Singletons.Net.Specialized;

namespace Singletons.Net.Test.Specialized;

[TestFixture]
public class TestDisposableSingleton
{
    [SetUp]
    public void Setup() => DisposableSingleton<TestDisposable>.Reset();

    [Test]
    public void DisposableSingleton_ShouldReturnSameInstanceUntilReset()
    {
        var instance1 = DisposableSingleton<TestDisposable>.Instance;
        var instance2 = DisposableSingleton<TestDisposable>.Instance;
        Assert.That(instance1, Is.SameAs(instance2));
    }

    [Test]
    public void DisposableSingleton_ShouldCreateNewInstanceAfterReset()
    {
        var instance1 = DisposableSingleton<TestDisposable>.Instance;
        DisposableSingleton<TestDisposable>.Reset();
        var instance2 = DisposableSingleton<TestDisposable>.Instance;
        Assert.That(instance1, Is.Not.SameAs(instance2));
        Assert.That(instance1.IsDisposed, Is.True);
    }

    public class TestDisposable : IDisposable
    {
        public bool IsDisposed { get; private set; }
        public void Dispose() => IsDisposed = true;
    }
} 