using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using StaticMock.Entities;
using StaticMock.Entities.Context;
using StaticMock.Helpers.Entities;
using StaticMock.HookBuilders.Factories;
using StaticMock.Hooks.Entities;
using StaticMock.Hooks.Factories;
using StaticMock.Hooks.Factories.Implementation;
using StaticMock.Hooks.Implementation;
using StaticMock.Mocks;
using StaticMock.Mocks.Implementation.Hierarchical;

namespace StaticMock.Helpers;

internal static class SetupMockHelper
{
    private const BindingFlags DefaultMethodBindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

    private static MockSetupProperties GetMockSetupProperties<TReturnValue>(Expression<Func<TReturnValue>> methodGetExpression)
    {
        MethodInfo? originalMethodInfo = null;
        var setupContext = new SetupContext();

        if (methodGetExpression.Body is MemberExpression memberExpression)
        {
            originalMethodInfo = memberExpression.Member is PropertyInfo propertyInfo ? propertyInfo.GetMethod : null;
        }

        if (methodGetExpression.Body is MethodCallExpression methodExpression)
        {
            originalMethodInfo = methodExpression.Method;
            setupContext.State.ItParameterExpressions.AddRange(
                methodExpression.Arguments.Select(x => new ItParameterExpression
                {
                    ParameterType = x.Type
                }));
        }

        if (originalMethodInfo is null)
        {
            throw new Exception("Get expression not contains method nor property to setup");
        }

        return new MockSetupProperties
        {
            OriginalMethodInfo = originalMethodInfo,
            SetupContextState = setupContext.State
        };
    }

    private static MockSetupProperties GetMockSetupProperties<TDelegate>(
        Expression<TDelegate> methodGetExpression)
    {
        MethodInfo? originalMethodInfo = null;
        var setupContext = new SetupContext();

        if (methodGetExpression.Body is MemberExpression memberExpression)
        {
            originalMethodInfo = memberExpression.Member is PropertyInfo propertyInfo ? propertyInfo.GetMethod : null;
        }

        if (methodGetExpression.Body is MethodCallExpression methodExpression)
        {
            originalMethodInfo = methodExpression.Method;
            foreach (var methodExpressionArgument in methodExpression.Arguments.OfType<MethodCallExpression>())
            {
                var argumentMethod = methodExpressionArgument.Method;
                if (argumentMethod.DeclaringType == typeof(It))
                {
                    argumentMethod.Invoke(
                        setupContext.It,
                        BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Public,
                        null,
                        [.. methodExpressionArgument.Arguments
                            .Cast<UnaryExpression>()
                            .Select(x => x.Operand)],
                        CultureInfo.InvariantCulture);
                }
            }
        }

        if (originalMethodInfo is null)
        {
            throw new Exception("Get expression not contains method nor property to setup");
        }

        return new MockSetupProperties
        {
            OriginalMethodInfo = originalMethodInfo,
            SetupContextState = setupContext.State
        };
    }

    public static IFuncMock SetupInternal(Type type, string methodName, Action? action = null, SetupProperties? setupProperties = null)
    {
        var originalMethodInfo = GetOriginalMethodInfo(type, methodName, setupProperties);

        if (originalMethodInfo.ReturnType == typeof(void))
        {
            throw new Exception($"Can't use some features of this setup for void return. To Setup void method us {nameof(Mock.SetupAction)} setup");
        }

        if (!originalMethodInfo.IsStatic && setupProperties?.Instance is null)
        {
            throw new Exception($"For testing instance methods/properties you should pass instance to {nameof(SetupProperties)}");
        }

        var context = new SetupContext();
        var hookSettings = new HookSettings
        {
            HookManagerType = Mock.GlobalSettings.HookManagerType,
            ItParameterExpressions = context.State.ItParameterExpressions
        };

        var hookBuilderFactory = new HookBuilderFactory(originalMethodInfo, hookSettings).CreateHookBuilder();
        var hookManagerFactory = new HookManagerFactory(originalMethodInfo, hookSettings).CreateHookManager();

        return action is null
            ? new Mocks.Implementation.Sequential.FuncMock(hookBuilderFactory, hookManagerFactory)
            : new FuncMock(hookBuilderFactory, hookManagerFactory, action);
    }

