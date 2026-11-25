#if NET11_0_OR_GREATER
using System.Reflection;
using NUnit.Framework;
using StaticMock.Tests.Common.TestEntities;

namespace StaticMock.Tests.Tests.RuntimeAsync;

/// <summary>
/// Checks the static runtime-async test entity without any mock: the compiler marker, real suspension
/// and the original behavior. The interception tests are in the sequential and hierarchical
/// <c>RuntimeAsyncMockTests</c> fixtures.
/// </summary>
[TestFixture]
public class TestRuntimeAsyncClassTests
{
    private const BindingFlags StaticMethods = BindingFlags.Static;

    private static string[] RuntimeAsyncMethodNames { get; } =
        RuntimeAsyncMarker.GetRuntimeAsyncMethodNames(typeof(TestRuntimeAsyncClass), StaticMethods);

    [Test]
    public void TestEntityHasRuntimeAsyncMethodsForAllFourReturnTypes()
    {
        var returnTypes = typeof(TestRuntimeAsyncClass)
            .GetMethods(StaticMethods | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .Select(method => method.ReturnType)
            .ToHashSet();

        Assert.That(returnTypes, Does.Contain(typeof(Task)));
        Assert.That(returnTypes, Does.Contain(typeof(Task<int>)));
        Assert.That(returnTypes, Does.Contain(typeof(ValueTask)));
        Assert.That(returnTypes, Does.Contain(typeof(ValueTask<int>)));
    }

    [TestCaseSource(nameof(RuntimeAsyncMethodNames))]
    public void TestMethodCarriesRuntimeAsyncMarker(string methodName)
    {
        var method = typeof(TestRuntimeAsyncClass).GetMethod(methodName, StaticMethods | BindingFlags.Public)!;

        Assert.That(RuntimeAsyncMarker.IsRuntimeAsync(method), Is.True, $"{methodName} must be a runtime-async method");
    }

    [Test]
    public void TestNormalCallerDoesNotCarryRuntimeAsyncMarker()
    {
        var method = typeof(TestRuntimeAsyncClass)
            .GetMethod(nameof(TestRuntimeAsyncClass.TestNormalCallerReturnTaskOfIntAsync), StaticMethods | BindingFlags.Public)!;

        Assert.That(RuntimeAsyncMarker.IsRuntimeAsync(method), Is.False);
    }

    [Test]
    public async Task TestMethodReturnTaskCompletes()
    {
        await TestRuntimeAsyncClass.TestMethodReturnTaskAsync();
    }

    [Test]
    public async Task TestMethodReturnTaskOfIntReturnsOriginalResult()
    {
        Assert.That(await TestRuntimeAsyncClass.TestMethodReturnTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncClass.OriginalResult));
    }

    [Test]
    public async Task TestMethodReturnValueTaskCompletes()
    {
        await TestRuntimeAsyncClass.TestMethodReturnValueTaskAsync();
    }

