namespace Code2.Net.Dns.Models;

public class MessageHeader
{
	public ushort Id { get; set; }
	public bool IsResponse { get; set; }
	public byte OperationCode { get; set; }
	public bool IsAuthoritativeAnswer { get; set; }
	public bool IsTruncated { get; set; }
	public bool IsRecursionDesired { get; set; }
	public bool IsRecursionAvailable { get; set; }
	public byte ResponseCode { get; set; }
	public ushort QuestionCount { get; set; }
	public ushort AnswerRecordCount { get; set; }
	public ushort NameServerRecordCount { get; set; }
	public ushort AdditionalRecordCount { get; set; }
}