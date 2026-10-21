using System.Collections.Generic;

namespace Code2.Net.Dns.Models;

public class Message
{
	public MessageHeader Header { get; set; } = new();
	public List<Question> Questions { get; private set; } = [];
	public List<ResourceRecord> Answers { get; private set; } = [];

	public List<ResourceRecord> Authority { get; private set; } = [];
	public List<ResourceRecord> Additional { get; private set; } = [];
}
