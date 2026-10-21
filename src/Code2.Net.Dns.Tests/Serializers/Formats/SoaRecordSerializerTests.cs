using Code2.Net.Dns.Models.Formats;
using Code2.Net.Dns.WireSerializers.Formats;
using NSubstitute;
using Xunit;

namespace Code2.Net.Dns.Tests.Serializers.Formats;

public class SoaRecordSerializerTests
{
	[Fact]
	public void Serialize_When_Serialized_Expect_WriterWriteAttributes()
	{
		var record1 = new SoaRecord
		{
			MName = "test-it.com",
			RName = "tt.test-it.com",
			Serial = 1234,
			Refresh = 60,
			Retry = 10,
			Expire = 120,
			Minimum = 60
		};
		var record2 = new SoaRecord();
		var intCount = 0;
		var stringCount = 0;
		var writer = Substitute.For<IWireWriter>();

		writer.TryWriteName(Arg.Any<string>(), out Arg.Any<string?>()).Returns(x =>
		{
			if (stringCount == 0) record2.MName = (string)x[0];
			if (stringCount == 1) record2.RName = (string)x[0];
			stringCount++;
			return true;
		});
		writer.When(x => x.WriteUInt32(Arg.Any<uint>())).Do(x =>
		{
			if (intCount == 0) record2.Serial = (uint)x[0];
			if (intCount == 1) record2.Refresh = (uint)x[0];
			if (intCount == 2) record2.Retry = (uint)x[0];
			if (intCount == 3) record2.Expire = (uint)x[0];
			if (intCount == 4) record2.Minimum = (uint)x[0];
			intCount++;
		});
		var serializer = new SoaRecordSerializer();

		var result = serializer.Serialize(writer, record1);

		if (!result.IsSuccess)
		{
			Assert.Fail(result.Message);
			return;
		}

		Assert.Equal(record1.MName, record2.MName);
		Assert.Equal(record1.RName, record2.RName);
		Assert.Equal(record1.Serial, record2.Serial);
		Assert.Equal(record1.Refresh, record2.Refresh);
		Assert.Equal(record1.Expire, record2.Expire);
		Assert.Equal(record1.Minimum, record2.Minimum);
	}

	[Fact]
	public void Deserialize_When_AttributesPresent_Expect_RecordAttributesSet()
	{
		var record1 = new SoaRecord
		{
			MName = "test-it.com",
			RName = "tt.test-it.com",
			Serial = 1234,
			Refresh = 60,
			Retry = 10,
			Expire = 120,
			Minimum = 60
		};
		var intCount = 0;
		var stringCount = 0;
		var reader = Substitute.For<IWireReader>();

		reader.TryReadName(out Arg.Any<string?>(), out Arg.Any<string?>()).Returns(x =>
		{
			if (stringCount == 0) x[0] = record1.MName;
			if (stringCount == 1) x[0] = record1.RName;
			stringCount++;
			return true;
		});
		reader.TryReadUInt32(out Arg.Any<uint>()).Returns(x =>
		{
			if (intCount == 0) x[0] = record1.Serial;
			if (intCount == 1) x[0] = record1.Refresh;
			if (intCount == 2) x[0] = record1.Retry;
			if (intCount == 3) x[0] = record1.Expire;
			if (intCount == 4) x[0] = record1.Minimum;
			intCount++;
			return true;
		});
		var serializer = new SoaRecordSerializer();

		var result = serializer.Deserialize(reader);

		if (!result.IsSuccess)
		{
			Assert.Fail(result.Message);
			return;
		}

		Assert.Equal(record1.MName, result.Value.MName);
		Assert.Equal(record1.RName, result.Value.RName);
		Assert.Equal(record1.Serial, result.Value.Serial);
		Assert.Equal(record1.Refresh, result.Value.Refresh);
		Assert.Equal(record1.Expire, result.Value.Expire);
		Assert.Equal(record1.Minimum, result.Value.Minimum);
	}

	[Fact]
	public void Deserialize_When_ReadFailed_Expect_Failure()
	{
		var reader = Substitute.For<IWireReader>();
		reader.TryReadName(out Arg.Any<string?>(), out Arg.Any<string?>()).Returns(false);
		var serializer = new SoaRecordSerializer();

		var result = serializer.Deserialize(reader);

		Assert.False(result.IsSuccess);
	}
}
