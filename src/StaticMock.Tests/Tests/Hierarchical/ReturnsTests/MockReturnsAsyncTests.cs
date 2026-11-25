using NUnit.Framework;
using NUnit.Framework.Legacy;
using StaticMock.Entities;
using StaticMock.Tests.Common.TestEntities;

namespace StaticMock.Tests.Tests.Hierarchical.ReturnsTests;

[TestFixture]
public class MockReturnsAsyncTests
{
    [Test]
    public async Task TestSetupReturnsAsyncMethodsReturnTask()
    {
        var originalResult = await TestStaticAsyncClass.TestMethodReturnTaskWithoutParameters();
        ClassicAssert.AreEqual(1, originalResult);
        var expectedResult = 2;

        Mock.Setup(typeof(TestStaticAsyncClass), nameof(TestStaticAsyncClass.TestMethodReturnTaskWithoutParameters), () =>
        {
            var actualResult = TestStaticAsyncClass.TestMethodReturnTaskWithoutParameters().GetAwaiter().GetResult();
            ClassicAssert.AreEqual(expectedResult, actualResult);
        }).ReturnsAsync(expectedResult);
    }

    [Test]
    public async Task TestSetupReturnsAsyncMethodsReturnTaskAsync()
    {
        var originalResult = await TestStaticAsyncClass.TestMethodReturnTaskWithoutParametersAsync();
        ClassicAssert.AreEqual(1, originalResult);
        var expectedResult = 2;

        Mock.Setup(typeof(TestStaticAsyncClass), nameof(TestStaticAsyncClass.TestMethodReturnTaskWithoutParametersAsync), () =>
        {
            var actualResult = TestStaticAsyncClass.TestMethodReturnTaskWithoutParametersAsync().GetAwaiter().GetResult();
            ClassicAssert.AreEqual(expectedResult, actualResult);
        }).ReturnsAsync(expectedResult);
    }

    [Test]
    public async Task TestSetupInstanceReturnsAsyncMethodsReturnTask()
    {
        var instance = new TestInstance();
        var originalResult = await instance.TestMethodReturnTaskWithoutParameters();
        ClassicAssert.AreEqual(1, originalResult);
        var expectedResult = 2;

        Mock.Setup(typeof(TestInstance), nameof(TestInstance.TestMethodReturnTaskWithoutParameters), new SetupProperties { Instance = instance }, () =>
        {
            var actualResult = instance.TestMethodReturnTaskWithoutParameters().GetAwaiter().GetResult();
            ClassicAssert.AreEqual(expectedResult, actualResult);
        }).ReturnsAsync(expectedResult);
    }

    [Test]
    public async Task TestSetupInstanceReturnsAsyncMethodsReturnTaskAsync()
    {
        var instance = new TestInstance();
        var originalResult = await instance.TestMethodReturnTaskWithoutParametersAsync();
        ClassicAssert.AreEqual(1, originalResult);
        var expectedResult = 2;

        Mock.Setup(typeof(TestInstance), nameof(TestInstance.TestMethodReturnTaskWithoutParametersAsync), new SetupProperties { Instance = instance }, () =>
        {
            var actualResult = instance.TestMethodReturnTaskWithoutParametersAsync().GetAwaiter().GetResult();
            ClassicAssert.AreEqual(expectedResult, actualResult);
        }).ReturnsAsync(expectedResult);
    }

    [Test]
    public async Task TestGenericSetupReturnsWithTestMethodReturnWithoutParameterFuncAsync()
    {
        var originalResult = await TestStaticAsyncClass.TestMethodReturnTaskWithoutParametersAsync();
        const int expectedResult = 2;

        Mock.Setup(
                typeof(TestStaticAsyncClass), nameof(TestStaticAsyncClass.TestMethodReturnTaskWithoutParametersAsync),
                () =>
                {
                    var actualResult = TestStaticAsyncClass.TestMethodReturnTaskWithoutParametersAsync().GetAwaiter().GetResult();

                    ClassicAssert.AreNotEqual(originalResult, actualResult);
                    ClassicAssert.AreEqual(expectedResult, actualResult);
                })
            .Returns(() => Task.FromResult(2));

        ClassicAssert.AreEqual(1, originalResult);
    }

