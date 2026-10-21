using System.Diagnostics.CodeAnalysis;

namespace Code2.Net.Dns.Common;

public interface IResult<T> : IResult
{
	T? Value { get; }
	[MemberNotNullWhen(true, nameof(Value))]
	new bool IsSuccess { get; }
}
