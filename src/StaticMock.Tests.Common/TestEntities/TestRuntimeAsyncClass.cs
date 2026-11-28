#if NET11_0_OR_GREATER
using System;
using System.Threading.Tasks;

namespace StaticMock.Tests.Common.TestEntities;

/// <summary>
/// Static methods that the C# compiler emits as runtime-async methods (the project sets the compiler feature
/// <c>runtime-async=on</c> for <c>net11.0</c>). Every <c>Task</c>, <c>Task&lt;T&gt;</c>, <c>ValueTask</c> and
/// <c>ValueTask&lt;T&gt;</c> method really suspends: it awaits <see cref="Task.Yield"/> or a gate task that the caller
/// controls. The <c>async void</c> methods are not runtime-async methods and exist for compatibility tests only.
/// </summary>
public static class TestRuntimeAsyncClass
{
    /// <summary>The result that the <c>Task&lt;int&gt;</c> and <c>ValueTask&lt;int&gt;</c> methods return.</summary>
    public const int OriginalResult = 1;

    /// <summary>The message of every exception that the methods throw after a suspension.</summary>
    public const string ExceptionMessage = "Test runtime-async exception";

    public static async Task TestMethodReturnTaskAsync()
    {
        await Task.Yield();
    }

    public static async Task<int> TestMethodReturnTaskOfIntAsync()
    {
        await Task.Yield();
        return OriginalResult;
    }

    public static async ValueTask TestMethodReturnValueTaskAsync()
    {
        await Task.Yield();
    }

    public static async ValueTask<int> TestMethodReturnValueTaskOfIntAsync()
    {
        await Task.Yield();
        return OriginalResult;
    }

    public static async Task<int> TestMethodReturnTaskOfIntWithParameterAsync(int parameter)
    {
        await Task.Yield();
        return parameter;
    }

    public static async ValueTask<int> TestMethodReturnValueTaskOfIntWithParameterAsync(int parameter)
    {
        await Task.Yield();
        return parameter;
    }

    public static async Task TestMethodAwaitGateReturnTaskAsync(Task gate)
    {
        await gate;
    }

    public static async Task<int> TestMethodAwaitGateReturnTaskOfIntAsync(Task gate)
    {
        await gate;
        return OriginalResult;
    }

    public static async ValueTask TestMethodAwaitGateReturnValueTaskAsync(Task gate)
    {
        await gate;
    }

    public static async ValueTask<int> TestMethodAwaitGateReturnValueTaskOfIntAsync(Task gate)
    {
        await gate;
        return OriginalResult;
    }

    public static async Task TestMethodThrowsAfterSuspensionReturnTaskAsync()
    {
        await Task.Yield();
        throw new InvalidOperationException(ExceptionMessage);
    }

    public static async Task<int> TestMethodThrowsAfterSuspensionReturnTaskOfIntAsync()
    {
        await Task.Yield();
        throw new InvalidOperationException(ExceptionMessage);
    }

    public static async ValueTask TestMethodThrowsAfterSuspensionReturnValueTaskAsync()
    {
        await Task.Yield();
        throw new InvalidOperationException(ExceptionMessage);
    }

    public static async ValueTask<int> TestMethodThrowsAfterSuspensionReturnValueTaskOfIntAsync()
    {
        await Task.Yield();
        throw new InvalidOperationException(ExceptionMessage);
    }

    /// <summary>A runtime-async caller: it awaits <see cref="TestMethodReturnTaskOfIntAsync"/> and adds one.</summary>
    public static async Task<int> TestCallerAwaitsTaskOfIntAsync()
    {
        return await TestMethodReturnTaskOfIntAsync() + 1;
    }

    /// <summary>A runtime-async caller: it awaits <see cref="TestMethodReturnTaskAsync"/>.</summary>
    public static async Task TestCallerAwaitsTaskAsync()
    {
        await TestMethodReturnTaskAsync();
    }

    /// <summary>A runtime-async caller: it awaits <see cref="TestMethodReturnValueTaskAsync"/>.</summary>
    public static async ValueTask TestCallerAwaitsValueTaskAsync()
    {
        await TestMethodReturnValueTaskAsync();
    }

    /// <summary>A runtime-async caller: it awaits <see cref="TestMethodReturnValueTaskOfIntAsync"/> and adds one.</summary>
    public static async ValueTask<int> TestCallerAwaitsValueTaskOfIntAsync()
    {
        return await TestMethodReturnValueTaskOfIntAsync() + 1;
    }

    /// <summary>A normal caller (not a runtime-async method) that calls <see cref="TestMethodReturnTaskOfIntAsync"/>.</summary>
    public static Task<int> TestNormalCallerReturnTaskOfIntAsync()
    {
        return TestMethodReturnTaskOfIntAsync();
    }

    /// <summary>
    /// An <c>async void</c> method. It is not a runtime-async method. It sets <paramref name="completion"/> after a suspension.
    /// </summary>
    public static async void TestVoidMethodAsync(TaskCompletionSource completion)
    {
        await Task.Yield();
        completion.SetResult();
    }

    /// <summary>
    /// An <c>async void</c> method with a parameter. It is not a runtime-async method.
    /// It sets <paramref name="completion"/> to <paramref name="parameter"/> after a suspension.
    /// </summary>
    public static async void TestVoidMethodWithParameterAsync(int parameter, TaskCompletionSource<int> completion)
    {
        await Task.Yield();
        completion.SetResult(parameter);
    }
}
#endif