    [Test]
    public async Task TestSetupReturnsWithTestMethodReturnParameterFuncAsync()
    {
        const int parameter1 = 10;

        var originalResult = await TestStaticAsyncClass.TestMethodReturnWithParameterAsync(parameter1);
        var expectedResult = originalResult / 2;

        ClassicAssert.AreEqual(parameter1, originalResult);

        Mock.Setup(typeof(TestStaticAsyncClass), nameof(TestStaticAsyncClass.TestMethodReturnWithParameterAsync),
            () =>
                {
                    var actualResult = TestStaticAsyncClass.TestMethodReturnWithParameterAsync(parameter1).GetAwaiter().GetResult();

                    ClassicAssert.AreNotEqual(originalResult, actualResult);
                    ClassicAssert.AreEqual(expectedResult, actualResult);
                })
            .Returns<int, Task<int>>(async argument =>
            {
                await Task.CompletedTask;
                ClassicAssert.AreEqual(parameter1, argument);
                return argument / 2;
            });

        ClassicAssert.AreEqual(parameter1, originalResult);
    }

    [Test]
    public async Task TestSetupReturnsWithTestMethodReturnParameters2FuncAsync()
    {
        const int parameter1 = 10;
        const string parameter2 = "testString";

        var originalResult = await TestStaticAsyncClass.TestMethodReturnWithParametersAsync(parameter1, parameter2);
        var expectedResult = originalResult / 2;

        ClassicAssert.AreEqual(parameter1, originalResult);

        Mock.Setup(typeof(TestStaticAsyncClass), nameof(TestStaticAsyncClass.TestMethodReturnWithParametersAsync),
                new SetupProperties
                {
                    MethodParametersTypes = new[]
                    {
                        typeof(int),
                        typeof(string)
                    }
                },
                () =>
                {
                    var actualResult = TestStaticAsyncClass.TestMethodReturnWithParametersAsync(parameter1, parameter2).GetAwaiter().GetResult();

                    ClassicAssert.AreNotEqual(originalResult, actualResult);
                    ClassicAssert.AreEqual(expectedResult, actualResult);
                })
            .Returns<int, string, Task<int>>(async (argument1, argument2) =>
            {
                ClassicAssert.AreEqual(parameter1, argument1);
                ClassicAssert.AreEqual(parameter2, argument2);
                return await Task.FromResult(argument1 / 2);
            });

        ClassicAssert.AreEqual(parameter1, originalResult);
    }

    [Test]
    public async Task TestSetupReturnsWithTestMethodReturnParameters3FuncAsync()
    {
        const int parameter1 = 10;
        const string parameter2 = "testString";
        const double parameter3 = 10;

        var originalResult = await TestStaticAsyncClass.TestMethodReturnWithParametersAsync(parameter1, parameter2, parameter3);
        var expectedResult = originalResult / 2;

        ClassicAssert.AreEqual(parameter1, originalResult);

        Mock.Setup(
            typeof(TestStaticAsyncClass), nameof(TestStaticAsyncClass.TestMethodReturnWithParametersAsync),
            new SetupProperties
            {
                MethodParametersTypes = new[]
                {
                    typeof(int),
                    typeof(string),
                    typeof(double)
                }
            },
            () =>
            {
                var actualResult = TestStaticAsyncClass.TestMethodReturnWithParametersAsync(parameter1, parameter2, parameter3).GetAwaiter().GetResult();

                ClassicAssert.AreNotEqual(originalResult, actualResult);
                ClassicAssert.AreEqual(expectedResult, actualResult);
            })
            .Returns<int, string, double, Task<int>>((argument1, argument2, argument3) =>
            {
                ClassicAssert.AreEqual(parameter1, argument1);
                ClassicAssert.AreEqual(parameter2, argument2);
                ClassicAssert.AreEqual(parameter3, argument3);
                return Task.FromResult(argument1 / 2);
            });

        ClassicAssert.AreEqual(parameter1, originalResult);
    }

