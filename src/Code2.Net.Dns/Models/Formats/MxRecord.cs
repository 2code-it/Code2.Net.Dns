namespace Code2.Net.Dns.Models.Formats;

public class MxRecord
{
	public ushort Preference { get; set; }
	public string Exchange { get; set; } = string.Empty;
}
