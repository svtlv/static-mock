using System;
using System.Reflection;
using StaticMock.Hooks.Factories;

namespace StaticMock.Hooks.Implementation;

internal class MonoModHookManager(IHookFactory hookFactory) : IHookManager
{
    private IDisposable? _hook;

    public IReturnable ApplyHook(MethodInfo transpiler)
    {
        _hook = hookFactory.CreateHook(transpiler);
        return this;
    }

    public void Return()
    {
        _hook?.Dispose();
    }

    public void Dispose()
    {
        Return();
    }
}