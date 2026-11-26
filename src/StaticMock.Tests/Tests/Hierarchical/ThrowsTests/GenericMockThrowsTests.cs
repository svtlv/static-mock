using NUnit.Framework;
using NUnit.Framework.Legacy;
using StaticMock.Tests.Common.TestEntities;
using StaticMock.Tests.Entities;

namespace StaticMock.Tests.Tests.Hierarchical.ThrowsTests;

[TestFixture]
public class GenericMockThrowsTests
{
    [Test]
    public void TestThrowsTestMethodReturn1WithoutParameters()
    {
        Mock.Setup(() => TestStaticClass.TestMethodReturn1WithoutParameters(), () =>
            {
                Assert.Throws<Exception>(() => TestStaticClass.TestMethodReturn1WithoutParameters());
            }).Throws<Exception>();
    }

    [Test]
    public void TestThrowsTestVoidMethodWithoutParameters()
    {
        Mock.Setup(() => TestStaticClass.TestVoidMethodWithoutParameters(), () =>
            {
                Assert.Throws<Exception>(() => TestStaticClass.TestVoidMethodWithoutParameters());
            }).Throws<Exception>();
    }

    [Test]
    public void TestThrowsTestVoidMethodWithParameters()
    {
        Mock.Setup(() => TestStaticClass.TestVoidMethodWithParameters(1), () =>
            {
                Assert.Throws<Exception>(() => TestStaticClass.TestVoidMethodWithParameters(1));
            }).Throws<Exception>();
    }

    [Test]
    public void TestThrowsArgumentNullException()
    {
        Mock.Setup(() => TestStaticClass.TestVoidMethodWithoutParameters(), () =>
            {
                Assert.Throws<ArgumentNullException>(() => TestStaticClass.TestVoidMethodWithoutParameters());
            }).Throws<ArgumentNullException>();
    }

    [Test]
    public void TestInstanceMethodThrows()
    {
        var testInstance = new TestInstance();

        Mock.Setup(() => testInstance.TestMethodReturn1WithoutParameters(), () =>
        {
            Assert.Throws<Exception>(() => testInstance.TestMethodReturn1WithoutParameters());
        }).Throws<Exception>();
    }

    [Test]
    public void TestGenericThrowsTestMethodReturnTask()
    {
        Mock.Setup(() => TestStaticAsyncClass.TestMethodReturnTask(), () =>
        {
            Assert.ThrowsAsync<Exception>(() => TestStaticAsyncClass.TestMethodReturnTask()).GetAwaiter().GetResult();
        }).Throws<Exception>();
    }

    [Test]
    public void TestGenericThrowsTestMethodReturnTaskAsync()
    {
        Mock.Setup(() => TestStaticAsyncClass.TestMethodReturnTaskAsync(), () =>
        {
            Assert.ThrowsAsync<Exception>(() => TestStaticAsyncClass.TestMethodReturnTaskAsync()).GetAwaiter().GetResult();
        }).Throws<Exception>();
    }

    [Test]
    public void TestGenericThrowsTestVoidMethodReturnTaskAsync()
    {
        Mock.Setup(() => TestStaticAsyncClass.TestVoidMethodReturnTaskAsync(), () =>
        {
            Assert.Throws<Exception>(() => TestStaticAsyncClass.TestVoidMethodReturnTaskAsync());
        }).Throws<Exception>();
    }

    [Test]
    public void TestGenericThrowsTestMethodReturnTaskWithoutParametersAsync()
    {
        Mock.Setup(() => TestStaticAsyncClass.TestMethodReturnTaskWithoutParametersAsync(), () =>
        {
            Assert.ThrowsAsync<Exception>(() => TestStaticAsyncClass.TestMethodReturnTaskWithoutParametersAsync()).GetAwaiter().GetResult();
        }).Throws<Exception>();
    }

    [Test]
    public void TestGenericThrowsTestMethodReturnTaskWithoutParameters()
    {
        Mock.Setup(() => TestStaticAsyncClass.TestMethodReturnTaskWithoutParameters(), () =>
        {
            Assert.ThrowsAsync<Exception>(() => TestStaticAsyncClass.TestMethodReturnTaskWithoutParameters()).GetAwaiter().GetResult();
        }).Throws<Exception>();
    }

