namespace Code2.Net.Dns.Common;

public interface IResult
{
	public bool IsSuccess { get; }
	public string? Message { get; }

}