    public static IFuncMock SetupPropertyInternal(Type type, string propertyName, Action? action = null, SetupProperties? setupProperties = null)
    {
        var bindingFlags = setupProperties?.BindingFlags;
        var originalPropertyInfo = bindingFlags.HasValue ? type.GetProperty(propertyName, bindingFlags.Value) : type.GetProperty(propertyName);
        if (originalPropertyInfo is null)
        {
            throw new Exception($"Can't find property {propertyName} of type {type.FullName}");
        }

        var originalMethodInfo = originalPropertyInfo.GetMethod;
        Guard.ThrowIfNull(originalMethodInfo, nameof(originalMethodInfo));

        if (originalMethodInfo.ReturnType == typeof(void))
        {
            throw new Exception($"Can't use some features of this setup for void return. To Setup void method us {nameof(Mock.SetupAction)} setup");
        }

        if (!originalMethodInfo.IsStatic && setupProperties?.Instance is null)
        {
            throw new Exception($"For testing instance methods/properties you should pass instance to {nameof(SetupProperties)}");
        }

        var context = new SetupContext();
        var hookSettings = new HookSettings
        {
            HookManagerType = Mock.GlobalSettings.HookManagerType,
            ItParameterExpressions = context.State.ItParameterExpressions
        };

        var hookBuilderFactory = new HookBuilderFactory(originalMethodInfo, hookSettings).CreateHookBuilder();
        var hookManagerFactory =  new HookManagerFactory(originalMethodInfo, hookSettings).CreateHookManager();

        return action is null
            ? new Mocks.Implementation.Sequential.FuncMock(hookBuilderFactory, hookManagerFactory)
            : new FuncMock(hookBuilderFactory, hookManagerFactory, action);
    }

    public static IActionMock SetupVoidInternal(Type type, string methodName, Action? action = null, SetupProperties? setupProperties = null)
    {
        var originalMethodInfo = GetOriginalMethodInfo(type, methodName, setupProperties);

        if (!originalMethodInfo.IsStatic && setupProperties?.Instance is null)
        {
            throw new Exception($"For testing instance methods/properties you should pass instance to {nameof(SetupProperties)}");
        }

        var context = new SetupContext();
        var hookSettings = new HookSettings
        {
            HookManagerType = Mock.GlobalSettings.HookManagerType,
            ItParameterExpressions = context.State.ItParameterExpressions
        };

        var hookBuilderFactory = new HookBuilderFactory(originalMethodInfo, hookSettings).CreateHookBuilder();
        var hookManagerFactory =  new HookManagerFactory(originalMethodInfo, hookSettings).CreateHookManager();

        return action is null
            ? new Mocks.Implementation.Sequential.ActionMock(hookBuilderFactory, hookManagerFactory)
            : new ActionMock(hookBuilderFactory, hookManagerFactory, action);
    }

    public static void SetupDefaultInternal(Type type, string methodName, Action? action = null, SetupProperties? setupProperties = null)
    {
        var originalMethodInfo = GetOriginalMethodInfo(type, methodName, setupProperties);

        if (originalMethodInfo.ReturnType != typeof(void))
        {
            throw new Exception("Default setup supported only for void methods");
        }

        var context = new SetupContext();
        var hookSettings = new HookSettings
        {
            HookManagerType = Mock.GlobalSettings.HookManagerType,
            ItParameterExpressions = context.State.ItParameterExpressions
        };

        var hookBuilderFactory = new HookBuilderFactory(originalMethodInfo, hookSettings).CreateHookBuilder();
        var hookManagerFactory =  new HookManagerFactory(originalMethodInfo, hookSettings).CreateHookManager();

        IActionMock actionMock = action is null
            ? new Mocks.Implementation.Sequential.ActionMock(hookBuilderFactory, hookManagerFactory)
            : new ActionMock(hookBuilderFactory, hookManagerFactory, action);

        actionMock.Callback(() => { });
    }

