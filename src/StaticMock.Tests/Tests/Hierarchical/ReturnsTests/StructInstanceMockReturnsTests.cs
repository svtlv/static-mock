using NUnit.Framework;
using NUnit.Framework.Legacy;
using StaticMock.Entities;
using StaticMock.Tests.Common.TestEntities;

namespace StaticMock.Tests.Tests.Hierarchical.ReturnsTests;

[TestFixture]
public class StructInstanceMockReturnsTests
{
    [Test]
    public void TestStructInstanceMethodWithParameterReturns()
    {
        var testStruct = new TestStructInstance { Value = 1 };
        const int expectedResult = 2;

        Mock.Setup(typeof(TestStructInstance), nameof(TestStructInstance.GetValueWithParameter),
            new SetupProperties { Instance = testStruct }, () =>
            {
                ClassicAssert.AreEqual(expectedResult, testStruct.GetValueWithParameter(10));
            }).Returns(expectedResult);

        ClassicAssert.AreEqual(11, testStruct.GetValueWithParameter(10));
    }
}
