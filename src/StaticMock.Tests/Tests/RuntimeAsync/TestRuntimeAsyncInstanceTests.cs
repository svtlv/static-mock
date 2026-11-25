#if NET11_0_OR_GREATER
using System.Reflection;
using NUnit.Framework;
using StaticMock.Tests.Common.TestEntities;

namespace StaticMock.Tests.Tests.RuntimeAsync;

/// <summary>
/// Checks the instance runtime-async test entity without any mock: the compiler marker, real suspension
/// and the original behavior. The interception tests are in the sequential and hierarchical
/// <c>RuntimeAsyncMockTests</c> fixtures.
/// </summary>
[TestFixture]
public class TestRuntimeAsyncInstanceTests
{
    private const BindingFlags InstanceMethods = BindingFlags.Instance;

    private static string[] RuntimeAsyncMethodNames { get; } =
        RuntimeAsyncMarker.GetRuntimeAsyncMethodNames(typeof(TestRuntimeAsyncInstance), InstanceMethods);

    [Test]
    public void TestEntityHasRuntimeAsyncMethodsForAllFourReturnTypes()
    {
        var returnTypes = typeof(TestRuntimeAsyncInstance)
            .GetMethods(InstanceMethods | BindingFlags.Public | BindingFlags.DeclaredOnly)
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
        var method = typeof(TestRuntimeAsyncInstance).GetMethod(methodName, InstanceMethods | BindingFlags.Public)!;

        Assert.That(RuntimeAsyncMarker.IsRuntimeAsync(method), Is.True, $"{methodName} must be a runtime-async method");
    }

    [Test]
    public void TestNormalCallerDoesNotCarryRuntimeAsyncMarker()
    {
        var method = typeof(TestRuntimeAsyncInstance)
            .GetMethod(nameof(TestRuntimeAsyncInstance.TestNormalCallerReturnTaskOfIntAsync), InstanceMethods | BindingFlags.Public)!;

        Assert.That(RuntimeAsyncMarker.IsRuntimeAsync(method), Is.False);
    }

    [Test]
    public async Task TestMethodReturnTaskCompletes()
    {
        await new TestRuntimeAsyncInstance().TestMethodReturnTaskAsync();
    }

