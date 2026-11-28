using NUnit.Framework;
using StaticMock.Tests.Common.TestEntities;

namespace StaticMock.Tests.Tests.Hierarchical.ValidationTests;

/// <summary>
/// The validation parameter of the hierarchical <c>Mock.Setup</c> is an <see cref="Action"/>.
/// An <c>async</c> lambda here becomes <c>async void</c>: the hook can end before the assertion finishes.
/// These tests show that the validation stays synchronous and ends before the hook ends. They block on pending
/// tasks only because NUnit runs them without a SynchronizationContext and on the default TaskScheduler.
/// Application code must not do this: prefer the sequential API and await the call.
/// </summary>
[TestFixture]
public class HierarchicalAsyncValidationTests
{
    [Test]
    public async Task TestValidationThatSuspendsCompletesBeforeHookEnds()
    {
        var validationCompleted = false;
        var gate = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        Mock.Setup(() => TestStaticAsyncClass.TestMethodReturnTaskWithoutParametersAsync(), () =>
        {
            // The call below really suspends: it awaits a task that is not complete yet.
            var pendingCall = CallAfterGateAsync(gate.Task);
            Assert.That(pendingCall.IsCompleted, Is.False);

            // The call resumes on another thread. The validation blocks until it ends, so the hook is still active.
            gate.SetResult(true);
            var actualResult = pendingCall.GetAwaiter().GetResult();

            Assert.That(actualResult, Is.EqualTo(2));
            validationCompleted = true;
        }).ReturnsAsync(2);

        Assert.That(validationCompleted, Is.True);

        var originalResult = await TestStaticAsyncClass.TestMethodReturnTaskWithoutParametersAsync();
        Assert.That(originalResult, Is.EqualTo(1));
    }

    [Test]
    public async Task TestValidationThatYieldsCompletesBeforeHookEnds()
    {
        var validationCompleted = false;

        Mock.Setup(() => TestStaticAsyncClass.TestMethodReturnTaskWithoutParametersAsync(), () =>
        {
            var actualResult = CallAfterYieldAsync().GetAwaiter().GetResult();

            Assert.That(actualResult, Is.EqualTo(2));
            validationCompleted = true;
        }).ReturnsAsync(2);

        Assert.That(validationCompleted, Is.True);

        var originalResult = await TestStaticAsyncClass.TestMethodReturnTaskWithoutParametersAsync();
        Assert.That(originalResult, Is.EqualTo(1));
    }

    [Test]
    public async Task TestFailingAssertionInValidationReachesTestResult()
    {
        var exception = Assert.Throws<AssertionException>(() =>
            Mock.Setup(() => TestStaticAsyncClass.TestMethodReturnTaskWithoutParametersAsync(), () =>
            {
                var actualResult = TestStaticAsyncClass.TestMethodReturnTaskWithoutParametersAsync().GetAwaiter().GetResult();

                Assert.That(actualResult, Is.EqualTo(-1), "validation failure marker");
            }).ReturnsAsync(2));

        Assert.That(exception.Message, Does.Contain("validation failure marker"));

        // The hook ends even when the validation fails.
        var originalResult = await TestStaticAsyncClass.TestMethodReturnTaskWithoutParametersAsync();
        Assert.That(originalResult, Is.EqualTo(1));
    }

    [Test]
    public async Task TestFailingAssertionAfterSuspensionInValidationReachesTestResult()
    {
        var gate = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        var exception = Assert.Throws<AssertionException>(() =>
            Mock.Setup(() => TestStaticAsyncClass.TestMethodReturnTaskWithoutParametersAsync(), () =>
            {
                var pendingCall = CallAfterGateAsync(gate.Task);
                gate.SetResult(true);
                var actualResult = pendingCall.GetAwaiter().GetResult();

                Assert.That(actualResult, Is.EqualTo(-1), "failure after suspension marker");
            }).ReturnsAsync(2));

        Assert.That(exception.Message, Does.Contain("failure after suspension marker"));

        var originalResult = await TestStaticAsyncClass.TestMethodReturnTaskWithoutParametersAsync();
        Assert.That(originalResult, Is.EqualTo(1));
    }

    [Test]
    public async Task TestFailingAsyncAssertionInValidationReachesTestResult()
    {
        // The mocked method does not throw, so the async assertion fails inside the validation.
        var exception = Assert.Throws<AssertionException>(() =>
            Mock.Setup(() => TestStaticAsyncClass.TestMethodReturnTaskWithoutParametersAsync(), () =>
            {
                Assert.ThrowsAsync<InvalidOperationException>(() => TestStaticAsyncClass.TestMethodReturnTaskWithoutParametersAsync())
                    .GetAwaiter().GetResult();
            }).ReturnsAsync(2));

        Assert.That(exception.Message, Does.Contain(nameof(InvalidOperationException)));

        var originalResult = await TestStaticAsyncClass.TestMethodReturnTaskWithoutParametersAsync();
        Assert.That(originalResult, Is.EqualTo(1));
    }

    private static async Task<int> CallAfterGateAsync(Task gate)
    {
        await gate;
        return await TestStaticAsyncClass.TestMethodReturnTaskWithoutParametersAsync();
    }

    private static async Task<int> CallAfterYieldAsync()
    {
        await Task.Yield();
        return await TestStaticAsyncClass.TestMethodReturnTaskWithoutParametersAsync();
    }
}
