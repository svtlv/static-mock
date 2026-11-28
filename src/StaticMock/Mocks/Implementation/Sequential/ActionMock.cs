using System;
using StaticMock.HookBuilders;
using StaticMock.Hooks;
using StaticMock.Mocks.Callback;

namespace StaticMock.Mocks.Implementation.Sequential;

internal class ActionMock(IHookBuilder hookBuilder, IHookManager hookManager) : Mock(hookBuilder, hookManager), IActionMock
{
    public IDisposable Callback(Action callback) =>
        new CallbackMock(HookBuilder, HookManager).Callback(callback);

    public IDisposable Callback<TArg>(Action<TArg> callback) =>
        new CallbackMock(HookBuilder, HookManager).Callback(callback);

    public IDisposable Callback<TArg1, TArg2>(Action<TArg1, TArg2> callback) =>
        new CallbackMock(HookBuilder, HookManager).Callback(callback);

    public IDisposable Callback<TArg1, TArg2, TArg3>(Action<TArg1, TArg2, TArg3> callback) =>
        new CallbackMock(HookBuilder, HookManager).Callback(callback);

    public IDisposable Callback<TArg1, TArg2, TArg3, TArg4>(Action<TArg1, TArg2, TArg3, TArg4> callback) =>
        new CallbackMock(HookBuilder, HookManager).Callback(callback);

    public IDisposable Callback<TArg1, TArg2, TArg3, TArg4, TArg5>(Action<TArg1, TArg2, TArg3, TArg4, TArg5> callback) =>
        new CallbackMock(HookBuilder, HookManager).Callback(callback);

    public IDisposable Callback<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> callback) =>
        new CallbackMock(HookBuilder, HookManager).Callback(callback);

    public IDisposable Callback<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>(Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7> callback) =>
        new CallbackMock(HookBuilder, HookManager).Callback(callback);

    public IDisposable Callback<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>(Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8> callback) =>
        new CallbackMock(HookBuilder, HookManager).Callback(callback);

    public IDisposable Callback<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9>(Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9> callback) =>
        new CallbackMock(HookBuilder, HookManager).Callback(callback);
}