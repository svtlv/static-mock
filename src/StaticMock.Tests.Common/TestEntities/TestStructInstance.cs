namespace StaticMock.Tests.Common.TestEntities;

public struct TestStructInstance
{
    public int Value { get; set; }

    public int GetValue()
    {
        return Value;
    }

    public int GetValueWithParameter(int parameter)
    {
        return Value + parameter;
    }
}
