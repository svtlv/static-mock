#if NET11_0_OR_GREATER
using System.Reflection;

namespace StaticMock.Tests.Tests.RuntimeAsync;

/// <summary>
/// Reflection helpers for the runtime-async tests. The marker <see cref="MethodImplAttributes.Async"/> shows only how
/// the compiler emitted a method. It does not prove that SMock can intercept the method.
/// </summary>
internal static class RuntimeAsyncMarker
{
    /// <summary>Methods with this name prefix are normal callers: they return a task but are not runtime-async methods.</summary>
    public const string NormalCallerPrefix = "TestNormalCaller";

    public static bool IsRuntimeAsync(MethodInfo method)
    {
        return (method.MethodImplementationFlags & MethodImplAttributes.Async) != 0;
    }

    public static bool IsTaskLike(Type returnType)
    {
        if (returnType == typeof(Task) || returnType == typeof(ValueTask))
        {
            return true;
        }

        return returnType.IsGenericType
            && (returnType.GetGenericTypeDefinition() == typeof(Task<>)
                || returnType.GetGenericTypeDefinition() == typeof(ValueTask<>));
    }

    /// <summary>Names of the methods that must carry the runtime-async marker: they are async and return a task type.</summary>
    public static string[] GetRuntimeAsyncMethodNames(Type type, BindingFlags bindingFlags)
    {
        return
        [
            .. type.GetMethods(bindingFlags | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(method => IsTaskLike(method.ReturnType) && !method.Name.StartsWith(NormalCallerPrefix, StringComparison.Ordinal))
                .Select(method => method.Name)
                .OrderBy(name => name, StringComparer.Ordinal)
        ];
    }
}
#endif
