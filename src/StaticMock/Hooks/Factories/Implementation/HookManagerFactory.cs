using System;
using System.Reflection;
using StaticMock.Entities.Enums;
using StaticMock.Hooks.Entities;
using StaticMock.Hooks.Implementation;

namespace StaticMock.Hooks.Factories.Implementation;

internal class HookManagerFactory(MethodBase originalMethod, HookSettings settings) : IHookManagerFactory
{
    public IHookManager CreateHookManager() =>
        settings.HookManagerType switch
        {
            HookManagerType.MonoMod => new MonoModHookManager(new MonoModHookFactory(originalMethod)),
            _ => throw new ArgumentOutOfRangeException(nameof(settings.HookManagerType), settings.HookManagerType,
                $"{settings.HookManagerType} not exists in {nameof(HookManagerType)}")
        };
}