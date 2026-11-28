using System;
using StaticMock.Helpers;
using StaticMock.HookBuilders;
using StaticMock.Hooks;

namespace StaticMock.Mocks.Throws;

internal class ThrowsMock(
    IHookBuilder hookBuilder,
    IHookManager hookManager) : IThrowsMock
{
    public IReturnable Throws(Type exceptionType, params object[]? constructorArgs) =>
        ThrowsInternal(exceptionType, constructorArgs);

    public IReturnable Throws<TException>() where TException : Exception, new()
    {
        var hook = hookBuilder.CreateThrowsHook(new TException());

        return hookManager.ApplyHook(hook);
    }

    private IReturnable ThrowsInternal(Type exceptionType, object[]? constructorArgs)
    {
        Guard.ThrowIfNull(exceptionType, nameof(exceptionType));

        if (Activator.CreateInstance(exceptionType, constructorArgs) is not Exception hookException)
        {
            throw new Exception($"{exceptionType.FullName} is not an Exception");
        }

        var hook = hookBuilder.CreateThrowsHook(hookException);

        return hookManager.ApplyHook(hook);
    }
}