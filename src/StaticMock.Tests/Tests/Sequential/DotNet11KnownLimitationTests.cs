using NUnit.Framework;

namespace StaticMock.Tests.Tests.Sequential;

/// <summary>
/// Known limitation of MonoMod.Core 1.4.0-prerelease.3: on the .NET 11 runtime it cannot hook a generic method
/// instantiation that returns Task, Task&lt;T&gt;, ValueTask or ValueTask&lt;T&gt; (for example Task.FromResult&lt;int&gt;).
/// SMock throws a clear <see cref="NotSupportedException"/> and keeps the MonoMod exception as the inner exception.
/// These tests fail once MonoMod supports the case. Then remove the .NET 11 skips in
/// <c>AsyncExamples</c> (<c>Mock_Task_FromResult</c>, <c>Mock_Async_Return_Values</c>,
/// <c>Mock_Multiple_Async_Operations</c>), these tests and the known-limitation notes in the docs.
/// </summary>
[TestFixture]
public class DotNet11KnownLimitationTests
{
    private const string SkipReason = "The known limitation exists only on the .NET 11 runtime";

    [Test]
    public void TestSetupGenericTaskMethodReturnsThrowsClearNotSupportedExceptionOnDotNet11()
    {
        Assume.That(Environment.Version.Major, Is.GreaterThanOrEqualTo(11), SkipReason);

        var exception = Assert.Throws<NotSupportedException>(
            () => Mock.Setup(() => Task.FromResult(42)).Returns(Task.FromResult(100)));

        AssertClearLimitationException(exception, "FromResult");
    }

    [Test]
    public void TestSetupGenericTaskMethodReturnsAsyncThrowsClearNotSupportedExceptionOnDotNet11()
    {
        Assume.That(Environment.Version.Major, Is.GreaterThanOrEqualTo(11), SkipReason);

        var exception = Assert.Throws<NotSupportedException>(
            () => Mock.Setup(context => Task.FromResult<int>(context.It.IsAny<int>())).ReturnsAsync(50));

        AssertClearLimitationException(exception, "FromResult");
    }

#if NET11_0_OR_GREATER
    [Test]
    public void TestSetupGenericValueTaskMethodReturnsThrowsClearNotSupportedExceptionOnDotNet11()
    {
        Assume.That(Environment.Version.Major, Is.GreaterThanOrEqualTo(11), SkipReason);

        var exception = Assert.Throws<NotSupportedException>(
            () => Mock.Setup(() => ValueTask.FromResult(42)).Returns(new ValueTask<int>(100)));

        AssertClearLimitationException(exception, "FromResult");
    }
#endif

    [Test]
    public void TestSetupNonGenericTaskMethodStillWorksOnDotNet11()
    {
        Assume.That(Environment.Version.Major, Is.GreaterThanOrEqualTo(11), SkipReason);

        using var mock = Mock.Setup(context => Task.Delay(context.It.IsAny<int>()))
            .Returns(Task.CompletedTask);

        // A real Task.Delay(1000) is not completed yet. The mocked call returns a completed task.
        Assert.That(Task.Delay(1000).IsCompleted, Is.True);
    }

    private static void AssertClearLimitationException(NotSupportedException exception, string methodName)
    {
        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.Message, Does.Contain(methodName));
        Assert.That(exception.Message, Does.Contain("MonoMod.Core 1.4.0-prerelease.3"));
        Assert.That(exception.Message, Does.Contain("Task, Task<T>, ValueTask or ValueTask<T>"));
        Assert.That(exception.InnerException, Is.TypeOf<NotSupportedException>());
    }
}
