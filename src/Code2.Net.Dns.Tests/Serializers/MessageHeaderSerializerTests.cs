using Code2.Net.Dns.Models;
using Code2.Net.Dns.WireSerializers;
using NSubstitute;

namespace Code2.Net.Dns.Tests.Serializers;

public class MessageHeaderSerializerTests
{

	[Fact]
	public void Serialize_When_HeaderValuesSet_Expected_MatchingWrites()
	{
		ushort flags1 = 0b1000_1111_1000_0011;
		ushort flags2 = 0;
		var header1 = new MessageHeader
		{
			Id = 222,
			IsResponse = true,
			OperationCode = 1,
			IsAuthoritativeAnswer = true,
			IsTruncated = true,
			IsRecursionDesired = true,
			IsRecursionAvailable = true,
			ResponseCode = 3,
			QuestionCount = 1,
			AnswerRecordCount = 2,
			NameServerRecordCount = 3,
			AdditionalRecordCount = 4,
			
		};
		int i = 0;
		var header2 = new MessageHeader();
		var writer = Substitute.For<IWireWriter>();
		writer.When(x => x.WriteUInt16(Arg.Any<ushort>())).Do(x =>
		{
			if (i == 0) header2.Id = (ushort)x[0];
			if (i == 1) flags2 = (ushort)x[0];
			if (i == 2) header2.QuestionCount = (ushort)x[0];
			if (i == 3) header2.AnswerRecordCount = (ushort)x[0];
			if (i == 4) header2.NameServerRecordCount = (ushort)x[0];
			if (i == 5) header2.AdditionalRecordCount = (ushort)x[0];
			i++;
		});
		var serializer = new MessageHeaderSerializer();

		serializer.Serialize(writer, header1);

		Assert.Equal(header1.Id, header2.Id);
		Assert.Equal(flags1, flags2);
		Assert.Equal(header1.QuestionCount, header2.QuestionCount);
		Assert.Equal(header1.AnswerRecordCount, header2.AnswerRecordCount);
		Assert.Equal(header1.NameServerRecordCount, header2.NameServerRecordCount);
		Assert.Equal(header1.AdditionalRecordCount, header2.AdditionalRecordCount);
	}

	[Fact]
	public void Deserialize_When_BytesToReadIsLessThan12_Expected_ResultFail()
	{
		var reader = Substitute.For<IWireReader>();
		reader.CanRead(Arg.Any<int>()).Returns(false);
		var header = new MessageHeader();
		var serializer = new MessageHeaderSerializer();

		var result = serializer.Deserialize(reader);

		Assert.False(result.IsSuccess);
	}

	[Fact]
	public void Deserialize_When_BytesSet_Expected_HeaderPropertiesSet()
	{
		ushort[] parts = [
			1234,
			0b1111_1110_0111_1111,
			1, 2, 3, 4
		];
		int i = 0;
		var reader = Substitute.For<IWireReader>();
		reader.CanRead(Arg.Any<int>()).Returns(true);
		reader.ReadUInt16().Returns(x => parts[i++]);

		var header = new MessageHeader();
		var serializer = new MessageHeaderSerializer();

		var result = serializer.Deserialize(reader);

		if (!result.IsSuccess)
		{
			Assert.Fail(result.Message);
			return;
		}

		Assert.Equal(parts[0], result.Value.Id);
		Assert.True(result.Value.IsResponse);
		Assert.Equal(0b0000_1111, result.Value.OperationCode);
		Assert.True(result.Value.IsAuthoritativeAnswer);
		Assert.True(result.Value.IsTruncated);
		Assert.False(result.Value.IsRecursionDesired);
		Assert.False(result.Value.IsRecursionAvailable);
		Assert.Equal(0b0000_1111, result.Value.ResponseCode);
		Assert.Equal(parts[2], result.Value.QuestionCount);
		Assert.Equal(parts[3], result.Value.AnswerRecordCount);
		Assert.Equal(parts[4], result.Value.NameServerRecordCount);
		Assert.Equal(parts[5], result.Value.AdditionalRecordCount);
	}
}