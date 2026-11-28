using System;
using StaticMock.HookBuilders;
using StaticMock.Hooks;
using StaticMock.Mocks.Callback;

namespace StaticMock.Mocks.Implementation.Hierarchical;

internal class ActionMock(
    IHookBuilder hookBuilder,
    IHookManager hookManager,
    Action action)
    : Mock(hookBuilder, hookManager, action), IActionMock
{
    public IDisposable Callback(Action callback)
    {
        var callbackService = new CallbackMock(HookBuilder, HookManager);
        using (callbackService.Callback(callback))
        {
            ValidationAction();
        }

        return new Disposable();
    }

    public IDisposable Callback<TArg>(Action<TArg> callback)
    {
        var callbackService = new CallbackMock(HookBuilder, HookManager);
        using (callbackService.Callback(callback))
        {
            ValidationAction();
        }

        return new Disposable();
    }

    public IDisposable Callback<TArg1, TArg2>(Action<TArg1, TArg2> callback)
    {
        var callbackService = new CallbackMock(HookBuilder, HookManager);
        using (callbackService.Callback(callback))
        {
            ValidationAction();
        }

        return new Disposable();
    }

    public IDisposable Callback<TArg1, TArg2, TArg3>(Action<TArg1, TArg2, TArg3> callback)
    {
        var callbackService = new CallbackMock(HookBuilder, HookManager);
        using (callbackService.Callback(callback))
        {
            ValidationAction();
        }

        return new Disposable();
    }

    public IDisposable Callback<TArg1, TArg2, TArg3, TArg4>(Action<TArg1, TArg2, TArg3, TArg4> callback)
    {
        var callbackService = new CallbackMock(HookBuilder, HookManager);
        using (callbackService.Callback(callback))
        {
            ValidationAction();
        }

        return new Disposable();
    }

    public IDisposable Callback<TArg1, TArg2, TArg3, TArg4, TArg5>(Action<TArg1, TArg2, TArg3, TArg4, TArg5> callback)
    {
        var callbackService = new CallbackMock(HookBuilder, HookManager);
        using (callbackService.Callback(callback))
        {
            ValidationAction();
        }

        return new Disposable();
    }

    public IDisposable Callback<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> callback)
    {
        var callbackService = new CallbackMock(HookBuilder, HookManager);
        using (callbackService.Callback(callback))
        {
            ValidationAction();
        }

        return new Disposable();
    }

    public IDisposable Callback<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>(Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7> callback)
    {
        var callbackService = new CallbackMock(HookBuilder, HookManager);
        using (callbackService.Callback(callback))
        {
            ValidationAction();
        }

        return new Disposable();
    }

    public IDisposable Callback<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>(Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8> callback)
    {
        var callbackService = new CallbackMock(HookBuilder, HookManager);
        using (callbackService.Callback(callback))
        {
            ValidationAction();
        }

        return new Disposable();
    }

    public IDisposable Callback<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9>(Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9> callback)
    {
        var callbackService = new CallbackMock(HookBuilder, HookManager);
        using (callbackService.Callback(callback))
        {
            ValidationAction();
        }

        return new Disposable();
    }
}