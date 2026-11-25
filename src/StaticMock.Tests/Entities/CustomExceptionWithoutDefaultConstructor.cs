namespace StaticMock.Tests.Entities;

internal class CustomExceptionWithoutDefaultConstructor(string customMessage) : Exception(customMessage);