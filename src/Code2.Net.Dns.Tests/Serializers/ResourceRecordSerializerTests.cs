namespace Code2.Net.Dns.Tests.Serializers;

public class ResourceRecordSerializerTests
{
	//[Fact]
	//public void Convert_When_RecordTypeA_Expect_RecordAFormat()
	//{
	//	var reader = Substitute.For<IWireReader>();
	//	var readUInt16Count = 0;
	//	uint address = 100;
	//	reader.TryReadName(out Arg.Any<string?>(), out Arg.Any<string?>()).Returns(true);
	//	reader.TryReadUInt16(out Arg.Any<ushort>()).Returns(x => { 
	//		if (readUInt16Count == 0) x[0] = Constants.RecordTypes.A;
	//		if (readUInt16Count == 1) x[0] = Constants.RecordClasses.IN;
	//		if (readUInt16Count == 2) x[0] = (ushort)100;
	//		readUInt16Count++;
	//		return true; 
	//	});
	//	reader.TryReadUInt32(out Arg.Any<uint>()).Returns(x => { 
	//		x[0] = 100U;
	//		return true; 
	//	});
	//	reader.ReadUInt32().Returns(address);
	//	var converter = new WireToResourceRecordConverter(new RecordOptions());

	//	var result = converter.Convert(reader);

	//	if (!result.IsSuccess)
	//	{
	//		Assert.Fail(result.Message);
	//		return;
	//	}

	//	var aRecord = (ARecord)result.Value.Data;

	//	Assert.Equal(address, aRecord.Address);
	//}

	//[Fact]
	//public void Convert_When_RecordTypeUnknown_Expect_RecordNullFormat()
	//{
	//	var reader = Substitute.For<IWireReader>();
	//	ushort length = 100;
	//	int uint16ReadCount = 0;
	//	byte firstByteRead = 99;
	//	reader.TryReadName(out Arg.Any<string?>(), out Arg.Any<string?>()).Returns(true);
	//	reader.TryReadUInt16(out Arg.Any<ushort>()).Returns(x => {
	//		x[0] = uint16ReadCount == 2? length: (ushort)0;
	//		uint16ReadCount++;
	//		return true;
	//	});
	//	reader.TryReadUInt32(out Arg.Any<uint>()).Returns(true);
	//	reader.When(x=> x.Read(Arg.Any<byte[]>())).Do(x => { var buffer = (byte[])x[0]; buffer[0] = firstByteRead; });
	//	var converter = new WireToResourceRecordConverter(new RecordOptions());

	//	var result = converter.Convert(reader);

	//	if (!result.IsSuccess)
	//	{
	//		Assert.Fail(result.Message);
	//		return;
	//	}

	//	var record = (NullRecord)result.Value.Data;

	//	Assert.Equal(firstByteRead, record.Value[0]);
	//	Assert.Equal(length, record.Value.Length);
	//}

	//[Fact]
	//public void Convert_When_Propertie_Expect_RecordPropertiesSet()
	//{
	//	var reader = Substitute.For<IWireReader>();
	//	var readUInt16Count = 0;
	//	ushort recordClass = Constants.RecordClasses.IN;
	//	ushort recordType = Constants.RecordTypes.MF;
	//	uint recordTtl = 100;
	//	ushort dataLength = 100;
	//	string name = "www.test-it.com";
	//	reader.TryReadName(out Arg.Any<string?>(), out Arg.Any<string?>()).Returns(x => { x[0] = name; return true; });
	//	reader.TryReadUInt16(out Arg.Any<ushort>()).Returns(x => {
	//		if (readUInt16Count == 0) x[0] = recordType;
	//		if (readUInt16Count == 1) x[0] = recordClass;
	//		if (readUInt16Count == 2) x[0] = dataLength;
	//		readUInt16Count++;
	//		return true;
	//	});
	//	reader.TryReadUInt32(out Arg.Any<uint>()).Returns(x => {
	//		x[0] = recordTtl;
	//		return true;
	//	});
	//	var converter = new WireToResourceRecordConverter(new RecordOptions());

	//	var result = converter.Convert(reader);

	//	if (!result.IsSuccess)
	//	{
	//		Assert.Fail(result.Message);
	//		return;
	//	}

	//	var record = result.Value;

	//	Assert.Equal(recordClass, record.Class);
	//	Assert.Equal(recordType, record.Type);
	//	Assert.Equal(dataLength, record.DataLength);
	//	Assert.Equal(recordTtl, record.TimeToLiveInSeconds);
	//	Assert.Equal(name, record.Name);
	//}
}
