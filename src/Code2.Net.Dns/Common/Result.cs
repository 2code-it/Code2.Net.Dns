namespace Code2.Net.Dns.Common;

public class Result : IResult
{
	public Result(bool isSuccess, string? message = null, IResult? parent = null)
	{
		_isSuccess = isSuccess;
		Message = parent is null ? message : $"{message} -> {parent.Message}";
	}
	private readonly bool _isSuccess;

	public bool IsSuccess => _isSuccess;
	public string? Message { get; private set; }

	public static IResult Ok() => new Result(true);
	public static IResult Fail(string? message) => new Result(false, message);
	public static IResult Fail(IResult parent) => new Result(false, parent: parent);
	public static IResult Fail(string? message = null, IResult? parent = null) => new Result(false, message, parent);

	public static IResult<T> Ok<T>(T value) => new Result<T>(true, value: value);
	public static IResult<T> Fail<T>(string? message) => new Result<T>(false, message: message);
	public static IResult<T> Fail<T>(IResult parent) => new Result<T>(false, parent: parent);
	public static IResult<T> Fail<T>(string? message = null, IResult? parent = null) => new Result<T>(false, message: message, parent: parent);
}
