namespace Code2.Net.Dns.Common;

public sealed class Result<T> : Result, IResult<T>
{
	public Result(bool isSuccess, string? message = null, T? value = default, IResult? parent = null) : base(isSuccess, message, parent)
	{
		Value = value;
	}

	public new bool IsSuccess => base.IsSuccess;

	public T? Value { get; private set; }

}
