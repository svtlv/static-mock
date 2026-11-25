using NUnit.Framework;
using NUnit.Framework.Legacy;
using StaticMock.Entities;
using StaticMock.Tests.Common.TestEntities;

namespace StaticMock.Tests.Tests.Sequential.ReturnsTests;

[TestFixture]
public class StructInstanceMockReturnsTests
{
    [Test]
    public void TestStructInstanceMethodReturns()
    {
        var testStruct = new TestStructInstance { Value = 1 };
        ClassicAssert.AreEqual(1, testStruct.GetValue());
        const int expectedResult = 2;

        using (Mock.Setup(typeof(TestStructInstance), nameof(TestStructInstance.GetValue), new SetupProperties { Instance = testStruct })
                   .Returns(expectedResult))
        {
            ClassicAssert.AreEqual(expectedResult, testStruct.GetValue());
        }

        ClassicAssert.AreEqual(1, testStruct.GetValue());
    }

    [Test]
    public void TestStructInstanceMethodWithParameterReturns()
    {
        var testStruct = new TestStructInstance { Value = 1 };
        ClassicAssert.AreEqual(11, testStruct.GetValueWithParameter(10));
        const int expectedResult = 2;

        using var _ = Mock.Setup(typeof(TestStructInstance), nameof(TestStructInstance.GetValueWithParameter), new SetupProperties { Instance = testStruct })
            .Returns(expectedResult);

        ClassicAssert.AreEqual(expectedResult, testStruct.GetValueWithParameter(10));
    }
}
