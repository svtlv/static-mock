#if NET11_0_OR_GREATER
using NUnit.Framework;
using StaticMock.Tests.Common.TestEntities;

namespace StaticMock.Tests.Tests.Sequential.RuntimeAsyncTests;

/// <summary>
/// Sequential API (<c>using var</c>): a mock lives until the returned object is disposed.
/// The tests mock runtime-async methods (compiler feature <c>runtime-async=on</c>) of <see cref="TestRuntimeAsyncClass"/>
/// and <see cref="TestRuntimeAsyncInstance"/> for <c>Task</c>, <c>Task&lt;T&gt;</c>, <c>ValueTask</c> and <c>ValueTask&lt;T&gt;</c>:
/// mocked return, <c>Throws</c>, original behavior after the mock ends, calls from a normal and from a runtime-async caller,
/// a suspended original call, and <c>Callback</c> on <c>async void</c> methods (not runtime-async methods).
/// Each mock is disposed before the test ends, so the original methods work again in the next test.
/// </summary>
[TestFixture]
public class RuntimeAsyncMockTests
{
    private const int MockedResult = 2;

    // ---- Static methods: TestRuntimeAsyncClass ----

    [Test]
    public async Task TestStaticReturnsTask()
    {
        var calls = 0;
        await TestRuntimeAsyncClass.TestMethodReturnTaskAsync();

        using (Mock.Setup(() => TestRuntimeAsyncClass.TestMethodReturnTaskAsync()).Returns(() => { calls++; return Task.CompletedTask; }))
        {
            var task = TestRuntimeAsyncClass.TestMethodReturnTaskAsync();
            Assert.That(task.IsCompleted, Is.True, "The mock must return a completed task without running the original method");
            await task;
            Assert.That(calls, Is.EqualTo(1));
        }

        await TestRuntimeAsyncClass.TestMethodReturnTaskAsync();
    }

    [Test]
    public async Task TestStaticThrowsTask()
    {
        await TestRuntimeAsyncClass.TestMethodReturnTaskAsync();

        using (Mock.Setup(() => TestRuntimeAsyncClass.TestMethodReturnTaskAsync()).Throws<ArgumentException>())
        {
            await Assert.ThrowsAsync<ArgumentException>(async () => await TestRuntimeAsyncClass.TestMethodReturnTaskAsync());
        }

        await TestRuntimeAsyncClass.TestMethodReturnTaskAsync();
    }

    [Test]
    public async Task TestStaticInterceptedFromRuntimeAsyncCallerTask()
    {
        var calls = 0;
        await TestRuntimeAsyncClass.TestCallerAwaitsTaskAsync();

        using (Mock.Setup(() => TestRuntimeAsyncClass.TestMethodReturnTaskAsync()).Returns(() => { calls++; return Task.CompletedTask; }))
        {
            await TestRuntimeAsyncClass.TestCallerAwaitsTaskAsync();
            Assert.That(calls, Is.EqualTo(1));
        }

        await TestRuntimeAsyncClass.TestCallerAwaitsTaskAsync();
    }

    [Test]
    public async Task TestStaticSuspendedOriginalCallTaskFinishesWithOriginalResultAfterMockInstalled()
    {
        var calls = 0;
        var gate = new TaskCompletionSource();
        var suspended = TestRuntimeAsyncClass.TestMethodAwaitGateReturnTaskAsync(gate.Task);
        Assert.That(suspended.IsCompleted, Is.False, "The original call must suspend on the incomplete gate");

        using (Mock.Setup(context => TestRuntimeAsyncClass.TestMethodAwaitGateReturnTaskAsync(context.It.IsAny<Task>())).Returns(() => { calls++; return Task.CompletedTask; }))
        {
            var newCall = TestRuntimeAsyncClass.TestMethodAwaitGateReturnTaskAsync(new TaskCompletionSource().Task);
            Assert.That(newCall.IsCompleted, Is.True, "A new call must reach the mock and must not suspend");
            await newCall;
            Assert.That(calls, Is.EqualTo(1));

            gate.SetResult();
            await suspended;
        }
    }

    [Test]
    public async Task TestStaticReturnsTaskOfInt()
    {
        Assert.That(await TestRuntimeAsyncClass.TestMethodReturnTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncClass.OriginalResult));

