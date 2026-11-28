using System;
using System.Reflection;
using StaticMock.Entities.Enums;
using StaticMock.HookBuilders.Implementation;
using StaticMock.Hooks.Entities;

namespace StaticMock.HookBuilders.Factories;

internal class HookBuilderFactory(MethodInfo originalMethodInfo, HookSettings settings) : IHookBuilderFactory
{
    public IHookBuilder CreateHookBuilder() =>
        settings.HookManagerType switch
        {
            HookManagerType.MonoMod => originalMethodInfo.IsStatic
                ? new StaticHookBuilder(originalMethodInfo, settings.ItParameterExpressions)
                : new InstanceHookBuilder(originalMethodInfo, settings.ItParameterExpressions),
            _ => throw new ArgumentOutOfRangeException(nameof(settings.HookManagerType), settings.HookManagerType,
                $"{settings.HookManagerType} not exists in {nameof(HookManagerType)}")
        };
}