    public static IFuncMock<TReturnValue> SetupInternal<TReturnValue>(Expression<Func<TReturnValue>> methodGetExpression, Action? action = null)
    {
        var mockSetupProperties = GetMockSetupProperties(methodGetExpression);
        var hookSettings = new HookSettings
        {
            HookManagerType = Mock.GlobalSettings.HookManagerType,
            ItParameterExpressions = mockSetupProperties.SetupContextState.ItParameterExpressions
        };

        var hookBuilderFactory = new HookBuilderFactory(mockSetupProperties.OriginalMethodInfo, hookSettings).CreateHookBuilder();
        var hookManagerFactory = new HookManagerFactory(mockSetupProperties.OriginalMethodInfo, hookSettings).CreateHookManager();

        return action is null
            ? new Mocks.Implementation.Sequential.FuncMock<TReturnValue>(hookBuilderFactory, hookManagerFactory)
            : new FuncMock<TReturnValue>(hookBuilderFactory, hookManagerFactory, action);
    }

    public static IAsyncFuncMock<TReturnValue> SetupInternal<TReturnValue>(Expression<Func<Task<TReturnValue>>> methodGetExpression, Action? action = null)
    {
        var mockSetupProperties = GetMockSetupProperties(methodGetExpression);
        var hookSettings = new HookSettings
        {
            HookManagerType = Mock.GlobalSettings.HookManagerType,
            ItParameterExpressions = mockSetupProperties.SetupContextState.ItParameterExpressions
        };

        var hookBuilderFactory = new HookBuilderFactory(mockSetupProperties.OriginalMethodInfo, hookSettings).CreateHookBuilder();
        var hookManagerFactory = new HookManagerFactory(mockSetupProperties.OriginalMethodInfo, hookSettings).CreateHookManager();

        return action is null
            ? new Mocks.Implementation.Sequential.AsyncFuncMock<TReturnValue>(hookBuilderFactory, hookManagerFactory)
            : new AsyncFuncMock<TReturnValue>(hookBuilderFactory, hookManagerFactory, action);
    }

    public static IFuncMock<TReturnValue> SetupInternal<TReturnValue>(Expression<Func<SetupContext, TReturnValue>> methodGetExpression, Action? action = null)
    {
        var mockSetupProperties = GetMockSetupProperties(methodGetExpression);
        var hookSettings = new HookSettings
        {
            HookManagerType = Mock.GlobalSettings.HookManagerType,
            ItParameterExpressions = mockSetupProperties.SetupContextState.ItParameterExpressions
        };

        var hookBuilderFactory = new HookBuilderFactory(mockSetupProperties.OriginalMethodInfo, hookSettings).CreateHookBuilder();
        var hookManagerFactory = new HookManagerFactory(mockSetupProperties.OriginalMethodInfo, hookSettings).CreateHookManager();

        return action is null
            ? new Mocks.Implementation.Sequential.FuncMock<TReturnValue>(hookBuilderFactory, hookManagerFactory)
            : new FuncMock<TReturnValue>(hookBuilderFactory, hookManagerFactory, action);
    }

    public static IAsyncFuncMock<TReturnValue> SetupInternal<TReturnValue>(Expression<Func<SetupContext, Task<TReturnValue>>> methodGetExpression, Action? action = null)
    {
        var mockSetupProperties = GetMockSetupProperties(methodGetExpression);
        var hookSettings = new HookSettings
        {
            HookManagerType = Mock.GlobalSettings.HookManagerType,
            ItParameterExpressions = mockSetupProperties.SetupContextState.ItParameterExpressions
        };

        var hookBuilderFactory = new HookBuilderFactory(mockSetupProperties.OriginalMethodInfo, hookSettings).CreateHookBuilder();
        var hookManagerFactory = new HookManagerFactory(mockSetupProperties.OriginalMethodInfo, hookSettings).CreateHookManager();

        return action is null
            ? new Mocks.Implementation.Sequential.AsyncFuncMock<TReturnValue>(hookBuilderFactory, hookManagerFactory)
            : new AsyncFuncMock<TReturnValue>(hookBuilderFactory, hookManagerFactory,  action);
    }

