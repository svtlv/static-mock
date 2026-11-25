using System;
using System.Reflection;
using MonoMod.Core;
using StaticMock.Hooks.Entities;

namespace StaticMock.Hooks.Factories.Implementation;

internal class MonoModHookFactory : IHookFactory
{
    private readonly MethodBase _originalMethod;
    private readonly HookSettings _settings;

    public MonoModHookFactory(MethodBase originalMethod, HookSettings settings)
    {
        _originalMethod = originalMethod;
        _settings = settings;
    }

    public IDisposable? CreateHook(MethodInfo transpiler)
    {
        return DetourFactory.Current.CreateDetour(_originalMethod, transpiler);
    }
}
