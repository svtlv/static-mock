using System;

namespace StaticMock.Tests.Common.TestEntities;

public class TestInstanceWithoutDefaultEmptyConstructor
{
#pragma warning disable IDE0290 // The parameters stay unused on purpose: this entity only has no default constructor
    public TestInstanceWithoutDefaultEmptyConstructor(TestInstance param1, int param2, string param3)
    { }
#pragma warning restore IDE0290

    public object ObjectProperty { get; set; }

    public int TestMethodReturn1WithoutParameters()
    {
        return 1;
    }

    public void TestVoidMethodWithoutParametersThrowsException()
    {
        throw new Exception("Test exception");
    }
}