    public static IActionMock SetupInternal(Expression<Action> methodGetExpression, Action? action = null)
    {
        if (methodGetExpression.Body is not MethodCallExpression methodExpression)
        {
            throw new Exception("Get expression not contains method to setup");
        }

        var context = new SetupContext();
        var hookSettings = new HookSettings
        {
            HookManagerType = Mock.GlobalSettings.HookManagerType,
            ItParameterExpressions = context.State.ItParameterExpressions
        };

        var hookBuilderFactory = new HookBuilderFactory(methodExpression.Method, hookSettings).CreateHookBuilder();
        var hookManagerFactory = new HookManagerFactory(methodExpression.Method, hookSettings).CreateHookManager();

        return action is null
            ? new Mocks.Implementation.Sequential.ActionMock(hookBuilderFactory, hookManagerFactory)
            : new ActionMock(hookBuilderFactory, hookManagerFactory, action);
    }

    public static IActionMock SetupInternal(Expression<Action<SetupContext>> methodGetExpression, Action? action = null)
    {
        var mockSetupProperties = GetMockSetupProperties(methodGetExpression);
        var hookSettings = new HookSettings
        {
            HookManagerType = Mock.GlobalSettings.HookManagerType,
            ItParameterExpressions = mockSetupProperties.SetupContextState.ItParameterExpressions
        };

        var hookBuilderFactory = new HookBuilderFactory(mockSetupProperties.OriginalMethodInfo, hookSettings).CreateHookBuilder();
        var hookManagerFactory = new HookManagerFactory(mockSetupProperties.OriginalMethodInfo, hookSettings).CreateHookManager();

        return action is null
            ? new Mocks.Implementation.Sequential.ActionMock(hookBuilderFactory, hookManagerFactory)
            : new ActionMock(hookBuilderFactory, hookManagerFactory, action);
    }

    public static void SetupDefaultInternal(Expression<Action> methodGetExpression, Action? action = null)
    {
        if (methodGetExpression.Body is not MethodCallExpression methodExpression)
        {
            throw new Exception("Get expression not contains method to setup");
        }

        var originalMethodInfo = methodExpression.Method;
        if (originalMethodInfo.ReturnType != typeof(void))
        {
            throw new Exception("Default setup supported only for void methods");
        }

        var context = new SetupContext();
        var hookSettings = new HookSettings
        {
            HookManagerType = Mock.GlobalSettings.HookManagerType,
            ItParameterExpressions = context.State.ItParameterExpressions
        };

        var hookBuilderFactory = new HookBuilderFactory(originalMethodInfo, hookSettings).CreateHookBuilder();
        var hookManagerFactory = new HookManagerFactory(originalMethodInfo, hookSettings).CreateHookManager();

        IActionMock actionMock = action is null
            ? new Mocks.Implementation.Sequential.ActionMock(hookBuilderFactory, hookManagerFactory)
            : new ActionMock(hookBuilderFactory, hookManagerFactory, action);

        actionMock.Callback(() => { });
    }

    private static MethodInfo GetOriginalMethodInfo(Type type, string methodName, SetupProperties? setupProperties)
    {
        var originalMethodInfo = type.GetMethod(
            methodName,
            setupProperties?.BindingFlags ?? DefaultMethodBindingFlags,
            null,
            CallingConventions.Any,
            setupProperties?.MethodParametersTypes ?? [],
            null) ?? type.GetMethod(methodName);

        if (originalMethodInfo is null)
        {
            throw new Exception($"Can't find method {methodName} of type {type.FullName}");
        }

        if (!originalMethodInfo.IsGenericMethodDefinition)
        {
            return originalMethodInfo;
        }

        var genericTypes = setupProperties?.GenericTypes;

        Guard.ThrowIfNull(genericTypes, nameof(SetupProperties.GenericTypes));

        var genericArguments = originalMethodInfo.GetGenericArguments();
        if (genericTypes.Length != genericArguments.Length)
        {
            throw new Exception(
                $"Length and order of {nameof(SetupProperties.GenericTypes)} must be like count and order of generics in setup method");
        }

        originalMethodInfo = originalMethodInfo.MakeGenericMethod(genericTypes);

        return originalMethodInfo;
    }
}