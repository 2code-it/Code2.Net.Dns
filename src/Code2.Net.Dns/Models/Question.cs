namespace Code2.Net.Dns.Models;

public class Question
{
	public string Name { get; set; } = string.Empty;
	public ushort Type { get; set; }
	public ushort Class { get; set; }
}
