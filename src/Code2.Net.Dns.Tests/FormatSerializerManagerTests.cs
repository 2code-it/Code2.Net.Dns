using Code2.Net.Dns.Internal;
using Code2.Net.Dns.Models.Formats;
using Code2.Net.Dns.WireSerializers.Formats;
using NSubstitute;
using System;

namespace Code2.Net.Dns.Tests;

public class FormatSerializerManagerTests
{
	[Fact]
	public void HasSerializer_When_SerializerForTypeCodeExists_Expect_True()
	{
		var reflection = Substitute.For<IReflectionUtility>();
		reflection.GetNonFrameworkTypes(Arg.Any<Func<Type, bool>>()).Returns([typeof(ARecordSerializer)]);
		reflection.Create(Arg.Any<Type>()).Returns(new ARecordSerializer());
		var manager = new FormatSerializerManager(reflection);

		bool result = manager.HasSerializer(Constants.RecordTypes.A);

		Assert.True(result);
	}

	[Fact]
	public void HasSerializer_When_NoSerializerForTypeCodeExists_Expect_False()
	{
		var reflection = Substitute.For<IReflectionUtility>();
		reflection.GetNonFrameworkTypes(Arg.Any<Func<Type, bool>>()).Returns([]);
		var manager = new FormatSerializerManager(reflection);

		bool result = manager.HasSerializer(Constants.RecordTypes.A);

		Assert.False(result);
	}

	[Fact]
	public void HasSerializer_When_SerializerForTypeExists_Expect_True()
	{
		var reflection = Substitute.For<IReflectionUtility>();
		reflection.GetNonFrameworkTypes(Arg.Any<Func<Type, bool>>()).Returns([typeof(ARecordSerializer)]);
		reflection.Create(Arg.Any<Type>()).Returns(new ARecordSerializer());
		var manager = new FormatSerializerManager(reflection);

		bool result = manager.HasSerializer(typeof(ARecord));

		Assert.True(result);
	}

	[Fact]
	public void HasSerializer_When_NoSerializerForTypeExists_Expect_False()
	{
		var reflection = Substitute.For<IReflectionUtility>();
		reflection.GetNonFrameworkTypes(Arg.Any<Func<Type, bool>>()).Returns([]);
		var manager = new FormatSerializerManager(reflection);

		bool result = manager.HasSerializer(typeof(ARecord));

		Assert.False(result);
	}

	[Theory]
	[InlineData(Constants.RecordTypes.A, typeof(ARecord), true)]
	[InlineData(Constants.RecordTypes.A, typeof(PtrRecord), false)]
	[InlineData(Constants.RecordTypes.PTR, typeof(PtrRecord), true)]
	public void IsMatch_When_SerializerTypeCodeToTypeArgIsMatch_Expect_TrueElseFalse(ushort typeCode, Type type, bool expected)
	{
		var reflection = Substitute.For<IReflectionUtility>();
		reflection.GetNonFrameworkTypes(Arg.Any<Func<Type, bool>>()).Returns([typeof(ARecordSerializer), typeof(PtrRecordSerializer)]);
		reflection.Create(Arg.Any<Type>()).Returns(x => Activator.CreateInstance((Type)x[0]));
		var manager = new FormatSerializerManager(reflection);

		bool result = manager.IsMatch(typeCode, type);

		Assert.Equal(expected, result);
	}

	[Fact]
	public void Deserialize_When_NoSerializerForTypeCode_Expect_ResultFail()
	{
		var reflection = Substitute.For<IReflectionUtility>();
		reflection.GetNonFrameworkTypes(Arg.Any<Func<Type, bool>>()).Returns([]);
		var reader = Substitute.For<IWireReader>();
		var manager = new FormatSerializerManager(reflection);

		var result = manager.Deserialize(Constants.RecordTypes.A, reader);

		Assert.False(result.IsSuccess);
	}

	[Fact]
	public void Deserialize_When_SerializerForTypeCode_Expect_ResultOk()
	{
		var reflection = new ReflectionUtility();
		var reader = Substitute.For<IWireReader>();
		var manager = new FormatSerializerManager(reflection);

		var result = manager.Deserialize(Constants.RecordTypes.TXT, reader);

		Assert.True(result.IsSuccess);
	}

	[Fact]
	public void Serialize_When_NoSerializerForTypeCode_Expect_ResultFail()
	{
		var reflection = Substitute.For<IReflectionUtility>();
		reflection.GetNonFrameworkTypes(Arg.Any<Func<Type, bool>>()).Returns([]);
		var writer = Substitute.For<IWireWriter>();
		var manager = new FormatSerializerManager(reflection);
		var record = new TxtRecord();

		var result = manager.Serialize(Constants.RecordTypes.A, writer, record);

		Assert.False(result.IsSuccess);
	}

	[Fact]
	public void Serialize_When_SerializerForTypeCode_Expect_ResultOk()
	{
		var reflection = new ReflectionUtility();
		var writer = Substitute.For<IWireWriter>();
		var manager = new FormatSerializerManager(reflection);
		var record = new TxtRecord();

		var result = manager.Serialize(Constants.RecordTypes.TXT, writer, record);

		Assert.True(result.IsSuccess);
	}
}
