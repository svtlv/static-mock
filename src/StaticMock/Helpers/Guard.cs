using System;
using System.Diagnostics.CodeAnalysis;

namespace StaticMock.Helpers;

internal static class Guard
{
    public static void ThrowIfNull([NotNull] object? argument, string paramName)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(argument, paramName);
#else
        if (argument is null)
        {
            throw new ArgumentNullException(paramName);
        }
#endif
    }
}
