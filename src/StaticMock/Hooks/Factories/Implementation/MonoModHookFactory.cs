using System;
using System.Reflection;
using MonoMod.Core;
using StaticMock.Hooks.Entities;

namespace StaticMock.Hooks.Factories.Implementation;

internal class MonoModHookFactory : IHookFactory
{
    private const string TasksNamespacePrefix = "System.Threading.Tasks.";

    private readonly MethodBase _originalMethod;
    private readonly HookSettings _settings;

    public MonoModHookFactory(MethodBase originalMethod, HookSettings settings)
    {
        _originalMethod = originalMethod;
        _settings = settings;
    }

    public IDisposable? CreateHook(MethodInfo transpiler)
    {
        try
        {
            return DetourFactory.Current.CreateDetour(_originalMethod, transpiler);
        }
        catch (NotSupportedException exception) when (IsGenericTaskLikeMethodOnDotNet11(_originalMethod))
        {
            // Known limitation of MonoMod.Core 1.4.0-prerelease.3: on the .NET 11 runtime it cannot hook
            // a generic method instantiation that returns Task, Task<T>, ValueTask or ValueTask<T>.
            throw new NotSupportedException(
                $"SMock cannot mock the method '{_originalMethod.DeclaringType}.{_originalMethod.Name}' on .NET 11. " +
                "MonoMod.Core 1.4.0-prerelease.3 cannot hook generic methods that return Task, Task<T>, ValueTask or ValueTask<T> on .NET 11. " +
                "Mock a non-generic method instead, or run the test on an earlier runtime. See the MonoMod exception in InnerException.",
                exception);
        }
    }

    private static bool IsGenericTaskLikeMethodOnDotNet11(MethodBase method)
    {
        return Environment.Version.Major >= 11
            && method is MethodInfo { IsGenericMethod: true } methodInfo
            && IsTaskLikeType(methodInfo.ReturnType);
    }

    private static bool IsTaskLikeType(Type type)
    {
        var typeDefinition = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
        var fullName = typeDefinition.FullName;

        return fullName is TasksNamespacePrefix + "Task"
            or TasksNamespacePrefix + "Task`1"
            or TasksNamespacePrefix + "ValueTask"
            or TasksNamespacePrefix + "ValueTask`1";
    }
}
