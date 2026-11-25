#if NET11_0_OR_GREATER
using NUnit.Framework;
using StaticMock.Tests.Common.TestEntities;

namespace StaticMock.Tests.Tests.Hierarchical.RuntimeAsyncTests;

/// <summary>
/// Hierarchical API: the validation action runs while the hook is active, then the hook is removed.
/// The tests mock runtime-async methods (compiler feature <c>runtime-async=on</c>) of <see cref="TestRuntimeAsyncClass"/>
/// and <see cref="TestRuntimeAsyncInstance"/> for <c>Task</c>, <c>Task&lt;T&gt;</c>, <c>ValueTask</c> and <c>ValueTask&lt;T&gt;</c>:
/// mocked return, <c>Throws</c>, original behavior after the mock ends, calls from a normal and from a runtime-async caller,
/// a suspended original call, and <c>Callback</c> on <c>async void</c> methods (not runtime-async methods).
/// The validation is always synchronous. It never uses an <c>async</c> lambda, because the <c>Action</c> would become <c>async void</c> and the hook could end before the assertion finishes.
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

        Mock.Setup(() => TestRuntimeAsyncClass.TestMethodReturnTaskAsync(), () =>
        {
            var task = TestRuntimeAsyncClass.TestMethodReturnTaskAsync();
            Assert.That(task.IsCompleted, Is.True, "The mock must return a completed task without running the original method");
            task.GetAwaiter().GetResult();
        }).Returns(() => { calls++; return Task.CompletedTask; });

        Assert.That(calls, Is.EqualTo(1));

        await TestRuntimeAsyncClass.TestMethodReturnTaskAsync();
    }

    [Test]
    public async Task TestStaticThrowsTask()
    {
        await TestRuntimeAsyncClass.TestMethodReturnTaskAsync();

        Mock.Setup(() => TestRuntimeAsyncClass.TestMethodReturnTaskAsync(), () =>
        {
            Assert.ThrowsAsync<ArgumentException>(async () => await TestRuntimeAsyncClass.TestMethodReturnTaskAsync()).GetAwaiter().GetResult();
        }).Throws<ArgumentException>();

        await TestRuntimeAsyncClass.TestMethodReturnTaskAsync();
    }

    [Test]
    public async Task TestStaticInterceptedFromRuntimeAsyncCallerTask()
    {
        var calls = 0;
        await TestRuntimeAsyncClass.TestCallerAwaitsTaskAsync();

        Mock.Setup(() => TestRuntimeAsyncClass.TestMethodReturnTaskAsync(), () =>
        {
            TestRuntimeAsyncClass.TestCallerAwaitsTaskAsync().GetAwaiter().GetResult();
        }).Returns(() => { calls++; return Task.CompletedTask; });

        Assert.That(calls, Is.EqualTo(1));

        await TestRuntimeAsyncClass.TestCallerAwaitsTaskAsync();
    }

    [Test]
    public void TestStaticSuspendedOriginalCallTaskFinishesWithOriginalResultAfterMockInstalled()
    {
        var calls = 0;
        var gate = new TaskCompletionSource();
        var suspended = TestRuntimeAsyncClass.TestMethodAwaitGateReturnTaskAsync(gate.Task);
        Assert.That(suspended.IsCompleted, Is.False, "The original call must suspend on the incomplete gate");

        Mock.Setup(context => TestRuntimeAsyncClass.TestMethodAwaitGateReturnTaskAsync(context.It.IsAny<Task>()), () =>
        {
            var newCall = TestRuntimeAsyncClass.TestMethodAwaitGateReturnTaskAsync(new TaskCompletionSource().Task);
            Assert.That(newCall.IsCompleted, Is.True, "A new call must reach the mock and must not suspend");
            newCall.GetAwaiter().GetResult();

            gate.SetResult();
            suspended.GetAwaiter().GetResult();
        }).Returns(() => { calls++; return Task.CompletedTask; });
        Assert.That(calls, Is.EqualTo(1));
    }

    [Test]
    public async Task TestStaticReturnsTaskOfInt()
    {
        Assert.That(await TestRuntimeAsyncClass.TestMethodReturnTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncClass.OriginalResult));

        Mock.Setup(() => TestRuntimeAsyncClass.TestMethodReturnTaskOfIntAsync(), () =>
        {
            var task = TestRuntimeAsyncClass.TestMethodReturnTaskOfIntAsync();
            Assert.That(task.IsCompleted, Is.True, "The mock must return a completed task without running the original method");
            Assert.That(task.GetAwaiter().GetResult(), Is.EqualTo(MockedResult));
        }).ReturnsAsync(MockedResult);

        Assert.That(await TestRuntimeAsyncClass.TestMethodReturnTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncClass.OriginalResult));
    }

    [Test]
    public async Task TestStaticThrowsTaskOfInt()
    {
        Assert.That(await TestRuntimeAsyncClass.TestMethodReturnTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncClass.OriginalResult));

        Mock.Setup(() => TestRuntimeAsyncClass.TestMethodReturnTaskOfIntAsync(), () =>
        {
            Assert.ThrowsAsync<ArgumentException>(async () => await TestRuntimeAsyncClass.TestMethodReturnTaskOfIntAsync()).GetAwaiter().GetResult();
        }).Throws<ArgumentException>();

        Assert.That(await TestRuntimeAsyncClass.TestMethodReturnTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncClass.OriginalResult));
    }

    [Test]
    public async Task TestStaticInterceptedFromRuntimeAsyncCallerTaskOfInt()
    {
        Assert.That(await TestRuntimeAsyncClass.TestCallerAwaitsTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncClass.OriginalResult + 1));

        Mock.Setup(() => TestRuntimeAsyncClass.TestMethodReturnTaskOfIntAsync(), () =>
        {
            Assert.That(TestRuntimeAsyncClass.TestCallerAwaitsTaskOfIntAsync().GetAwaiter().GetResult(), Is.EqualTo(MockedResult + 1));
        }).ReturnsAsync(MockedResult);

        Assert.That(await TestRuntimeAsyncClass.TestCallerAwaitsTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncClass.OriginalResult + 1));
    }

    [Test]
    public void TestStaticSuspendedOriginalCallTaskOfIntFinishesWithOriginalResultAfterMockInstalled()
    {
        var gate = new TaskCompletionSource();
        var suspended = TestRuntimeAsyncClass.TestMethodAwaitGateReturnTaskOfIntAsync(gate.Task);
        Assert.That(suspended.IsCompleted, Is.False, "The original call must suspend on the incomplete gate");

        Mock.Setup(context => TestRuntimeAsyncClass.TestMethodAwaitGateReturnTaskOfIntAsync(context.It.IsAny<Task>()), () =>
        {
            var newCall = TestRuntimeAsyncClass.TestMethodAwaitGateReturnTaskOfIntAsync(new TaskCompletionSource().Task);
            Assert.That(newCall.IsCompleted, Is.True, "A new call must reach the mock and must not suspend");
            Assert.That(newCall.GetAwaiter().GetResult(), Is.EqualTo(MockedResult));

            gate.SetResult();
            Assert.That(suspended.GetAwaiter().GetResult(), Is.EqualTo(TestRuntimeAsyncClass.OriginalResult));
        }).ReturnsAsync(MockedResult);
    }

    [Test]
    public async Task TestStaticReturnsValueTask()
    {
        var calls = 0;
        await TestRuntimeAsyncClass.TestMethodReturnValueTaskAsync();

        Mock.Setup(() => TestRuntimeAsyncClass.TestMethodReturnValueTaskAsync(), () =>
        {
            var task = TestRuntimeAsyncClass.TestMethodReturnValueTaskAsync();
            Assert.That(task.IsCompleted, Is.True, "The mock must return a completed task without running the original method");
            task.GetAwaiter().GetResult();
        }).Returns(() => { calls++; return ValueTask.CompletedTask; });

        Assert.That(calls, Is.EqualTo(1));

        await TestRuntimeAsyncClass.TestMethodReturnValueTaskAsync();
    }

    [Test]
    public async Task TestStaticThrowsValueTask()
    {
        await TestRuntimeAsyncClass.TestMethodReturnValueTaskAsync();

        Mock.Setup(() => TestRuntimeAsyncClass.TestMethodReturnValueTaskAsync(), () =>
        {
            Assert.ThrowsAsync<ArgumentException>(async () => await TestRuntimeAsyncClass.TestMethodReturnValueTaskAsync()).GetAwaiter().GetResult();
        }).Throws<ArgumentException>();

        await TestRuntimeAsyncClass.TestMethodReturnValueTaskAsync();
    }

    [Test]
    public async Task TestStaticInterceptedFromRuntimeAsyncCallerValueTask()
    {
        var calls = 0;
        await TestRuntimeAsyncClass.TestCallerAwaitsValueTaskAsync();

        Mock.Setup(() => TestRuntimeAsyncClass.TestMethodReturnValueTaskAsync(), () =>
        {
            TestRuntimeAsyncClass.TestCallerAwaitsValueTaskAsync().GetAwaiter().GetResult();
        }).Returns(() => { calls++; return ValueTask.CompletedTask; });

        Assert.That(calls, Is.EqualTo(1));

        await TestRuntimeAsyncClass.TestCallerAwaitsValueTaskAsync();
    }

    [Test]
    public void TestStaticSuspendedOriginalCallValueTaskFinishesWithOriginalResultAfterMockInstalled()
    {
        var calls = 0;
        var gate = new TaskCompletionSource();
        var suspended = TestRuntimeAsyncClass.TestMethodAwaitGateReturnValueTaskAsync(gate.Task);
        Assert.That(suspended.IsCompleted, Is.False, "The original call must suspend on the incomplete gate");

        Mock.Setup(context => TestRuntimeAsyncClass.TestMethodAwaitGateReturnValueTaskAsync(context.It.IsAny<Task>()), () =>
        {
            var newCall = TestRuntimeAsyncClass.TestMethodAwaitGateReturnValueTaskAsync(new TaskCompletionSource().Task);
            Assert.That(newCall.IsCompleted, Is.True, "A new call must reach the mock and must not suspend");
            newCall.GetAwaiter().GetResult();

            gate.SetResult();
            suspended.GetAwaiter().GetResult();
        }).Returns(() => { calls++; return ValueTask.CompletedTask; });
        Assert.That(calls, Is.EqualTo(1));
    }

    [Test]
    public async Task TestStaticReturnsValueTaskOfInt()
    {
        Assert.That(await TestRuntimeAsyncClass.TestMethodReturnValueTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncClass.OriginalResult));

        Mock.Setup(() => TestRuntimeAsyncClass.TestMethodReturnValueTaskOfIntAsync(), () =>
        {
            var task = TestRuntimeAsyncClass.TestMethodReturnValueTaskOfIntAsync();
            Assert.That(task.IsCompleted, Is.True, "The mock must return a completed task without running the original method");
            Assert.That(task.GetAwaiter().GetResult(), Is.EqualTo(MockedResult));
        }).Returns(() => new ValueTask<int>(MockedResult));

        Assert.That(await TestRuntimeAsyncClass.TestMethodReturnValueTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncClass.OriginalResult));
    }

    [Test]
    public async Task TestStaticThrowsValueTaskOfInt()
    {
        Assert.That(await TestRuntimeAsyncClass.TestMethodReturnValueTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncClass.OriginalResult));

        Mock.Setup(() => TestRuntimeAsyncClass.TestMethodReturnValueTaskOfIntAsync(), () =>
        {
            Assert.ThrowsAsync<ArgumentException>(async () => await TestRuntimeAsyncClass.TestMethodReturnValueTaskOfIntAsync()).GetAwaiter().GetResult();
        }).Throws<ArgumentException>();

        Assert.That(await TestRuntimeAsyncClass.TestMethodReturnValueTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncClass.OriginalResult));
    }

    [Test]
    public async Task TestStaticInterceptedFromRuntimeAsyncCallerValueTaskOfInt()
    {
        Assert.That(await TestRuntimeAsyncClass.TestCallerAwaitsValueTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncClass.OriginalResult + 1));

        Mock.Setup(() => TestRuntimeAsyncClass.TestMethodReturnValueTaskOfIntAsync(), () =>
        {
            Assert.That(TestRuntimeAsyncClass.TestCallerAwaitsValueTaskOfIntAsync().GetAwaiter().GetResult(), Is.EqualTo(MockedResult + 1));
        }).Returns(() => new ValueTask<int>(MockedResult));

        Assert.That(await TestRuntimeAsyncClass.TestCallerAwaitsValueTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncClass.OriginalResult + 1));
    }

    [Test]
    public void TestStaticSuspendedOriginalCallValueTaskOfIntFinishesWithOriginalResultAfterMockInstalled()
    {
        var gate = new TaskCompletionSource();
        var suspended = TestRuntimeAsyncClass.TestMethodAwaitGateReturnValueTaskOfIntAsync(gate.Task);
        Assert.That(suspended.IsCompleted, Is.False, "The original call must suspend on the incomplete gate");

        Mock.Setup(context => TestRuntimeAsyncClass.TestMethodAwaitGateReturnValueTaskOfIntAsync(context.It.IsAny<Task>()), () =>
        {
            var newCall = TestRuntimeAsyncClass.TestMethodAwaitGateReturnValueTaskOfIntAsync(new TaskCompletionSource().Task);
            Assert.That(newCall.IsCompleted, Is.True, "A new call must reach the mock and must not suspend");
            Assert.That(newCall.GetAwaiter().GetResult(), Is.EqualTo(MockedResult));

            gate.SetResult();
            Assert.That(suspended.GetAwaiter().GetResult(), Is.EqualTo(TestRuntimeAsyncClass.OriginalResult));
        }).Returns(() => new ValueTask<int>(MockedResult));
    }

    [Test]
    public async Task TestStaticReturnsWithParameterMatcherTaskOfInt()
    {
        Mock.Setup(context => TestRuntimeAsyncClass.TestMethodReturnTaskOfIntWithParameterAsync(context.It.Is<int>(value => value == 5)), () =>
        {
            Assert.That(TestRuntimeAsyncClass.TestMethodReturnTaskOfIntWithParameterAsync(5).GetAwaiter().GetResult(), Is.EqualTo(MockedResult));
            Assert.ThrowsAsync<Exception>(async () => await TestRuntimeAsyncClass.TestMethodReturnTaskOfIntWithParameterAsync(6)).GetAwaiter().GetResult();
        }).ReturnsAsync(MockedResult);

        Assert.That(await TestRuntimeAsyncClass.TestMethodReturnTaskOfIntWithParameterAsync(5), Is.EqualTo(5));
    }

    [Test]
    public async Task TestStaticReturnsFuncWithParameterValueTaskOfInt()
    {
        Mock.Setup(context => TestRuntimeAsyncClass.TestMethodReturnValueTaskOfIntWithParameterAsync(context.It.IsAny<int>()), () =>
        {
            Assert.That(TestRuntimeAsyncClass.TestMethodReturnValueTaskOfIntWithParameterAsync(4).GetAwaiter().GetResult(), Is.EqualTo(40));
        }).Returns<int>(value => new ValueTask<int>(value * 10));

        Assert.That(await TestRuntimeAsyncClass.TestMethodReturnValueTaskOfIntWithParameterAsync(4), Is.EqualTo(4));
    }

    [Test]
    public async Task TestStaticReturnsOverridesOriginalThatThrowsAfterSuspensionTaskOfInt()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () => await TestRuntimeAsyncClass.TestMethodThrowsAfterSuspensionReturnTaskOfIntAsync());
        Assert.That(exception.Message, Is.EqualTo(TestRuntimeAsyncClass.ExceptionMessage));

        Mock.Setup(() => TestRuntimeAsyncClass.TestMethodThrowsAfterSuspensionReturnTaskOfIntAsync(), () =>
        {
            Assert.That(TestRuntimeAsyncClass.TestMethodThrowsAfterSuspensionReturnTaskOfIntAsync().GetAwaiter().GetResult(), Is.EqualTo(MockedResult));
        }).ReturnsAsync(MockedResult);

        exception = await Assert.ThrowsAsync<InvalidOperationException>(async () => await TestRuntimeAsyncClass.TestMethodThrowsAfterSuspensionReturnTaskOfIntAsync());
        Assert.That(exception.Message, Is.EqualTo(TestRuntimeAsyncClass.ExceptionMessage));
    }

    [Test]
    public async Task TestStaticInterceptedFromNormalCallerTaskOfInt()
    {
        Assert.That(await TestRuntimeAsyncClass.TestNormalCallerReturnTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncClass.OriginalResult));

        Mock.Setup(() => TestRuntimeAsyncClass.TestMethodReturnTaskOfIntAsync(), () =>
        {
            Assert.That(TestRuntimeAsyncClass.TestNormalCallerReturnTaskOfIntAsync().GetAwaiter().GetResult(), Is.EqualTo(MockedResult));
        }).ReturnsAsync(MockedResult);

        Assert.That(await TestRuntimeAsyncClass.TestNormalCallerReturnTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncClass.OriginalResult));
    }

    [Test]
    public async Task TestStaticCallbackAsyncVoidMethod()
    {
        Mock.Setup(context => TestRuntimeAsyncClass.TestVoidMethodAsync(context.It.IsAny<TaskCompletionSource>()), () =>
        {
            var completion = new TaskCompletionSource();
            TestRuntimeAsyncClass.TestVoidMethodAsync(completion);

            Assert.That(completion.Task.IsCompleted, Is.True, "The callback must run instead of the original method, which suspends first");
        }).Callback<TaskCompletionSource>(source => source.SetResult());

        var originalCompletion = new TaskCompletionSource();
        TestRuntimeAsyncClass.TestVoidMethodAsync(originalCompletion);
        await originalCompletion.Task;
    }

    [Test]
    public async Task TestStaticCallbackAsyncVoidMethodWithParameter()
    {
        Mock.Setup(context => TestRuntimeAsyncClass.TestVoidMethodWithParameterAsync(context.It.IsAny<int>(), context.It.IsAny<TaskCompletionSource<int>>()), () =>
        {
            var completion = new TaskCompletionSource<int>();
            TestRuntimeAsyncClass.TestVoidMethodWithParameterAsync(4, completion);

            Assert.That(completion.Task.IsCompleted, Is.True, "The callback must run instead of the original method, which suspends first");
            Assert.That(completion.Task.GetAwaiter().GetResult(), Is.EqualTo(40));
        }).Callback<int, TaskCompletionSource<int>>((parameter, source) => source.SetResult(parameter * 10));

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

        Mock.Setup(() => instance.TestMethodReturnTaskAsync(), () =>
        {
            var task = instance.TestMethodReturnTaskAsync();
            Assert.That(task.IsCompleted, Is.True, "The mock must return a completed task without running the original method");
            task.GetAwaiter().GetResult();
        }).Returns(() => { calls++; return Task.CompletedTask; });

        Assert.That(calls, Is.EqualTo(1));

        await instance.TestMethodReturnTaskAsync();
    }

    [Test]
    public async Task TestInstanceThrowsTask()
    {
        var instance = new TestRuntimeAsyncInstance();
        await instance.TestMethodReturnTaskAsync();

        Mock.Setup(() => instance.TestMethodReturnTaskAsync(), () =>
        {
            Assert.ThrowsAsync<ArgumentException>(async () => await instance.TestMethodReturnTaskAsync()).GetAwaiter().GetResult();
        }).Throws<ArgumentException>();

        await instance.TestMethodReturnTaskAsync();
    }

    [Test]
    public async Task TestInstanceInterceptedFromRuntimeAsyncCallerTask()
    {
        var instance = new TestRuntimeAsyncInstance();
        var calls = 0;
        await instance.TestCallerAwaitsTaskAsync();

        Mock.Setup(() => instance.TestMethodReturnTaskAsync(), () =>
        {
            instance.TestCallerAwaitsTaskAsync().GetAwaiter().GetResult();
        }).Returns(() => { calls++; return Task.CompletedTask; });

        Assert.That(calls, Is.EqualTo(1));

        await instance.TestCallerAwaitsTaskAsync();
    }

    [Test]
    public void TestInstanceSuspendedOriginalCallTaskFinishesWithOriginalResultAfterMockInstalled()
    {
        var instance = new TestRuntimeAsyncInstance();
        var calls = 0;
        var gate = new TaskCompletionSource();
        var suspended = instance.TestMethodAwaitGateReturnTaskAsync(gate.Task);
        Assert.That(suspended.IsCompleted, Is.False, "The original call must suspend on the incomplete gate");

        Mock.Setup(context => instance.TestMethodAwaitGateReturnTaskAsync(context.It.IsAny<Task>()), () =>
        {
            var newCall = instance.TestMethodAwaitGateReturnTaskAsync(new TaskCompletionSource().Task);
            Assert.That(newCall.IsCompleted, Is.True, "A new call must reach the mock and must not suspend");
            newCall.GetAwaiter().GetResult();

            gate.SetResult();
            suspended.GetAwaiter().GetResult();
        }).Returns(() => { calls++; return Task.CompletedTask; });
        Assert.That(calls, Is.EqualTo(1));
    }

    [Test]
    public async Task TestInstanceReturnsTaskOfInt()
    {
        var instance = new TestRuntimeAsyncInstance();
        Assert.That(await instance.TestMethodReturnTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncInstance.OriginalResult));

        Mock.Setup(() => instance.TestMethodReturnTaskOfIntAsync(), () =>
        {
            var task = instance.TestMethodReturnTaskOfIntAsync();
            Assert.That(task.IsCompleted, Is.True, "The mock must return a completed task without running the original method");
            Assert.That(task.GetAwaiter().GetResult(), Is.EqualTo(MockedResult));
        }).ReturnsAsync(MockedResult);

        Assert.That(await instance.TestMethodReturnTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncInstance.OriginalResult));
    }

    [Test]
    public async Task TestInstanceThrowsTaskOfInt()
    {
        var instance = new TestRuntimeAsyncInstance();
        Assert.That(await instance.TestMethodReturnTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncInstance.OriginalResult));

        Mock.Setup(() => instance.TestMethodReturnTaskOfIntAsync(), () =>
        {
            Assert.ThrowsAsync<ArgumentException>(async () => await instance.TestMethodReturnTaskOfIntAsync()).GetAwaiter().GetResult();
        }).Throws<ArgumentException>();

        Assert.That(await instance.TestMethodReturnTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncInstance.OriginalResult));
    }

    [Test]
    public async Task TestInstanceInterceptedFromRuntimeAsyncCallerTaskOfInt()
    {
        var instance = new TestRuntimeAsyncInstance();
        Assert.That(await instance.TestCallerAwaitsTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncInstance.OriginalResult + 1));

        Mock.Setup(() => instance.TestMethodReturnTaskOfIntAsync(), () =>
        {
            Assert.That(instance.TestCallerAwaitsTaskOfIntAsync().GetAwaiter().GetResult(), Is.EqualTo(MockedResult + 1));
        }).ReturnsAsync(MockedResult);

        Assert.That(await instance.TestCallerAwaitsTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncInstance.OriginalResult + 1));
    }

    [Test]
    public void TestInstanceSuspendedOriginalCallTaskOfIntFinishesWithOriginalResultAfterMockInstalled()
    {
        var instance = new TestRuntimeAsyncInstance();
        var gate = new TaskCompletionSource();
        var suspended = instance.TestMethodAwaitGateReturnTaskOfIntAsync(gate.Task);
        Assert.That(suspended.IsCompleted, Is.False, "The original call must suspend on the incomplete gate");

        Mock.Setup(context => instance.TestMethodAwaitGateReturnTaskOfIntAsync(context.It.IsAny<Task>()), () =>
        {
            var newCall = instance.TestMethodAwaitGateReturnTaskOfIntAsync(new TaskCompletionSource().Task);
            Assert.That(newCall.IsCompleted, Is.True, "A new call must reach the mock and must not suspend");
            Assert.That(newCall.GetAwaiter().GetResult(), Is.EqualTo(MockedResult));

            gate.SetResult();
            Assert.That(suspended.GetAwaiter().GetResult(), Is.EqualTo(TestRuntimeAsyncInstance.OriginalResult));
        }).ReturnsAsync(MockedResult);
    }

    [Test]
    public async Task TestInstanceReturnsValueTask()
    {
        var instance = new TestRuntimeAsyncInstance();
        var calls = 0;
        await instance.TestMethodReturnValueTaskAsync();

        Mock.Setup(() => instance.TestMethodReturnValueTaskAsync(), () =>
        {
            var task = instance.TestMethodReturnValueTaskAsync();
            Assert.That(task.IsCompleted, Is.True, "The mock must return a completed task without running the original method");
            task.GetAwaiter().GetResult();
        }).Returns(() => { calls++; return ValueTask.CompletedTask; });

        Assert.That(calls, Is.EqualTo(1));

        await instance.TestMethodReturnValueTaskAsync();
    }

    [Test]
    public async Task TestInstanceThrowsValueTask()
    {
        var instance = new TestRuntimeAsyncInstance();
        await instance.TestMethodReturnValueTaskAsync();

        Mock.Setup(() => instance.TestMethodReturnValueTaskAsync(), () =>
        {
            Assert.ThrowsAsync<ArgumentException>(async () => await instance.TestMethodReturnValueTaskAsync()).GetAwaiter().GetResult();
        }).Throws<ArgumentException>();

        await instance.TestMethodReturnValueTaskAsync();
    }

    [Test]
    public async Task TestInstanceInterceptedFromRuntimeAsyncCallerValueTask()
    {
        var instance = new TestRuntimeAsyncInstance();
        var calls = 0;
        await instance.TestCallerAwaitsValueTaskAsync();

        Mock.Setup(() => instance.TestMethodReturnValueTaskAsync(), () =>
        {
            instance.TestCallerAwaitsValueTaskAsync().GetAwaiter().GetResult();
        }).Returns(() => { calls++; return ValueTask.CompletedTask; });

        Assert.That(calls, Is.EqualTo(1));

        await instance.TestCallerAwaitsValueTaskAsync();
    }

    [Test]
    public void TestInstanceSuspendedOriginalCallValueTaskFinishesWithOriginalResultAfterMockInstalled()
    {
        var instance = new TestRuntimeAsyncInstance();
        var calls = 0;
        var gate = new TaskCompletionSource();
        var suspended = instance.TestMethodAwaitGateReturnValueTaskAsync(gate.Task);
        Assert.That(suspended.IsCompleted, Is.False, "The original call must suspend on the incomplete gate");

        Mock.Setup(context => instance.TestMethodAwaitGateReturnValueTaskAsync(context.It.IsAny<Task>()), () =>
        {
            var newCall = instance.TestMethodAwaitGateReturnValueTaskAsync(new TaskCompletionSource().Task);
            Assert.That(newCall.IsCompleted, Is.True, "A new call must reach the mock and must not suspend");
            newCall.GetAwaiter().GetResult();

            gate.SetResult();
            suspended.GetAwaiter().GetResult();
        }).Returns(() => { calls++; return ValueTask.CompletedTask; });
        Assert.That(calls, Is.EqualTo(1));
    }

    [Test]
    public async Task TestInstanceReturnsValueTaskOfInt()
    {
        var instance = new TestRuntimeAsyncInstance();
        Assert.That(await instance.TestMethodReturnValueTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncInstance.OriginalResult));

        Mock.Setup(() => instance.TestMethodReturnValueTaskOfIntAsync(), () =>
        {
            var task = instance.TestMethodReturnValueTaskOfIntAsync();
            Assert.That(task.IsCompleted, Is.True, "The mock must return a completed task without running the original method");
            Assert.That(task.GetAwaiter().GetResult(), Is.EqualTo(MockedResult));
        }).Returns(() => new ValueTask<int>(MockedResult));

        Assert.That(await instance.TestMethodReturnValueTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncInstance.OriginalResult));
    }

    [Test]
    public async Task TestInstanceThrowsValueTaskOfInt()
    {
        var instance = new TestRuntimeAsyncInstance();
        Assert.That(await instance.TestMethodReturnValueTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncInstance.OriginalResult));

        Mock.Setup(() => instance.TestMethodReturnValueTaskOfIntAsync(), () =>
        {
            Assert.ThrowsAsync<ArgumentException>(async () => await instance.TestMethodReturnValueTaskOfIntAsync()).GetAwaiter().GetResult();
        }).Throws<ArgumentException>();

        Assert.That(await instance.TestMethodReturnValueTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncInstance.OriginalResult));
    }

    [Test]
    public async Task TestInstanceInterceptedFromRuntimeAsyncCallerValueTaskOfInt()
    {
        var instance = new TestRuntimeAsyncInstance();
        Assert.That(await instance.TestCallerAwaitsValueTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncInstance.OriginalResult + 1));

        Mock.Setup(() => instance.TestMethodReturnValueTaskOfIntAsync(), () =>
        {
            Assert.That(instance.TestCallerAwaitsValueTaskOfIntAsync().GetAwaiter().GetResult(), Is.EqualTo(MockedResult + 1));
        }).Returns(() => new ValueTask<int>(MockedResult));

        Assert.That(await instance.TestCallerAwaitsValueTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncInstance.OriginalResult + 1));
    }

    [Test]
    public void TestInstanceSuspendedOriginalCallValueTaskOfIntFinishesWithOriginalResultAfterMockInstalled()
    {
        var instance = new TestRuntimeAsyncInstance();
        var gate = new TaskCompletionSource();
        var suspended = instance.TestMethodAwaitGateReturnValueTaskOfIntAsync(gate.Task);
        Assert.That(suspended.IsCompleted, Is.False, "The original call must suspend on the incomplete gate");

        Mock.Setup(context => instance.TestMethodAwaitGateReturnValueTaskOfIntAsync(context.It.IsAny<Task>()), () =>
        {
            var newCall = instance.TestMethodAwaitGateReturnValueTaskOfIntAsync(new TaskCompletionSource().Task);
            Assert.That(newCall.IsCompleted, Is.True, "A new call must reach the mock and must not suspend");
            Assert.That(newCall.GetAwaiter().GetResult(), Is.EqualTo(MockedResult));

            gate.SetResult();
            Assert.That(suspended.GetAwaiter().GetResult(), Is.EqualTo(TestRuntimeAsyncInstance.OriginalResult));
        }).Returns(() => new ValueTask<int>(MockedResult));
    }

    [Test]
    public async Task TestInstanceReturnsWithParameterMatcherTaskOfInt()
    {
        var instance = new TestRuntimeAsyncInstance();
        Mock.Setup(context => instance.TestMethodReturnTaskOfIntWithParameterAsync(context.It.Is<int>(value => value == 5)), () =>
        {
            Assert.That(instance.TestMethodReturnTaskOfIntWithParameterAsync(5).GetAwaiter().GetResult(), Is.EqualTo(MockedResult));
            Assert.ThrowsAsync<Exception>(async () => await instance.TestMethodReturnTaskOfIntWithParameterAsync(6)).GetAwaiter().GetResult();
        }).ReturnsAsync(MockedResult);

        Assert.That(await instance.TestMethodReturnTaskOfIntWithParameterAsync(5), Is.EqualTo(5));
    }

    [Test]
    public async Task TestInstanceReturnsFuncWithParameterValueTaskOfInt()
    {
        var instance = new TestRuntimeAsyncInstance();
        Mock.Setup(context => instance.TestMethodReturnValueTaskOfIntWithParameterAsync(context.It.IsAny<int>()), () =>
        {
            Assert.That(instance.TestMethodReturnValueTaskOfIntWithParameterAsync(4).GetAwaiter().GetResult(), Is.EqualTo(40));
        }).Returns<int>(value => new ValueTask<int>(value * 10));

        Assert.That(await instance.TestMethodReturnValueTaskOfIntWithParameterAsync(4), Is.EqualTo(4));
    }

    [Test]
    public async Task TestInstanceReturnsOverridesOriginalThatThrowsAfterSuspensionTaskOfInt()
    {
        var instance = new TestRuntimeAsyncInstance();
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () => await instance.TestMethodThrowsAfterSuspensionReturnTaskOfIntAsync());
        Assert.That(exception.Message, Is.EqualTo(TestRuntimeAsyncInstance.ExceptionMessage));

        Mock.Setup(() => instance.TestMethodThrowsAfterSuspensionReturnTaskOfIntAsync(), () =>
        {
            Assert.That(instance.TestMethodThrowsAfterSuspensionReturnTaskOfIntAsync().GetAwaiter().GetResult(), Is.EqualTo(MockedResult));
        }).ReturnsAsync(MockedResult);

        exception = await Assert.ThrowsAsync<InvalidOperationException>(async () => await instance.TestMethodThrowsAfterSuspensionReturnTaskOfIntAsync());
        Assert.That(exception.Message, Is.EqualTo(TestRuntimeAsyncInstance.ExceptionMessage));
    }

    [Test]
    public async Task TestInstanceInterceptedFromNormalCallerTaskOfInt()
    {
        var instance = new TestRuntimeAsyncInstance();
        Assert.That(await instance.TestNormalCallerReturnTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncInstance.OriginalResult));

        Mock.Setup(() => instance.TestMethodReturnTaskOfIntAsync(), () =>
        {
            Assert.That(instance.TestNormalCallerReturnTaskOfIntAsync().GetAwaiter().GetResult(), Is.EqualTo(MockedResult));
        }).ReturnsAsync(MockedResult);

        Assert.That(await instance.TestNormalCallerReturnTaskOfIntAsync(), Is.EqualTo(TestRuntimeAsyncInstance.OriginalResult));
    }

    [Test]
    public async Task TestInstanceCallbackAsyncVoidMethod()
    {
        var instance = new TestRuntimeAsyncInstance();
        Mock.Setup(context => instance.TestVoidMethodAsync(context.It.IsAny<TaskCompletionSource>()), () =>
        {
            var completion = new TaskCompletionSource();
            instance.TestVoidMethodAsync(completion);

            Assert.That(completion.Task.IsCompleted, Is.True, "The callback must run instead of the original method, which suspends first");
        }).Callback<TaskCompletionSource>(source => source.SetResult());

        var originalCompletion = new TaskCompletionSource();
        instance.TestVoidMethodAsync(originalCompletion);
        await originalCompletion.Task;
    }

    [Test]
    public async Task TestInstanceCallbackAsyncVoidMethodWithParameter()
    {
        var instance = new TestRuntimeAsyncInstance();
        Mock.Setup(context => instance.TestVoidMethodWithParameterAsync(context.It.IsAny<int>(), context.It.IsAny<TaskCompletionSource<int>>()), () =>
        {
            var completion = new TaskCompletionSource<int>();
            instance.TestVoidMethodWithParameterAsync(4, completion);

            Assert.That(completion.Task.IsCompleted, Is.True, "The callback must run instead of the original method, which suspends first");
            Assert.That(completion.Task.GetAwaiter().GetResult(), Is.EqualTo(40));
        }).Callback<int, TaskCompletionSource<int>>((parameter, source) => source.SetResult(parameter * 10));

        var originalCompletion = new TaskCompletionSource<int>();
        instance.TestVoidMethodWithParameterAsync(4, originalCompletion);
        Assert.That(await originalCompletion.Task, Is.EqualTo(4));
    }
}
#endif
