using System.Reflection;
using Singletons.Net.Specialized;

namespace Singletons.Net.Test.Specialized;

[TestFixture]
public class TestReadOnlySingleton
{
    [SetUp]
    public void Setup()
    {
        // Reset static state via reflection
        FieldInfo? field =
            typeof(ReadOnlySingleton<TestObject>).GetField("_instance", BindingFlags.NonPublic | BindingFlags.Static);
        field?.SetValue(null, null);
        FieldInfo? isSetField =
            typeof(ReadOnlySingleton<TestObject>).GetField("_isSet", BindingFlags.NonPublic | BindingFlags.Static);
        isSetField?.SetValue(null, false);
    }

    [Test]
    public void ReadOnlySingleton_ShouldAllowSetInstanceOnce()
    {
        // Reset static state via reflection
        FieldInfo? field =
            typeof(ReadOnlySingleton<UniqueTestObject>).GetField("_instance",
                BindingFlags.NonPublic | BindingFlags.Static);
        field?.SetValue(null, null);
        FieldInfo? isSetField =
            typeof(ReadOnlySingleton<UniqueTestObject>).GetField("_isSet",
                BindingFlags.NonPublic | BindingFlags.Static);
        isSetField?.SetValue(null, false);

        UniqueTestObject custom = new() { Value = 42 };
        ReadOnlySingleton<UniqueTestObject>.SetInstance(custom);
        Assert.That(ReadOnlySingleton<UniqueTestObject>.Instance, Is.SameAs(custom));
    }

    [Test]
    public void ReadOnlySingleton_ShouldReturnSameInstance()
    {
        TestObject instance1 = ReadOnlySingleton<TestObject>.Instance;
        TestObject instance2 = ReadOnlySingleton<TestObject>.Instance;
        Assert.That(instance1, Is.SameAs(instance2));
    }

    [Test]
    public void ReadOnlySingleton_SetInstance_ShouldThrowIfAlreadySetOrAccessed()
    {
        TestObject _ = ReadOnlySingleton<TestObject>.Instance;
        Assert.Throws<InvalidOperationException>(() => ReadOnlySingleton<TestObject>.SetInstance(new TestObject()));
    }

    private class UniqueTestObject
    {
        public int Value { get; set; }
    }

    public class TestObject
    {
        public int Value { get; set; }
    }
}
