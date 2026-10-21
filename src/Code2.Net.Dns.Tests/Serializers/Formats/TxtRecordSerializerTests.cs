using Code2.Net.Dns.Models.Formats;
using Code2.Net.Dns.WireSerializers.Formats;
using NSubstitute;
using System;
using System.Text;
using Xunit;

namespace Code2.Net.Dns.Tests.Serializers.Formats;

public class TxtRecordSerializerTests
{
	[Fact]
	public void Serialize_When_Serialized_Expect_WriterWriteAttributes()
	{
		string? receivedText = null;
		string text = "test text";
		var writer = Substitute.For<IWireWriter>();
		writer.When(x => x.Write(Arg.Any<byte[]>())).Do(x => { receivedText = Encoding.ASCII.GetString((byte[])x[0]); });
		var record = new TxtRecord() { Text = text };
		var serializer = new TxtRecordSerializer();

		var result = serializer.Serialize(writer, record);

		if (!result.IsSuccess)
		{
			Assert.Fail(result.Message);
			return;
		}

		Assert.Equal(text, receivedText);
	}

	[Theory]
	[InlineData("test test test txt")]
	[InlineData("")]
	public void Deserialize_When_AttributesPresent_Expect_RecordAttributesSet1(string text)
	{
		byte[] textBytes = Encoding.ASCII.GetBytes(text);
		var reader = Substitute.For<IWireReader>();
		reader.Length.Returns(text.Length);
		reader.CanRead(Arg.Any<int>()).Returns(true);
		reader.When(x => x.Read(Arg.Any<byte[]>())).Do(x => { Array.Copy(textBytes, (byte[])x[0], textBytes.Length); });
		var serializer = new TxtRecordSerializer();

		var result = serializer.Deserialize(reader);

		if (!result.IsSuccess)
		{
			Assert.Fail(result.Message);
			return;
		}

		Assert.Equal(text, result.Value.Text);
	}
}