    [Test]
    public async Task TestSetupReturnsWithTestMethodReturnParameters4FuncAsync()
    {
        const int parameter1 = 10;
        const string parameter2 = "testString";
        const double parameter3 = 10;
        var parameter4 = new[] { 10, 20 };

        var originalResult = await TestStaticAsyncClass.TestMethodReturnWithParametersAsync(
            parameter1,
            parameter2,
            parameter3,
            parameter4);

        var expectedResult = originalResult / 2;

        ClassicAssert.AreEqual(parameter1, originalResult);

        Mock.Setup(
                typeof(TestStaticAsyncClass), nameof(TestStaticAsyncClass.TestMethodReturnWithParametersAsync),
                new SetupProperties
                {
                    MethodParametersTypes = new[]
                    {
                        typeof(int),
                        typeof(string),
                        typeof(double),
                        typeof(int[]),
                    }
                },
                () =>
                {
                    var actualResult = TestStaticAsyncClass.TestMethodReturnWithParametersAsync(
                        parameter1,
                        parameter2,
                        parameter3,
                        parameter4).GetAwaiter().GetResult();

                    ClassicAssert.AreNotEqual(originalResult, actualResult);
                    ClassicAssert.AreEqual(expectedResult, actualResult);
                })
            .Returns<int, string, double, int[], Task<int>>(async (
                argument1,
                argument2,
                argument3,
                argument4) =>
            {
                ClassicAssert.AreEqual(parameter1, argument1);
                ClassicAssert.AreEqual(parameter2, argument2);
                ClassicAssert.AreEqual(parameter3, argument3);
                ClassicAssert.AreEqual(parameter4, argument4);
                return await Task.FromResult(argument1 / 2);
            });

        ClassicAssert.AreEqual(parameter1, originalResult);
    }

    [Test]
    public async Task TestSetupReturnsWithTestMethodReturnParameters5FuncAsync()
    {
        const int parameter1 = 10;
        const string parameter2 = "testString";
        const double parameter3 = 10;
        var parameter4 = new[] { 10, 20 };
        var parameter5 = new[] { parameter2, parameter1.ToString() };

        var originalResult = await TestStaticAsyncClass.TestMethodReturnWithParametersAsync(
            parameter1,
            parameter2,
            parameter3,
            parameter4,
            parameter5);

        var expectedResult = originalResult / 2;

        ClassicAssert.AreEqual(parameter1, originalResult);

        Mock.Setup(
                typeof(TestStaticAsyncClass), nameof(TestStaticAsyncClass.TestMethodReturnWithParametersAsync),
                new SetupProperties
                {
                    MethodParametersTypes = new[]
                    {
                        typeof(int),
                        typeof(string),
                        typeof(double),
                        typeof(int[]),
                        typeof(string[])
                    }
                },
                () =>
                {
                    var actualResult = TestStaticAsyncClass.TestMethodReturnWithParametersAsync(
                        parameter1,
                        parameter2,
                        parameter3,
                        parameter4,
                        parameter5).GetAwaiter().GetResult();

                    ClassicAssert.AreNotEqual(originalResult, actualResult);
                    ClassicAssert.AreEqual(expectedResult, actualResult);
                })
            .Returns<int, string, double, int[], string[], Task<int>>((
                argument1,
                argument2,
                argument3,
                argument4,
                argument5) =>
            {
                ClassicAssert.AreEqual(parameter1, argument1);
                ClassicAssert.AreEqual(parameter2, argument2);
                ClassicAssert.AreEqual(parameter3, argument3);
                ClassicAssert.AreEqual(parameter4, argument4);
                ClassicAssert.AreEqual(parameter5, argument5);
                return Task.FromResult(argument1 / 2);
            });

        ClassicAssert.AreEqual(parameter1, originalResult);
    }

    [Test]
    public async Task TestSetupReturnsWithTestMethodReturnParameters6FuncAsync()
    {
        const int parameter1 = 10;
        const string parameter2 = "testString";
        const double parameter3 = 10;
        var parameter4 = new[] { 10, 20 };
        var parameter5 = new[] { parameter2, parameter1.ToString() };
        const char parameter6 = 'a';

        var originalResult = await TestStaticAsyncClass.TestMethodReturnWithParametersAsync(
            parameter1,
            parameter2,
            parameter3,
            parameter4,
            parameter5,
            parameter6);

        var expectedResult = originalResult / 2;

        ClassicAssert.AreEqual(parameter1, originalResult);

        Mock.Setup(
                typeof(TestStaticAsyncClass), nameof(TestStaticAsyncClass.TestMethodReturnWithParametersAsync),
                new SetupProperties
                {
                    MethodParametersTypes = new[]
                    {
                        typeof(int),
                        typeof(string),
                        typeof(double),
                        typeof(int[]),
                        typeof(string[]),
                        typeof(char)
                    }
                },
                () =>
                {
                    var actualResult = TestStaticAsyncClass.TestMethodReturnWithParametersAsync(
                        parameter1,
                        parameter2,
                        parameter3,
                        parameter4,
                        parameter5,
                        parameter6).GetAwaiter().GetResult();

                    ClassicAssert.AreNotEqual(originalResult, actualResult);
                    ClassicAssert.AreEqual(expectedResult, actualResult);
                })
            .Returns<int, string, double, int[], string[], char, Task<int>>(async (
                argument1,
                argument2,
                argument3,
                argument4,
                argument5,
                argument6) =>
            {
                ClassicAssert.AreEqual(parameter1, argument1);
                ClassicAssert.AreEqual(parameter2, argument2);
                ClassicAssert.AreEqual(parameter3, argument3);
                ClassicAssert.AreEqual(parameter4, argument4);
                ClassicAssert.AreEqual(parameter5, argument5);
                ClassicAssert.AreEqual(parameter6, argument6);
                return await Task.FromResult(argument1 / 2);
            });

        ClassicAssert.AreEqual(parameter1, originalResult);
    }

