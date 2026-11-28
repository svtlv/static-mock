using System;
using StaticMock.HookBuilders;
using StaticMock.Hooks;
using StaticMock.Mocks.Throws;

namespace StaticMock.Mocks.Implementation.Sequential;

internal class Mock(IHookBuilder hookBuilder, IHookManager hookManager) : IMock
{
    protected IHookBuilder HookBuilder { get; } = hookBuilder;

    protected IHookManager HookManager { get; } = hookManager;

    public IDisposable Throws(Type exceptionType) => ThrowsInternal(exceptionType, null);

    public IDisposable Throws(Type exceptionType, params object[] constructorArgs)
    {
        if (constructorArgs is not { Length: > 0 })
        {
            throw new Exception(
                "Constructor args can't be null or empty. Try throws exception with parameterless constructor or put constructor args you want to use");
        }

        return ThrowsInternal(exceptionType, constructorArgs);
    }

    public IDisposable Throws<TException>() where TException : Exception, new() =>
        new ThrowsMock(HookBuilder, HookManager).Throws<TException>();

    public IDisposable Throws<TException>(object[] constructorArgs) where TException : Exception
    {
        if (constructorArgs is not { Length: > 0 })
        {
            throw new Exception(
                "Constructor args can't be null or empty. Try throws exception with parameterless constructor or put constructor args you want to use");
        }

        return ThrowsInternal(typeof(TException), constructorArgs);
    }

    private IDisposable ThrowsInternal(Type exceptionType, object[]? constructorArgs) =>
        new ThrowsMock(HookBuilder, HookManager).Throws(exceptionType, constructorArgs);
}