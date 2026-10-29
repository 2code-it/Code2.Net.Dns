using Code2.Net.Dns.Internal;
using Code2.Net.Dns.Models;
using Code2.Net.Dns.Models.Formats;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Code2.Net.Dns.Tests;

public class ExtensionsDnsTests
{
	[Fact]
	public void TrySetDataAs_When_SettingTypeWithSerializer_Expect_ResultTrue()
	{
		var record = new ResourceRecord();
		var aRecord = new ARecord();
		var addressBytes = new byte[] { 192, 168, 2, 2 };
		aRecord.Address = System.Buffers.Binary.BinaryPrimitives.ReadUInt32BigEndian(addressBytes);

		var result = record.TrySetDataAs(aRecord);

		Assert.True(result);
		Assert.Equal(addressBytes, record.Data);
	}

	[Fact]
	public void TrySetDataAs_When_SerializerForTypeNotFound_Expect_ResultFalse()
	{
		var reflection = Substitute.For<IReflectionUtility>();
		reflection.GetNonFrameworkTypes().Returns([]);
		var record = new ResourceRecord();
		var aRecord = new ARecord();		

		var result = record.TrySetDataAsInner(aRecord, reflection);

		Assert.False(result);
	}

	[Fact]
	public void TryGetDataAs_When_GettingTypeWithSerializer_Expect_ResultTrue()
	{
		var addressBytes = new byte[4];
		System.Buffers.Binary.BinaryPrimitives.WriteUInt32BigEndian(addressBytes, 2000U);
		uint address = System.Buffers.Binary.BinaryPrimitives.ReadUInt32BigEndian(addressBytes);
		var record = new ResourceRecord() { Data = addressBytes };

		var result = record.TryGetDataAs(out ARecord? aRecord);

		Assert.True(result);
		Assert.Equal(address, aRecord!.Address);
	}


	[Fact]
	public void TryGetDataAs_When_SerializerForTypeNotFound_Expect_ResultFalse()
	{
		var reflection = Substitute.For<IReflectionUtility>();
		reflection.GetNonFrameworkTypes().Returns([]);
		var record = new ResourceRecord();

		var result = record.TryGetDataAsInner(out ARecord? aRecord, reflection);

		Assert.False(result);
	}
}