    [Test]
    public async Task TestSetupReturnsWithTestMethodReturnParameters7FuncAsync()
    {
        const int parameter1 = 10;
        const string parameter2 = "testString";
        const double parameter3 = 10;
        var parameter4 = new[] { 10, 20 };
        var parameter5 = new[] { parameter2, parameter1.ToString() };
        const char parameter6 = 'a';
        const bool parameter7 = true;

        var originalResult = await TestStaticAsyncClass.TestMethodReturnWithParametersAsync(
            parameter1,
            parameter2,
            parameter3,
            parameter4,
            parameter5,
            parameter6,
            parameter7);

        var expectedResult = originalResult / 2;

        ClassicAssert.AreEqual(parameter1, originalResult);

        Mock.Setup(
                typeof(TestStaticAsyncClass), nameof(TestStaticAsyncClass.TestMethodReturnWithParametersAsync),
                new SetupProperties
                {
                    MethodParametersTypes = new[]
                    {
                        typeof(int),
                        typeof(string),
                        typeof(double),
                        typeof(int[]),
                        typeof(string[]),
                        typeof(char),
                        typeof(bool)
                    }
                },
                () =>
                {
                    var actualResult = TestStaticAsyncClass.TestMethodReturnWithParametersAsync(
                        parameter1,
                        parameter2,
                        parameter3,
                        parameter4,
                        parameter5,
                        parameter6,
                        parameter7).GetAwaiter().GetResult();

                    ClassicAssert.AreNotEqual(originalResult, actualResult);
                    ClassicAssert.AreEqual(expectedResult, actualResult);
                })
            .Returns<int, string, double, int[], string[], char, bool, Task<int>>((
                argument1,
                argument2,
                argument3,
                argument4,
                argument5,
                argument6,
                argument7) =>
            {
                ClassicAssert.AreEqual(parameter1, argument1);
                ClassicAssert.AreEqual(parameter2, argument2);
                ClassicAssert.AreEqual(parameter3, argument3);
                ClassicAssert.AreEqual(parameter4, argument4);
                ClassicAssert.AreEqual(parameter5, argument5);
                ClassicAssert.AreEqual(parameter6, argument6);
                ClassicAssert.AreEqual(parameter7, argument7);

                return Task.FromResult(argument1 / 2);
            });

        ClassicAssert.AreEqual(parameter1, originalResult);
    }

