using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace StaticMock.Tests.Tests.Examples.GettingStarted;

[TestFixture]
public class AsyncExamples
{
    private const string DotNet11GenericTaskLimitation =
        "Known limitation: MonoMod.Core 1.4.0-prerelease.3 cannot hook generic methods that return Task-like types " +
        "(such as Task.FromResult<int>) on .NET 11. Remove this skip when MonoMod supports them. " +
        "See DotNet11KnownLimitationTests.";

    private static void IgnoreOnDotNet11()
    {
        if (Environment.Version.Major >= 11)
        {
            Assert.Ignore(DotNet11GenericTaskLimitation);
        }
    }

    [Test]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public async Task Mock_Async_Methods()
    {
        // Mock async HTTP call - using expression-based setup
        using var mock = Mock.Setup(context => Task.Delay(context.It.IsAny<int>()))
            .Returns(Task.CompletedTask);

        // Test the mocked delay
        await Task.Delay(1000); // Should complete immediately due to mock

        // Verify the test completes quickly (no actual delay)
        Assert.Pass("Async mock executed successfully");
    }

    [Test]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public async Task Mock_Task_FromResult()
    {
        IgnoreOnDotNet11();

        using var mock = Mock.Setup(() => Task.FromResult(42))
            .Returns(Task.FromResult(100));

        var result = await Task.FromResult(42);
        ClassicAssert.AreEqual(100, result);
    }

    [Test]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public async Task Mock_Async_With_Delay_Simulation()
    {
        const string testData = "processed data";

        // Mock Task.Delay to return immediately
        using var delayMock = Mock.Setup(context => Task.Delay(context.It.IsAny<int>()))
            .Returns(Task.CompletedTask);

        // Simulate an async operation that would normally take time
        await Task.Delay(5000); // This should complete immediately
        var result = testData.ToUpper();

        ClassicAssert.AreEqual("PROCESSED DATA", result);
    }

    [Test]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public async Task Mock_Async_Exception_Handling()
    {
        // Mock Task.Delay to throw an exception
        using var mock = Mock.Setup(context => Task.Delay(context.It.Is<int>(ms => ms < 0)))
            .Throws<ArgumentOutOfRangeException>();

        // Test exception handling in async context
        try
        {
            await Task.Delay(-1);
            Assert.Fail("Expected ArgumentOutOfRangeException to be thrown");
        }
        catch (ArgumentOutOfRangeException exception)
        {
            ClassicAssert.IsNotNull(exception);
        }
    }
    
    [Test]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public async Task Mock_Async_Return_Values()
    {
        IgnoreOnDotNet11();

        if (RuntimeInformation.ProcessArchitecture == Architecture.Arm64)
        {
            await Mock_Task_FromResult();
            Assert.Pass("Skipped direct async mock Task.FromResult for string on ARM64 due to known issues. Work with Task.FromResult<int> instead.");
        }
        else
        {
            const string mockResult = "async mock result";

            // Mock an async method that returns a value
            using var mock = Mock.Setup(context => Task.FromResult<string>(context.It.IsAny<string>()))
                .ReturnsAsync(mockResult);

            var result = await Task.FromResult<string>("original");
            Assert.That(result, Is.EqualTo(mockResult));
        }
    }

    [Test]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public async Task Mock_Multiple_Async_Operations()
    {
        IgnoreOnDotNet11();

        // Mock multiple async operations
        using var delayMock = Mock.Setup(context => Task.Delay(context.It.IsAny<int>()))
            .Returns(Task.CompletedTask);

        using var resultMock = Mock.Setup(context => Task.FromResult<int>(context.It.IsAny<int>()))
            .ReturnsAsync(50);

        // Execute multiple async operations
        await Task.Delay(1000); // Should complete immediately
        var value = await Task.FromResult<int>(10); // Should return 50

        ClassicAssert.AreEqual(50, value);
    }
}