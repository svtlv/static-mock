using System;
using StaticMock.HookBuilders;
using StaticMock.Hooks;
using StaticMock.Mocks.Throws;

namespace StaticMock.Mocks.Implementation.Hierarchical;

internal class Mock(
    IHookBuilder hookBuilder,
    IHookManager hookManager,
    Action action) : IMock
{
    protected IHookBuilder HookBuilder { get; } = hookBuilder;

    protected IHookManager HookManager { get; } = hookManager;

    protected Action ValidationAction { get; } = action;

    public IDisposable Throws(Type exceptionType)
    {
        ThrowsInternal(exceptionType, null);
        return new Disposable();
    }

    public IDisposable Throws(Type exceptionType, params object[] constructorArgs)
    {
        if (constructorArgs is not { Length: > 0 })
        {
            throw new Exception(
                "Constructor args can't be null or empty. Try throws exception with parameterless constructor or put constructor args you want to use");
        }

        ThrowsInternal(exceptionType, constructorArgs);
        return new Disposable();
    }

    public IDisposable Throws<TException>() where TException : Exception, new()
    {
        var throwService = new ThrowsMock(HookBuilder, HookManager);
        using (throwService.Throws<TException>())
        {
            ValidationAction();
        }

        return new Disposable();
    }

    public IDisposable Throws<TException>(object[] constructorArgs) where TException : Exception
    {
        if (constructorArgs is not { Length: > 0 })
        {
            throw new Exception(
                "Constructor args can't be null or empty. Try throws exception with parameterless constructor or put constructor args you want to use");
        }

        ThrowsInternal(typeof(TException), constructorArgs);
        return new Disposable();
    }

    private void ThrowsInternal(Type exceptionType, object[]? constructorArgs)
    {
        var throwService = new ThrowsMock(HookBuilder, HookManager);
        using (throwService.Throws(exceptionType, constructorArgs))
        {
            ValidationAction();
        }
    }
}