    [Test]
    public async Task TestSetupReturnsWithTestMethodReturnParameters8FuncAsync()
    {
        const int parameter1 = 10;
        const string parameter2 = "testString";
        const double parameter3 = 10;
        var parameter4 = new[] { 10, 20 };
        var parameter5 = new[] { parameter2, parameter1.ToString() };
        const char parameter6 = 'a';
        const bool parameter7 = true;
        var parameter8 = new TestInstance();

        var originalResult = await TestStaticAsyncClass.TestMethodReturnWithParametersAsync(
            parameter1,
            parameter2,
            parameter3,
            parameter4,
            parameter5,
            parameter6,
            parameter7,
            parameter8);

        var expectedResult = originalResult / 2;

        ClassicAssert.AreEqual(parameter1, originalResult);

        Mock.Setup(
                typeof(TestStaticAsyncClass), nameof(TestStaticAsyncClass.TestMethodReturnWithParametersAsync),
                new SetupProperties
                {
                    MethodParametersTypes = new[]
                    {
                        typeof(int),
                        typeof(string),
                        typeof(double),
                        typeof(int[]),
                        typeof(string[]),
                        typeof(char),
                        typeof(bool),
                        typeof(TestInstance)
                    }
                },
                () =>
                {
                    var actualResult = TestStaticAsyncClass.TestMethodReturnWithParametersAsync(
                        parameter1,
                        parameter2,
                        parameter3,
                        parameter4,
                        parameter5,
                        parameter6,
                        parameter7,
                        parameter8).GetAwaiter().GetResult();

                    ClassicAssert.AreNotEqual(originalResult, actualResult);
                    ClassicAssert.AreEqual(expectedResult, actualResult);
                })
            .Returns<int, string, double, int[], string[], char, bool, TestInstance, Task<int>>(async (
                argument1,
                argument2,
                argument3,
                argument4,
                argument5,
                argument6,
                argument7,
                argument8) =>
            {
                ClassicAssert.AreEqual(parameter1, argument1);
                ClassicAssert.AreEqual(parameter2, argument2);
                ClassicAssert.AreEqual(parameter3, argument3);
                ClassicAssert.AreEqual(parameter4, argument4);
                ClassicAssert.AreEqual(parameter5, argument5);
                ClassicAssert.AreEqual(parameter6, argument6);
                ClassicAssert.AreEqual(parameter7, argument7);
                ClassicAssert.AreEqual(parameter8, argument8);

                return await Task.FromResult(argument1 / 2);
            });

        ClassicAssert.AreEqual(parameter1, originalResult);
    }

    [Test]
    public async Task TestSetupReturnsWithTestMethodReturnParameters9FuncAsync()
    {
        const int parameter1 = 10;
        const string parameter2 = "testString";
        const double parameter3 = 10;
        var parameter4 = new[] { 10, 20 };
        var parameter5 = new[] { parameter2, parameter1.ToString() };
        const char parameter6 = 'a';
        const bool parameter7 = true;
        var parameter8 = new TestInstance();
        Func<int, int> parameter9 = x => x;

        var originalResult = await TestStaticAsyncClass.TestMethodReturnWithParametersAsync(
            parameter1,
            parameter2,
            parameter3,
            parameter4,
            parameter5,
            parameter6,
            parameter7,
            parameter8,
            parameter9);

        var expectedResult = originalResult / 2;

        ClassicAssert.AreEqual(parameter1, originalResult);

        Mock.Setup(
                typeof(TestStaticAsyncClass), nameof(TestStaticAsyncClass.TestMethodReturnWithParametersAsync),
                new SetupProperties
                {
                    MethodParametersTypes = new[]
                    {
                        typeof(int),
                        typeof(string),
                        typeof(double),
                        typeof(int[]),
                        typeof(string[]),
                        typeof(char),
                        typeof(bool),
                        typeof(TestInstance),
                        typeof(Func<int, int>)
                    }
                },
                () =>
                {
                    var actualResult = TestStaticAsyncClass.TestMethodReturnWithParametersAsync(
                        parameter1,
                        parameter2,
                        parameter3,
                        parameter4,
                        parameter5,
                        parameter6,
                        parameter7,
                        parameter8,
                        parameter9).GetAwaiter().GetResult();

                    ClassicAssert.AreNotEqual(originalResult, actualResult);
                    ClassicAssert.AreEqual(expectedResult, actualResult);
                })
            .Returns<int, string, double, int[], string[], char, bool, TestInstance, Func<int, int>, Task<int>>((
                argument1,
                argument2,
                argument3,
                argument4,
                argument5,
                argument6,
                argument7,
                argument8,
                argument9) =>
            {
                ClassicAssert.AreEqual(parameter1, argument1);
                ClassicAssert.AreEqual(parameter2, argument2);
                ClassicAssert.AreEqual(parameter3, argument3);
                ClassicAssert.AreEqual(parameter4, argument4);
                ClassicAssert.AreEqual(parameter5, argument5);
                ClassicAssert.AreEqual(parameter6, argument6);
                ClassicAssert.AreEqual(parameter7, argument7);
                ClassicAssert.AreEqual(parameter8, argument8);
                ClassicAssert.AreEqual(parameter9, argument9);
                return Task.FromResult(argument1 / 2);
            });

        ClassicAssert.AreEqual(parameter1, originalResult);
    }
}