        using (Mock.Setup(() => TestRuntimeAsyncClass.TestMethodReturnTaskOfIntAsync()).ReturnsAsync(MockedResult))
        {
            var task = TestRuntimeAsyncClass.TestMethodReturnTaskOfIntAsync();
            Assert.That(task.IsCompleted, Is.True, "The mock must return a completed task without running the original method");
            Assert.That(await task, Is.EqualTo(MockedResult));
        }

        Assert.That(await TestRuntimeAsyncClass.TestMethodReturnTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncClass.OriginalResult));
    }

    [Test]
    public async Task TestStaticThrowsTaskOfInt()
    {
        Assert.That(await TestRuntimeAsyncClass.TestMethodReturnTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncClass.OriginalResult));

        using (Mock.Setup(() => TestRuntimeAsyncClass.TestMethodReturnTaskOfIntAsync()).Throws<ArgumentException>())
        {
            await Assert.ThrowsAsync<ArgumentException>(async () => await TestRuntimeAsyncClass.TestMethodReturnTaskOfIntAsync());
        }

        Assert.That(await TestRuntimeAsyncClass.TestMethodReturnTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncClass.OriginalResult));
    }

    [Test]
    public async Task TestStaticInterceptedFromRuntimeAsyncCallerTaskOfInt()
    {
        Assert.That(await TestRuntimeAsyncClass.TestCallerAwaitsTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncClass.OriginalResult + 1));

        using (Mock.Setup(() => TestRuntimeAsyncClass.TestMethodReturnTaskOfIntAsync()).ReturnsAsync(MockedResult))
        {
            Assert.That(await TestRuntimeAsyncClass.TestCallerAwaitsTaskOfIntAsync(), Is.EqualTo(MockedResult + 1));
        }

        Assert.That(await TestRuntimeAsyncClass.TestCallerAwaitsTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncClass.OriginalResult + 1));
    }

    [Test]
    public async Task TestStaticSuspendedOriginalCallTaskOfIntFinishesWithOriginalResultAfterMockInstalled()
    {
        var gate = new TaskCompletionSource();
        var suspended = TestRuntimeAsyncClass.TestMethodAwaitGateReturnTaskOfIntAsync(gate.Task);
        Assert.That(suspended.IsCompleted, Is.False, "The original call must suspend on the incomplete gate");

        using (Mock.Setup(context => TestRuntimeAsyncClass.TestMethodAwaitGateReturnTaskOfIntAsync(context.It.IsAny<Task>())).ReturnsAsync(MockedResult))
        {
            var newCall = TestRuntimeAsyncClass.TestMethodAwaitGateReturnTaskOfIntAsync(new TaskCompletionSource().Task);
            Assert.That(newCall.IsCompleted, Is.True, "A new call must reach the mock and must not suspend");
            Assert.That(await newCall, Is.EqualTo(MockedResult));

            gate.SetResult();
            Assert.That(await suspended, Is.EqualTo(TestRuntimeAsyncClass.OriginalResult));
        }
    }

    [Test]
    public async Task TestStaticReturnsValueTask()
    {
        var calls = 0;
        await TestRuntimeAsyncClass.TestMethodReturnValueTaskAsync();

        using (Mock.Setup(() => TestRuntimeAsyncClass.TestMethodReturnValueTaskAsync()).Returns(() => { calls++; return ValueTask.CompletedTask; }))
        {
            var task = TestRuntimeAsyncClass.TestMethodReturnValueTaskAsync();
            Assert.That(task.IsCompleted, Is.True, "The mock must return a completed task without running the original method");
            await task;
            Assert.That(calls, Is.EqualTo(1));
        }

        await TestRuntimeAsyncClass.TestMethodReturnValueTaskAsync();
    }

    [Test]
    public async Task TestStaticThrowsValueTask()
    {
        await TestRuntimeAsyncClass.TestMethodReturnValueTaskAsync();

        using (Mock.Setup(() => TestRuntimeAsyncClass.TestMethodReturnValueTaskAsync()).Throws<ArgumentException>())
        {
            await Assert.ThrowsAsync<ArgumentException>(async () => await TestRuntimeAsyncClass.TestMethodReturnValueTaskAsync());
        }

        await TestRuntimeAsyncClass.TestMethodReturnValueTaskAsync();
    }

    [Test]
    public async Task TestStaticInterceptedFromRuntimeAsyncCallerValueTask()
    {
        var calls = 0;
        await TestRuntimeAsyncClass.TestCallerAwaitsValueTaskAsync();

        using (Mock.Setup(() => TestRuntimeAsyncClass.TestMethodReturnValueTaskAsync()).Returns(() => { calls++; return ValueTask.CompletedTask; }))
        {
            await TestRuntimeAsyncClass.TestCallerAwaitsValueTaskAsync();
            Assert.That(calls, Is.EqualTo(1));
        }

        await TestRuntimeAsyncClass.TestCallerAwaitsValueTaskAsync();
    }

    [Test]
    public async Task TestStaticSuspendedOriginalCallValueTaskFinishesWithOriginalResultAfterMockInstalled()
    {
        var calls = 0;
        var gate = new TaskCompletionSource();
        var suspended = TestRuntimeAsyncClass.TestMethodAwaitGateReturnValueTaskAsync(gate.Task);
        Assert.That(suspended.IsCompleted, Is.False, "The original call must suspend on the incomplete gate");

        using (Mock.Setup(context => TestRuntimeAsyncClass.TestMethodAwaitGateReturnValueTaskAsync(context.It.IsAny<Task>())).Returns(() => { calls++; return ValueTask.CompletedTask; }))
        {
            var newCall = TestRuntimeAsyncClass.TestMethodAwaitGateReturnValueTaskAsync(new TaskCompletionSource().Task);
            Assert.That(newCall.IsCompleted, Is.True, "A new call must reach the mock and must not suspend");
            await newCall;
            Assert.That(calls, Is.EqualTo(1));

            gate.SetResult();
            await suspended;
        }
    }

    [Test]
    public async Task TestStaticReturnsValueTaskOfInt()
    {
        Assert.That(await TestRuntimeAsyncClass.TestMethodReturnValueTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncClass.OriginalResult));

        using (Mock.Setup(() => TestRuntimeAsyncClass.TestMethodReturnValueTaskOfIntAsync()).Returns(() => new ValueTask<int>(MockedResult)))
        {
            var task = TestRuntimeAsyncClass.TestMethodReturnValueTaskOfIntAsync();
            Assert.That(task.IsCompleted, Is.True, "The mock must return a completed task without running the original method");
            Assert.That(await task, Is.EqualTo(MockedResult));
        }

        Assert.That(await TestRuntimeAsyncClass.TestMethodReturnValueTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncClass.OriginalResult));
    }

    [Test]
    public async Task TestStaticThrowsValueTaskOfInt()
    {
        Assert.That(await TestRuntimeAsyncClass.TestMethodReturnValueTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncClass.OriginalResult));

        using (Mock.Setup(() => TestRuntimeAsyncClass.TestMethodReturnValueTaskOfIntAsync()).Throws<ArgumentException>())
        {
            await Assert.ThrowsAsync<ArgumentException>(async () => await TestRuntimeAsyncClass.TestMethodReturnValueTaskOfIntAsync());
        }

        Assert.That(await TestRuntimeAsyncClass.TestMethodReturnValueTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncClass.OriginalResult));
    }

    [Test]
    public async Task TestStaticInterceptedFromRuntimeAsyncCallerValueTaskOfInt()
    {
        Assert.That(await TestRuntimeAsyncClass.TestCallerAwaitsValueTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncClass.OriginalResult + 1));

        using (Mock.Setup(() => TestRuntimeAsyncClass.TestMethodReturnValueTaskOfIntAsync()).Returns(() => new ValueTask<int>(MockedResult)))
        {
            Assert.That(await TestRuntimeAsyncClass.TestCallerAwaitsValueTaskOfIntAsync(), Is.EqualTo(MockedResult + 1));
        }

        Assert.That(await TestRuntimeAsyncClass.TestCallerAwaitsValueTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncClass.OriginalResult + 1));
    }

    [Test]
    public async Task TestStaticSuspendedOriginalCallValueTaskOfIntFinishesWithOriginalResultAfterMockInstalled()
    {
        var gate = new TaskCompletionSource();
        var suspended = TestRuntimeAsyncClass.TestMethodAwaitGateReturnValueTaskOfIntAsync(gate.Task);
        Assert.That(suspended.IsCompleted, Is.False, "The original call must suspend on the incomplete gate");

        using (Mock.Setup(context => TestRuntimeAsyncClass.TestMethodAwaitGateReturnValueTaskOfIntAsync(context.It.IsAny<Task>())).Returns(() => new ValueTask<int>(MockedResult)))
        {
            var newCall = TestRuntimeAsyncClass.TestMethodAwaitGateReturnValueTaskOfIntAsync(new TaskCompletionSource().Task);
            Assert.That(newCall.IsCompleted, Is.True, "A new call must reach the mock and must not suspend");
            Assert.That(await newCall, Is.EqualTo(MockedResult));

            gate.SetResult();
            Assert.That(await suspended, Is.EqualTo(TestRuntimeAsyncClass.OriginalResult));
        }
    }

    [Test]
    public async Task TestStaticReturnsWithParameterMatcherTaskOfInt()
    {
        using var _ = Mock.Setup(context => TestRuntimeAsyncClass.TestMethodReturnTaskOfIntWithParameterAsync(context.It.Is<int>(value => value == 5)))
            .ReturnsAsync(MockedResult);

        Assert.That(await TestRuntimeAsyncClass.TestMethodReturnTaskOfIntWithParameterAsync(5), Is.EqualTo(MockedResult));
        await Assert.ThrowsAsync<Exception>(async () => await TestRuntimeAsyncClass.TestMethodReturnTaskOfIntWithParameterAsync(6));
    }

    [Test]
    public async Task TestStaticReturnsFuncWithParameterValueTaskOfInt()
    {
        using var _ = Mock.Setup(context => TestRuntimeAsyncClass.TestMethodReturnValueTaskOfIntWithParameterAsync(context.It.IsAny<int>()))
            .Returns<int>(value => new ValueTask<int>(value * 10));

        Assert.That(await TestRuntimeAsyncClass.TestMethodReturnValueTaskOfIntWithParameterAsync(4), Is.EqualTo(40));
    }

    [Test]
    public async Task TestStaticReturnsOverridesOriginalThatThrowsAfterSuspensionTaskOfInt()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () => await TestRuntimeAsyncClass.TestMethodThrowsAfterSuspensionReturnTaskOfIntAsync());
        Assert.That(exception.Message, Is.EqualTo(TestRuntimeAsyncClass.ExceptionMessage));

        using (Mock.Setup(() => TestRuntimeAsyncClass.TestMethodThrowsAfterSuspensionReturnTaskOfIntAsync()).ReturnsAsync(MockedResult))
        {
            Assert.That(await TestRuntimeAsyncClass.TestMethodThrowsAfterSuspensionReturnTaskOfIntAsync(), Is.EqualTo(MockedResult));
        }

        exception = await Assert.ThrowsAsync<InvalidOperationException>(async () => await TestRuntimeAsyncClass.TestMethodThrowsAfterSuspensionReturnTaskOfIntAsync());
        Assert.That(exception.Message, Is.EqualTo(TestRuntimeAsyncClass.ExceptionMessage));
    }

    [Test]
    public async Task TestStaticInterceptedFromNormalCallerTaskOfInt()
    {
        Assert.That(await TestRuntimeAsyncClass.TestNormalCallerReturnTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncClass.OriginalResult));

        using (Mock.Setup(() => TestRuntimeAsyncClass.TestMethodReturnTaskOfIntAsync()).ReturnsAsync(MockedResult))
        {
            Assert.That(await TestRuntimeAsyncClass.TestNormalCallerReturnTaskOfIntAsync(), Is.EqualTo(MockedResult));
        }

        Assert.That(await TestRuntimeAsyncClass.TestNormalCallerReturnTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncClass.OriginalResult));
    }

    [Test]
    public async Task TestStaticCallbackAsyncVoidMethod()
    {
        var completion = new TaskCompletionSource();

        using (Mock.Setup(context => TestRuntimeAsyncClass.TestVoidMethodAsync(context.It.IsAny<TaskCompletionSource>()))
                   .Callback<TaskCompletionSource>(source => source.SetResult()))
        {
            TestRuntimeAsyncClass.TestVoidMethodAsync(completion);

            Assert.That(completion.Task.IsCompleted, Is.True, "The callback must run instead of the original method, which suspends first");
        }

        var originalCompletion = new TaskCompletionSource();
        TestRuntimeAsyncClass.TestVoidMethodAsync(originalCompletion);
        await originalCompletion.Task;
    }

    [Test]
    public async Task TestStaticCallbackAsyncVoidMethodWithParameter()
    {
        var completion = new TaskCompletionSource<int>();

        using (Mock.Setup(context => TestRuntimeAsyncClass.TestVoidMethodWithParameterAsync(context.It.IsAny<int>(), context.It.IsAny<TaskCompletionSource<int>>()))
                   .Callback<int, TaskCompletionSource<int>>((parameter, source) => source.SetResult(parameter * 10)))
        {
            TestRuntimeAsyncClass.TestVoidMethodWithParameterAsync(4, completion);

            Assert.That(completion.Task.IsCompleted, Is.True, "The callback must run instead of the original method, which suspends first");
            Assert.That(await completion.Task, Is.EqualTo(40));
        }

        var originalCompletion = new TaskCompletionSource<int>();
        TestRuntimeAsyncClass.TestVoidMethodWithParameterAsync(4, originalCompletion);
        Assert.That(await originalCompletion.Task, Is.EqualTo(4));
    }

    // ---- Instance methods: TestRuntimeAsyncInstance ----

    [Test]
    public async Task TestInstanceReturnsTask()
    {
        var instance = new TestRuntimeAsyncInstance();
        var calls = 0;
        await instance.TestMethodReturnTaskAsync();

        using (Mock.Setup(() => instance.TestMethodReturnTaskAsync()).Returns(() => { calls++; return Task.CompletedTask; }))
        {
            var task = instance.TestMethodReturnTaskAsync();
            Assert.That(task.IsCompleted, Is.True, "The mock must return a completed task without running the original method");
            await task;
            Assert.That(calls, Is.EqualTo(1));
        }

        await instance.TestMethodReturnTaskAsync();
    }

    [Test]
    public async Task TestInstanceThrowsTask()
    {
        var instance = new TestRuntimeAsyncInstance();
        await instance.TestMethodReturnTaskAsync();

        using (Mock.Setup(() => instance.TestMethodReturnTaskAsync()).Throws<ArgumentException>())
        {
            await Assert.ThrowsAsync<ArgumentException>(async () => await instance.TestMethodReturnTaskAsync());
        }

        await instance.TestMethodReturnTaskAsync();
    }

    [Test]
    public async Task TestInstanceInterceptedFromRuntimeAsyncCallerTask()
    {
        var instance = new TestRuntimeAsyncInstance();
        var calls = 0;
        await instance.TestCallerAwaitsTaskAsync();

        using (Mock.Setup(() => instance.TestMethodReturnTaskAsync()).Returns(() => { calls++; return Task.CompletedTask; }))
        {
            await instance.TestCallerAwaitsTaskAsync();
            Assert.That(calls, Is.EqualTo(1));
        }

        await instance.TestCallerAwaitsTaskAsync();
    }

    [Test]
    public async Task TestInstanceSuspendedOriginalCallTaskFinishesWithOriginalResultAfterMockInstalled()
    {
        var instance = new TestRuntimeAsyncInstance();
        var calls = 0;
        var gate = new TaskCompletionSource();
        var suspended = instance.TestMethodAwaitGateReturnTaskAsync(gate.Task);
        Assert.That(suspended.IsCompleted, Is.False, "The original call must suspend on the incomplete gate");

        using (Mock.Setup(context => instance.TestMethodAwaitGateReturnTaskAsync(context.It.IsAny<Task>())).Returns(() => { calls++; return Task.CompletedTask; }))
        {
            var newCall = instance.TestMethodAwaitGateReturnTaskAsync(new TaskCompletionSource().Task);
            Assert.That(newCall.IsCompleted, Is.True, "A new call must reach the mock and must not suspend");
            await newCall;
            Assert.That(calls, Is.EqualTo(1));

            gate.SetResult();
            await suspended;
        }
    }

    [Test]
    public async Task TestInstanceReturnsTaskOfInt()
    {
        var instance = new TestRuntimeAsyncInstance();
        Assert.That(await instance.TestMethodReturnTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncInstance.OriginalResult));

        using (Mock.Setup(() => instance.TestMethodReturnTaskOfIntAsync()).ReturnsAsync(MockedResult))
        {
            var task = instance.TestMethodReturnTaskOfIntAsync();
            Assert.That(task.IsCompleted, Is.True, "The mock must return a completed task without running the original method");
            Assert.That(await task, Is.EqualTo(MockedResult));
        }

        Assert.That(await instance.TestMethodReturnTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncInstance.OriginalResult));
    }

    [Test]
    public async Task TestInstanceThrowsTaskOfInt()
    {
        var instance = new TestRuntimeAsyncInstance();
        Assert.That(await instance.TestMethodReturnTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncInstance.OriginalResult));

        using (Mock.Setup(() => instance.TestMethodReturnTaskOfIntAsync()).Throws<ArgumentException>())
        {
            await Assert.ThrowsAsync<ArgumentException>(async () => await instance.TestMethodReturnTaskOfIntAsync());
        }

        Assert.That(await instance.TestMethodReturnTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncInstance.OriginalResult));
    }

    [Test]
    public async Task TestInstanceInterceptedFromRuntimeAsyncCallerTaskOfInt()
    {
        var instance = new TestRuntimeAsyncInstance();
        Assert.That(await instance.TestCallerAwaitsTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncInstance.OriginalResult + 1));

        using (Mock.Setup(() => instance.TestMethodReturnTaskOfIntAsync()).ReturnsAsync(MockedResult))
        {
            Assert.That(await instance.TestCallerAwaitsTaskOfIntAsync(), Is.EqualTo(MockedResult + 1));
        }

        Assert.That(await instance.TestCallerAwaitsTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncInstance.OriginalResult + 1));
    }

    [Test]
    public async Task TestInstanceSuspendedOriginalCallTaskOfIntFinishesWithOriginalResultAfterMockInstalled()
    {
        var instance = new TestRuntimeAsyncInstance();
        var gate = new TaskCompletionSource();
        var suspended = instance.TestMethodAwaitGateReturnTaskOfIntAsync(gate.Task);
        Assert.That(suspended.IsCompleted, Is.False, "The original call must suspend on the incomplete gate");

        using (Mock.Setup(context => instance.TestMethodAwaitGateReturnTaskOfIntAsync(context.It.IsAny<Task>())).ReturnsAsync(MockedResult))
        {
            var newCall = instance.TestMethodAwaitGateReturnTaskOfIntAsync(new TaskCompletionSource().Task);
            Assert.That(newCall.IsCompleted, Is.True, "A new call must reach the mock and must not suspend");
            Assert.That(await newCall, Is.EqualTo(MockedResult));

            gate.SetResult();
            Assert.That(await suspended, Is.EqualTo(TestRuntimeAsyncInstance.OriginalResult));
        }
    }

    [Test]
    public async Task TestInstanceReturnsValueTask()
    {
        var instance = new TestRuntimeAsyncInstance();
        var calls = 0;
        await instance.TestMethodReturnValueTaskAsync();

        using (Mock.Setup(() => instance.TestMethodReturnValueTaskAsync()).Returns(() => { calls++; return ValueTask.CompletedTask; }))
        {
            var task = instance.TestMethodReturnValueTaskAsync();
            Assert.That(task.IsCompleted, Is.True, "The mock must return a completed task without running the original method");
            await task;
            Assert.That(calls, Is.EqualTo(1));
        }

        await instance.TestMethodReturnValueTaskAsync();
    }

    [Test]
    public async Task TestInstanceThrowsValueTask()
    {
        var instance = new TestRuntimeAsyncInstance();
        await instance.TestMethodReturnValueTaskAsync();

        using (Mock.Setup(() => instance.TestMethodReturnValueTaskAsync()).Throws<ArgumentException>())
        {
            await Assert.ThrowsAsync<ArgumentException>(async () => await instance.TestMethodReturnValueTaskAsync());
        }

        await instance.TestMethodReturnValueTaskAsync();
    }

    [Test]
    public async Task TestInstanceInterceptedFromRuntimeAsyncCallerValueTask()
    {
        var instance = new TestRuntimeAsyncInstance();
        var calls = 0;
        await instance.TestCallerAwaitsValueTaskAsync();

        using (Mock.Setup(() => instance.TestMethodReturnValueTaskAsync()).Returns(() => { calls++; return ValueTask.CompletedTask; }))
        {
            await instance.TestCallerAwaitsValueTaskAsync();
            Assert.That(calls, Is.EqualTo(1));
        }

        await instance.TestCallerAwaitsValueTaskAsync();
    }

    [Test]
    public async Task TestInstanceSuspendedOriginalCallValueTaskFinishesWithOriginalResultAfterMockInstalled()
    {
        var instance = new TestRuntimeAsyncInstance();
        var calls = 0;
        var gate = new TaskCompletionSource();
        var suspended = instance.TestMethodAwaitGateReturnValueTaskAsync(gate.Task);
        Assert.That(suspended.IsCompleted, Is.False, "The original call must suspend on the incomplete gate");

        using (Mock.Setup(context => instance.TestMethodAwaitGateReturnValueTaskAsync(context.It.IsAny<Task>())).Returns(() => { calls++; return ValueTask.CompletedTask; }))
        {
            var newCall = instance.TestMethodAwaitGateReturnValueTaskAsync(new TaskCompletionSource().Task);
            Assert.That(newCall.IsCompleted, Is.True, "A new call must reach the mock and must not suspend");
            await newCall;
            Assert.That(calls, Is.EqualTo(1));

            gate.SetResult();
            await suspended;
        }
    }

    [Test]
    public async Task TestInstanceReturnsValueTaskOfInt()
    {
        var instance = new TestRuntimeAsyncInstance();
        Assert.That(await instance.TestMethodReturnValueTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncInstance.OriginalResult));

        using (Mock.Setup(() => instance.TestMethodReturnValueTaskOfIntAsync()).Returns(() => new ValueTask<int>(MockedResult)))
        {
            var task = instance.TestMethodReturnValueTaskOfIntAsync();
            Assert.That(task.IsCompleted, Is.True, "The mock must return a completed task without running the original method");
            Assert.That(await task, Is.EqualTo(MockedResult));
        }

        Assert.That(await instance.TestMethodReturnValueTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncInstance.OriginalResult));
    }

    [Test]
    public async Task TestInstanceThrowsValueTaskOfInt()
    {
        var instance = new TestRuntimeAsyncInstance();
        Assert.That(await instance.TestMethodReturnValueTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncInstance.OriginalResult));

        using (Mock.Setup(() => instance.TestMethodReturnValueTaskOfIntAsync()).Throws<ArgumentException>())
        {
            await Assert.ThrowsAsync<ArgumentException>(async () => await instance.TestMethodReturnValueTaskOfIntAsync());
        }

        Assert.That(await instance.TestMethodReturnValueTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncInstance.OriginalResult));
    }

    [Test]
    public async Task TestInstanceInterceptedFromRuntimeAsyncCallerValueTaskOfInt()
    {
        var instance = new TestRuntimeAsyncInstance();
        Assert.That(await instance.TestCallerAwaitsValueTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncInstance.OriginalResult + 1));

        using (Mock.Setup(() => instance.TestMethodReturnValueTaskOfIntAsync()).Returns(() => new ValueTask<int>(MockedResult)))
        {
            Assert.That(await instance.TestCallerAwaitsValueTaskOfIntAsync(), Is.EqualTo(MockedResult + 1));
        }

        Assert.That(await instance.TestCallerAwaitsValueTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncInstance.OriginalResult + 1));
    }

    [Test]
    public async Task TestInstanceSuspendedOriginalCallValueTaskOfIntFinishesWithOriginalResultAfterMockInstalled()
    {
        var instance = new TestRuntimeAsyncInstance();
        var gate = new TaskCompletionSource();
        var suspended = instance.TestMethodAwaitGateReturnValueTaskOfIntAsync(gate.Task);
        Assert.That(suspended.IsCompleted, Is.False, "The original call must suspend on the incomplete gate");

        using (Mock.Setup(context => instance.TestMethodAwaitGateReturnValueTaskOfIntAsync(context.It.IsAny<Task>())).Returns(() => new ValueTask<int>(MockedResult)))
        {
            var newCall = instance.TestMethodAwaitGateReturnValueTaskOfIntAsync(new TaskCompletionSource().Task);
            Assert.That(newCall.IsCompleted, Is.True, "A new call must reach the mock and must not suspend");
            Assert.That(await newCall, Is.EqualTo(MockedResult));

            gate.SetResult();
            Assert.That(await suspended, Is.EqualTo(TestRuntimeAsyncInstance.OriginalResult));
        }
    }

    [Test]
    public async Task TestInstanceReturnsWithParameterMatcherTaskOfInt()
    {
        var instance = new TestRuntimeAsyncInstance();
        using var _ = Mock.Setup(context => instance.TestMethodReturnTaskOfIntWithParameterAsync(context.It.Is<int>(value => value == 5)))
            .ReturnsAsync(MockedResult);

        Assert.That(await instance.TestMethodReturnTaskOfIntWithParameterAsync(5), Is.EqualTo(MockedResult));
        await Assert.ThrowsAsync<Exception>(async () => await instance.TestMethodReturnTaskOfIntWithParameterAsync(6));
    }

    [Test]
    public async Task TestInstanceReturnsFuncWithParameterValueTaskOfInt()
    {
        var instance = new TestRuntimeAsyncInstance();
        using var _ = Mock.Setup(context => instance.TestMethodReturnValueTaskOfIntWithParameterAsync(context.It.IsAny<int>()))
            .Returns<int>(value => new ValueTask<int>(value * 10));

        Assert.That(await instance.TestMethodReturnValueTaskOfIntWithParameterAsync(4), Is.EqualTo(40));
    }

    [Test]
    public async Task TestInstanceReturnsOverridesOriginalThatThrowsAfterSuspensionTaskOfInt()
    {
        var instance = new TestRuntimeAsyncInstance();
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () => await instance.TestMethodThrowsAfterSuspensionReturnTaskOfIntAsync());
        Assert.That(exception.Message, Is.EqualTo(TestRuntimeAsyncInstance.ExceptionMessage));

        using (Mock.Setup(() => instance.TestMethodThrowsAfterSuspensionReturnTaskOfIntAsync()).ReturnsAsync(MockedResult))
        {
            Assert.That(await instance.TestMethodThrowsAfterSuspensionReturnTaskOfIntAsync(), Is.EqualTo(MockedResult));
        }

        exception = await Assert.ThrowsAsync<InvalidOperationException>(async () => await instance.TestMethodThrowsAfterSuspensionReturnTaskOfIntAsync());
        Assert.That(exception.Message, Is.EqualTo(TestRuntimeAsyncInstance.ExceptionMessage));
    }

    [Test]
    public async Task TestInstanceInterceptedFromNormalCallerTaskOfInt()
    {
        var instance = new TestRuntimeAsyncInstance();
        Assert.That(await instance.TestNormalCallerReturnTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncInstance.OriginalResult));

        using (Mock.Setup(() => instance.TestMethodReturnTaskOfIntAsync()).ReturnsAsync(MockedResult))
        {
            Assert.That(await instance.TestNormalCallerReturnTaskOfIntAsync(), Is.EqualTo(MockedResult));
        }

        Assert.That(await instance.TestNormalCallerReturnTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncInstance.OriginalResult));
    }

    [Test]
    public async Task TestInstanceCallbackAsyncVoidMethod()
    {
        var instance = new TestRuntimeAsyncInstance();
        var completion = new TaskCompletionSource();

        using (Mock.Setup(context => instance.TestVoidMethodAsync(context.It.IsAny<TaskCompletionSource>()))
                   .Callback<TaskCompletionSource>(source => source.SetResult()))
        {
            instance.TestVoidMethodAsync(completion);

            Assert.That(completion.Task.IsCompleted, Is.True, "The callback must run instead of the original method, which suspends first");
        }

        var originalCompletion = new TaskCompletionSource();
        instance.TestVoidMethodAsync(originalCompletion);
        await originalCompletion.Task;
    }

    [Test]
    public async Task TestInstanceCallbackAsyncVoidMethodWithParameter()
    {
        var instance = new TestRuntimeAsyncInstance();
        var completion = new TaskCompletionSource<int>();

        using (Mock.Setup(context => instance.TestVoidMethodWithParameterAsync(context.It.IsAny<int>(), context.It.IsAny<TaskCompletionSource<int>>()))
                   .Callback<int, TaskCompletionSource<int>>((parameter, source) => source.SetResult(parameter * 10)))
        {
            instance.TestVoidMethodWithParameterAsync(4, completion);

            Assert.That(completion.Task.IsCompleted, Is.True, "The callback must run instead of the original method, which suspends first");
            Assert.That(await completion.Task, Is.EqualTo(40));
        }

        var originalCompletion = new TaskCompletionSource<int>();
        instance.TestVoidMethodWithParameterAsync(4, originalCompletion);
        Assert.That(await originalCompletion.Task, Is.EqualTo(4));
    }
}
#endif
