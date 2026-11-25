#if !NET5_0_OR_GREATER
namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Polyfill of the BCL attribute for targets that do not have it. The C# compiler recognizes it by name.
/// </summary>
[AttributeUsage(
    AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue,
    Inherited = false)]
internal sealed class NotNullAttribute : Attribute;
#endif
