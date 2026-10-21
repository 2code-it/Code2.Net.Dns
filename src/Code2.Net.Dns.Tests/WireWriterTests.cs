using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Code2.Net.Dns.Tests;

public class WireWriterTests
{
	[Fact]
	public void Write_When_SourceLength_Expect_PositionAccordingLength()
	{
		var len = 10;
		var bytes = new byte[len];
		var writer = new WireWriter();

		writer.Write(bytes);

		Assert.Equal(len, writer.Position);
	}

	[Fact]
	public void TryWriteName_When_WriteMultipleNames_Expect_NameCompression()
	{
		string name1 = "www.test-it.com";
		string name2 = "www1.test-it.com";
		string name3 = "aa1.www.test-it.com";
		var totalLength = name1.Length + name2.Length + name3.Length;
		var writer = new WireWriter();

		writer.TryWriteName(name1, out var error1);
		writer.TryWriteName(name2, out var error2);
		writer.TryWriteName(name3, out var error3);

		var data = writer.ToArray();
		Assert.True(totalLength > writer.Position);
		Assert.Equal(0, writer.SuffixCache[name1]);
		Assert.True(IsPointer(data[name1.Length + 7])); // labels prefixed by length, name pointer or 0 terminated 
	}

	[Fact]
	public void TryWriteName_When_NameLength0_Expect_ErrorSet()
	{
		string name = string.Empty;
		var writer = new WireWriter();

		var result = writer.TryWriteName(name, out var error);

		Assert.False(result);
		Assert.NotNull(error);
	}

	[Fact]
	public void TryWriteName_When_NameLengthGreaterThan255_Expect_ErrorSet()
	{
		string name = string.Join('.', Enumerable.Range(0,25).Select(x => "aabbccddee"));
		var writer = new WireWriter();

		var result = writer.TryWriteName(name, out var error);

		Assert.False(result);
		Assert.NotNull(error);
	}

	[Fact]
	public void TryWriteName_When_LabelLengthGreaterThan63_Expect_ErrorSet()
	{
		string name = "a".PadRight(70, 'b');
		var writer = new WireWriter();

		var result = writer.TryWriteName(name, out var error);

		Assert.False(result);
		Assert.NotNull(error);
	}

	[Fact]
	public void WriteUInt16_When_Written_Expect_PositionAdvance2()
	{
		var writer = new WireWriter();

		writer.WriteUInt16(10);

		Assert.Equal(2, writer.Position);
	}

	[Fact]
	public void WriteUInt32_When_Written_Expect_PositionAdvance4()
	{
		var writer = new WireWriter();

		writer.WriteUInt32(10);

		Assert.Equal(4, writer.Position);
	}

	[Fact]
	public void WriteUInt128_When_Written_Expect_PositionAdvance16()
	{
		var writer = new WireWriter();

		writer.WriteUInt128(10);

		Assert.Equal(16, writer.Position);
	}

	private bool IsPointer(byte data)
		=> (data & 0b1100_0000) == 0b1100_0000;
}