    [Test]
    public void TestGenericThrowsInstanceTestMethodReturnTask()
    {
        var instance = new TestInstance();

        Mock.Setup(() => instance.TestMethodReturnTask(), () =>
        {
            Assert.ThrowsAsync<Exception>(() => instance.TestMethodReturnTask()).GetAwaiter().GetResult();
        }).Throws<Exception>();
    }

    [Test]
    public void TestGenericThrowsInstanceTestMethodReturnTaskAsync()
    {
        var instance = new TestInstance();

        Mock.Setup(() => instance.TestMethodReturnTaskAsync(), () =>
        {
            Assert.ThrowsAsync<Exception>(() => instance.TestMethodReturnTaskAsync()).GetAwaiter().GetResult();
        }).Throws<Exception>();
    }

    [Test]
    public void TestGenericThrowsInstanceTestVoidMethodReturnTaskAsync()
    {
        var instance = new TestInstance();

        Mock.Setup(() => instance.TestVoidMethodReturnTaskAsync(), () =>
        {
            Assert.Throws<Exception>(() => instance.TestVoidMethodReturnTaskAsync());
        }).Throws<Exception>();
    }

    [Test]
    public void TestGenericThrowsInstanceTestMethodReturnTaskWithoutParametersAsync()
    {
        var instance = new TestInstance();

        Mock.Setup(() => instance.TestMethodReturnTaskWithoutParametersAsync(), () =>
        {
            Assert.ThrowsAsync<Exception>(() => instance.TestMethodReturnTaskWithoutParametersAsync()).GetAwaiter().GetResult();
        }).Throws<Exception>();
    }

    [Test]
    public void TestGenericThrowsInstanceTestMethodReturnTaskWithoutParameters()
    {
        var instance = new TestInstance();

        Mock.Setup(() => instance.TestMethodReturnTaskWithoutParameters(), () =>
        {
            Assert.ThrowsAsync<Exception>(() => instance.TestMethodReturnTaskWithoutParameters()).GetAwaiter().GetResult();
        }).Throws<Exception>();
    }

    [Test]
    public void TestThrowsStaticIntProperty()
    {
        var originalValue = TestStaticClass.StaticIntProperty;
        ClassicAssert.AreEqual(default(int), originalValue);

        Mock.Setup(() => TestStaticClass.StaticIntProperty, () =>
        {
            Assert.Throws<Exception>(() =>
            {
                var actualResult = TestStaticClass.StaticIntProperty;
            });
        }).Throws<Exception>();
    }

    [Test]
    public void TestThrowsStaticObjectProperty()
    {
        var originalValue = TestStaticClass.StaticObjectProperty;
        ClassicAssert.AreEqual(default, originalValue);

        Mock.Setup(() => TestStaticClass.StaticObjectProperty, () =>
        {
            Assert.Throws<Exception>(() =>
            {
                var actualResult = TestStaticClass.StaticObjectProperty;
            });
        }).Throws<Exception>();
    }

    [Test]
    public void TestThrowsStaticIntPropertyInstance()
    {
        var instance = new TestInstance();
        var originalValue = instance.IntProperty;
        ClassicAssert.AreEqual(default(int), originalValue);

        Mock.Setup(() => instance.IntProperty, () =>
        {
            Assert.Throws<Exception>(() =>
            {
                var actualResult = instance.IntProperty;
            });
        }).Throws<Exception>();
    }

    [Test]
    public void TestThrowsStaticObjectPropertyInstance()
    {
        var instance = new TestInstance();
        var originalValue = instance.ObjectProperty;
        ClassicAssert.AreEqual(default, originalValue);

        Mock.Setup(() => instance.ObjectProperty, () =>
        {
            Assert.Throws<Exception>(() =>
            {
                var actualResult = instance.ObjectProperty;
            });
        }).Throws<Exception>();
    }

    [Test]
    public void TestGenericSetupThrowsWithGenericTestMethodReturnDefaultWithoutParameters()
    {
        var originalResult = TestStaticClass.GenericTestMethodReturnDefaultWithoutParameters<int>();
        ClassicAssert.AreEqual(0, originalResult);

        Mock.Setup(() => TestStaticClass.GenericTestMethodReturnDefaultWithoutParameters<int>(), () =>
        {
            Assert.Throws<Exception>(() => TestStaticClass.GenericTestMethodReturnDefaultWithoutParameters<int>());
        }).Throws<Exception>();
    }

