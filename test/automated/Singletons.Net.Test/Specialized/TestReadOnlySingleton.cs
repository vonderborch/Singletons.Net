using NUnit.Framework;
using Singletons.Net.Specialized;

namespace Singletons.Net.Test.Specialized;

[TestFixture]
public class TestReadOnlySingleton
{
    [SetUp]
    public void Setup()
    {
        // Reset static state via reflection
        var field = typeof(ReadOnlySingleton<TestObject>).GetField("_instance", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        field?.SetValue(null, null);
        var isSetField = typeof(ReadOnlySingleton<TestObject>).GetField("_isSet", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        isSetField?.SetValue(null, false);
    }

    [Test]
    public void ReadOnlySingleton_ShouldAllowSetInstanceOnce()
    {
        // Reset static state via reflection
        var field = typeof(ReadOnlySingleton<UniqueTestObject>).GetField("_instance", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        field?.SetValue(null, null);
        var isSetField = typeof(ReadOnlySingleton<UniqueTestObject>).GetField("_isSet", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        isSetField?.SetValue(null, false);

        var custom = new UniqueTestObject { Value = 42 };
        ReadOnlySingleton<UniqueTestObject>.SetInstance(custom);
        Assert.That(ReadOnlySingleton<UniqueTestObject>.Instance, Is.SameAs(custom));
    }

    [Test]
    public void ReadOnlySingleton_ShouldReturnSameInstance()
    {
        var instance1 = ReadOnlySingleton<TestObject>.Instance;
        var instance2 = ReadOnlySingleton<TestObject>.Instance;
        Assert.That(instance1, Is.SameAs(instance2));
    }

    [Test]
    public void ReadOnlySingleton_SetInstance_ShouldThrowIfAlreadySetOrAccessed()
    {
        var _ = ReadOnlySingleton<TestObject>.Instance;
        Assert.Throws<InvalidOperationException>(() => ReadOnlySingleton<TestObject>.SetInstance(new TestObject()));
    }

    private class UniqueTestObject { public int Value { get; set; } }

    public class TestObject { public int Value { get; set; } }
} 