    [Test]
    public async Task TestMethodReturnTaskOfIntReturnsOriginalResult()
    {
        Assert.That(await new TestRuntimeAsyncInstance().TestMethodReturnTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncInstance.OriginalResult));
    }

    [Test]
    public async Task TestMethodReturnValueTaskCompletes()
    {
        await new TestRuntimeAsyncInstance().TestMethodReturnValueTaskAsync();
    }

    [Test]
    public async Task TestMethodReturnValueTaskOfIntReturnsOriginalResult()
    {
        Assert.That(await new TestRuntimeAsyncInstance().TestMethodReturnValueTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncInstance.OriginalResult));
    }

    [Test]
    public async Task TestMethodWithParameterReturnsParameter()
    {
        var instance = new TestRuntimeAsyncInstance();

        Assert.That(await instance.TestMethodReturnTaskOfIntWithParameterAsync(7), Is.EqualTo(7));
        Assert.That(await instance.TestMethodReturnValueTaskOfIntWithParameterAsync(8), Is.EqualTo(8));
    }

    [Test]
    public async Task TestMethodAwaitGateReturnTaskSuspendsUntilGateCompletes()
    {
        var gate = new TaskCompletionSource();

        var task = new TestRuntimeAsyncInstance().TestMethodAwaitGateReturnTaskAsync(gate.Task);
        Assert.That(task.IsCompleted, Is.False, "The method must suspend on the incomplete gate");

        gate.SetResult();
        await task;
    }

    [Test]
    public async Task TestMethodAwaitGateReturnTaskOfIntSuspendsUntilGateCompletes()
    {
        var gate = new TaskCompletionSource();

        var task = new TestRuntimeAsyncInstance().TestMethodAwaitGateReturnTaskOfIntAsync(gate.Task);
        Assert.That(task.IsCompleted, Is.False, "The method must suspend on the incomplete gate");

        gate.SetResult();
        Assert.That(await task, Is.EqualTo(TestRuntimeAsyncInstance.OriginalResult));
    }

    [Test]
    public async Task TestMethodAwaitGateReturnValueTaskSuspendsUntilGateCompletes()
    {
        var gate = new TaskCompletionSource();

        var task = new TestRuntimeAsyncInstance().TestMethodAwaitGateReturnValueTaskAsync(gate.Task);
        Assert.That(task.IsCompleted, Is.False, "The method must suspend on the incomplete gate");

        gate.SetResult();
        await task;
    }

    [Test]
    public async Task TestMethodAwaitGateReturnValueTaskOfIntSuspendsUntilGateCompletes()
    {
        var gate = new TaskCompletionSource();

        var task = new TestRuntimeAsyncInstance().TestMethodAwaitGateReturnValueTaskOfIntAsync(gate.Task);
        Assert.That(task.IsCompleted, Is.False, "The method must suspend on the incomplete gate");

        gate.SetResult();
        Assert.That(await task, Is.EqualTo(TestRuntimeAsyncInstance.OriginalResult));
    }

    [Test]
    public async Task TestMethodThrowsAfterSuspensionReturnTaskThrows()
    {
        var instance = new TestRuntimeAsyncInstance();

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await instance.TestMethodThrowsAfterSuspensionReturnTaskAsync());

        Assert.That(exception.Message, Is.EqualTo(TestRuntimeAsyncInstance.ExceptionMessage));
    }

    [Test]
    public async Task TestMethodThrowsAfterSuspensionReturnTaskOfIntThrows()
    {
        var instance = new TestRuntimeAsyncInstance();

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await instance.TestMethodThrowsAfterSuspensionReturnTaskOfIntAsync());

        Assert.That(exception.Message, Is.EqualTo(TestRuntimeAsyncInstance.ExceptionMessage));
    }

    [Test]
    public async Task TestMethodThrowsAfterSuspensionReturnValueTaskThrows()
    {
        var instance = new TestRuntimeAsyncInstance();

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await instance.TestMethodThrowsAfterSuspensionReturnValueTaskAsync());

        Assert.That(exception.Message, Is.EqualTo(TestRuntimeAsyncInstance.ExceptionMessage));
    }

    [Test]
    public async Task TestMethodThrowsAfterSuspensionReturnValueTaskOfIntThrows()
    {
        var instance = new TestRuntimeAsyncInstance();

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await instance.TestMethodThrowsAfterSuspensionReturnValueTaskOfIntAsync());

        Assert.That(exception.Message, Is.EqualTo(TestRuntimeAsyncInstance.ExceptionMessage));
    }

    [Test]
    public async Task TestCallersReturnResultsOfAwaitedMethods()
    {
        var instance = new TestRuntimeAsyncInstance();

        Assert.That(await instance.TestCallerAwaitsTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncInstance.OriginalResult + 1));
        Assert.That(await instance.TestCallerAwaitsValueTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncInstance.OriginalResult + 1));
        Assert.That(await instance.TestNormalCallerReturnTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncInstance.OriginalResult));

        await instance.TestCallerAwaitsTaskAsync();
        await instance.TestCallerAwaitsValueTaskAsync();
    }

    [Test]
    public async Task TestVoidMethodCompletesAfterSuspension()
    {
        var completion = new TaskCompletionSource();

        new TestRuntimeAsyncInstance().TestVoidMethodAsync(completion);

        await completion.Task.WaitAsync(TimeSpan.FromSeconds(10));
    }

    [Test]
    public async Task TestVoidMethodWithParameterCompletesAfterSuspension()
    {
        var completion = new TaskCompletionSource<int>();

        new TestRuntimeAsyncInstance().TestVoidMethodWithParameterAsync(5, completion);

        Assert.That(await completion.Task.WaitAsync(TimeSpan.FromSeconds(10)), Is.EqualTo(5));
    }
}
#endif