    [Test]
    public async Task TestMethodReturnValueTaskOfIntReturnsOriginalResult()
    {
        Assert.That(await TestRuntimeAsyncClass.TestMethodReturnValueTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncClass.OriginalResult));
    }

    [Test]
    public async Task TestMethodWithParameterReturnsParameter()
    {
        Assert.That(await TestRuntimeAsyncClass.TestMethodReturnTaskOfIntWithParameterAsync(7), Is.EqualTo(7));
        Assert.That(await TestRuntimeAsyncClass.TestMethodReturnValueTaskOfIntWithParameterAsync(8), Is.EqualTo(8));
    }

    [Test]
    public async Task TestMethodAwaitGateReturnTaskSuspendsUntilGateCompletes()
    {
        var gate = new TaskCompletionSource();

        var task = TestRuntimeAsyncClass.TestMethodAwaitGateReturnTaskAsync(gate.Task);
        Assert.That(task.IsCompleted, Is.False, "The method must suspend on the incomplete gate");

        gate.SetResult();
        await task;
    }

    [Test]
    public async Task TestMethodAwaitGateReturnTaskOfIntSuspendsUntilGateCompletes()
    {
        var gate = new TaskCompletionSource();

        var task = TestRuntimeAsyncClass.TestMethodAwaitGateReturnTaskOfIntAsync(gate.Task);
        Assert.That(task.IsCompleted, Is.False, "The method must suspend on the incomplete gate");

        gate.SetResult();
        Assert.That(await task, Is.EqualTo(TestRuntimeAsyncClass.OriginalResult));
    }

    [Test]
    public async Task TestMethodAwaitGateReturnValueTaskSuspendsUntilGateCompletes()
    {
        var gate = new TaskCompletionSource();

        var task = TestRuntimeAsyncClass.TestMethodAwaitGateReturnValueTaskAsync(gate.Task);
        Assert.That(task.IsCompleted, Is.False, "The method must suspend on the incomplete gate");

        gate.SetResult();
        await task;
    }

    [Test]
    public async Task TestMethodAwaitGateReturnValueTaskOfIntSuspendsUntilGateCompletes()
    {
        var gate = new TaskCompletionSource();

        var task = TestRuntimeAsyncClass.TestMethodAwaitGateReturnValueTaskOfIntAsync(gate.Task);
        Assert.That(task.IsCompleted, Is.False, "The method must suspend on the incomplete gate");

        gate.SetResult();
        Assert.That(await task, Is.EqualTo(TestRuntimeAsyncClass.OriginalResult));
    }

    [Test]
    public async Task TestMethodThrowsAfterSuspensionReturnTaskThrows()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await TestRuntimeAsyncClass.TestMethodThrowsAfterSuspensionReturnTaskAsync());

        Assert.That(exception.Message, Is.EqualTo(TestRuntimeAsyncClass.ExceptionMessage));
    }

    [Test]
    public async Task TestMethodThrowsAfterSuspensionReturnTaskOfIntThrows()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await TestRuntimeAsyncClass.TestMethodThrowsAfterSuspensionReturnTaskOfIntAsync());

        Assert.That(exception.Message, Is.EqualTo(TestRuntimeAsyncClass.ExceptionMessage));
    }

    [Test]
    public async Task TestMethodThrowsAfterSuspensionReturnValueTaskThrows()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await TestRuntimeAsyncClass.TestMethodThrowsAfterSuspensionReturnValueTaskAsync());

        Assert.That(exception.Message, Is.EqualTo(TestRuntimeAsyncClass.ExceptionMessage));
    }

    [Test]
    public async Task TestMethodThrowsAfterSuspensionReturnValueTaskOfIntThrows()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await TestRuntimeAsyncClass.TestMethodThrowsAfterSuspensionReturnValueTaskOfIntAsync());

        Assert.That(exception.Message, Is.EqualTo(TestRuntimeAsyncClass.ExceptionMessage));
    }

    [Test]
    public async Task TestCallersReturnResultsOfAwaitedMethods()
    {
        Assert.That(await TestRuntimeAsyncClass.TestCallerAwaitsTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncClass.OriginalResult + 1));
        Assert.That(await TestRuntimeAsyncClass.TestCallerAwaitsValueTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncClass.OriginalResult + 1));
        Assert.That(await TestRuntimeAsyncClass.TestNormalCallerReturnTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncClass.OriginalResult));

        await TestRuntimeAsyncClass.TestCallerAwaitsTaskAsync();
        await TestRuntimeAsyncClass.TestCallerAwaitsValueTaskAsync();
    }

    [Test]
    public async Task TestVoidMethodCompletesAfterSuspension()
    {
        var completion = new TaskCompletionSource();

        TestRuntimeAsyncClass.TestVoidMethodAsync(completion);

        await completion.Task.WaitAsync(TimeSpan.FromSeconds(10));
    }

    [Test]
    public async Task TestVoidMethodWithParameterCompletesAfterSuspension()
    {
        var completion = new TaskCompletionSource<int>();

        TestRuntimeAsyncClass.TestVoidMethodWithParameterAsync(5, completion);

        Assert.That(await completion.Task.WaitAsync(TimeSpan.FromSeconds(10)), Is.EqualTo(5));
    }
}
#endif
