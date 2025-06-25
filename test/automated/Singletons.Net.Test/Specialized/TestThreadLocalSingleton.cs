using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Singletons.Net.Specialized;

namespace Singletons.Net.Test.Specialized;

[TestFixture]
public class TestThreadLocalSingleton
{
    [Test]
    public void ThreadLocalSingleton_ShouldReturnSameInstanceWithinThread()
    {
        var instance1 = ThreadLocalSingleton<TestObject>.Instance;
        var instance2 = ThreadLocalSingleton<TestObject>.Instance;
        Assert.That(instance1, Is.SameAs(instance2));
    }

    [Test]
    public void ThreadLocalSingleton_ShouldReturnDifferentInstancesAcrossThreads()
    {
        TestObject? instance1 = null;
        TestObject? instance2 = null;
        var t1 = new Thread(() => instance1 = ThreadLocalSingleton<TestObject>.Instance);
        var t2 = new Thread(() => instance2 = ThreadLocalSingleton<TestObject>.Instance);
        t1.Start(); t2.Start(); t1.Join(); t2.Join();
        Assert.That(instance1, Is.Not.Null);
        Assert.That(instance2, Is.Not.Null);
        Assert.That(instance1, Is.Not.SameAs(instance2));
    }

    public class TestObject { }
} 