    [Test]
    public void TestGenericSetupThrowsWithGenericTestMethodReturnDefaultWithoutParametersInstance()
    {
        var testInstance = new TestInstance();
        var originalResult = testInstance.GenericTestMethodReturnDefaultWithoutParameters<int>();
        ClassicAssert.AreEqual(0, originalResult);

        Mock.Setup(() => testInstance.GenericTestMethodReturnDefaultWithoutParameters<int>(), () =>
        {
            Assert.Throws<Exception>(() => testInstance.GenericTestMethodReturnDefaultWithoutParameters<int>());
        }).Throws<Exception>();
    }

    [Test]
    public void TestGenericSetupThrowsWithGenericTestInstanceReturnDefaultWithoutParameters()
    {
        var testInstance = new TestGenericInstance<int>();
        var originalResult = testInstance.GenericTestMethodReturnDefaultWithoutParameters();
        ClassicAssert.AreEqual(0, originalResult);

        Mock.Setup(() => testInstance.GenericTestMethodReturnDefaultWithoutParameters(), () =>
        {
            Assert.Throws<Exception>(() => testInstance.GenericTestMethodReturnDefaultWithoutParameters());
        }).Throws<Exception>();
    }

    [Test]
    public void TestThrowsParameterLessCustomExceptionTestMethodReturn1WithoutParameters()
    {
        const string message = "testExceptionMessage";
        Mock.Setup(() => TestStaticClass.TestMethodReturn1WithoutParameters(), () =>
            {
                Assert.Throws<CustomExceptionWithoutDefaultConstructor>(() => TestStaticClass.TestMethodReturn1WithoutParameters(), message);
            }).Throws<CustomExceptionWithoutDefaultConstructor>([message]);
    }

    [Test]
    public void TestThrowsExceptionWithMessageTestMethodReturn1WithoutParameters()
    {
        const string message = "testExceptionMessage";
        Mock.Setup(() => TestStaticClass.TestMethodReturn1WithoutParameters(), () =>
        {
            Assert.Throws<Exception>(() => TestStaticClass.TestMethodReturn1WithoutParameters(), message);
        }).Throws<Exception>([message]);
    }
    
    [Test]
    [NonParallelizable]
    public void TestGenericSetupReturnsDateTimeUtcNow()
    {
        Mock.Setup(context => DateTime.UtcNow, () =>
            {
                Assert.Throws<Exception>(() =>
                {
                    var x = DateTime.UtcNow;
                });
            })
            .Throws<Exception>();
    }
    
    [Test]
    [NonParallelizable]
    public void TestGenericSetupReturnsDateTimeNow()
    {
        Mock.Setup(context => DateTime.Now, () =>
            {
                Assert.Throws<Exception>(() =>
                {
                    var x = DateTime.Now;
                });
            })
            .Throws<Exception>();
    }
    
    [Test]
    public void TestGenericSetupReturnsStaticUnsafeProperty()
    {
        Mock.Setup(context => TestStaticClass.UnsafeProperty, () =>
            {
                Assert.Throws<Exception>(() =>
                {
                    var x = TestStaticClass.UnsafeProperty;
                });
            })
            .Throws<Exception>();
    }
    
    [Test]
    public void TestGenericSetupReturnsStaticUnsafeMethod()
    {
        Mock.Setup(context => TestStaticClass.TestUnsafeStaticMethod(), () =>
            {
                Assert.Throws<Exception>(() => TestStaticClass.TestUnsafeStaticMethod());
            })
            .Throws<Exception>();
        
    }
    
    [Test]
    public void TestGenericSetupReturnsInstanceUnsafeProperty()
    {
        var instance = new TestInstance();
        Mock.Setup(context => instance.UnsafeProperty, () =>
            {
                Assert.Throws<Exception>(() =>
                {
                    var x = instance.UnsafeProperty;
                });
            })
            .Throws<Exception>();
    }
    
    [Test]
    public void TestGenericSetupReturnsInstanceUnsafeMethod()
    {
        var instance = new TestInstance();
        Mock.Setup(context => instance.TestUnsafeInstanceMethod(), () =>
            {
                Assert.Throws<Exception>(() => instance.TestUnsafeInstanceMethod());
            })
            .Throws<Exception>();
        
    }
}