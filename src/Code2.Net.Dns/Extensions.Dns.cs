using Code2.Net.Dns.Internal;
using Code2.Net.Dns.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;

namespace Code2.Net.Dns;

public static class ExtensionDns
{
	private static Dictionary<Type, object>? _serializers;
	private static Lock _lock = new();
	public static bool TryGetDataAs<T>(this ResourceRecord record, [NotNullWhen(true)] out T? formatValue)
		=> TryGetDataAsInner(record, out formatValue, ReflectionUtility.Instance);

	public static bool TrySetDataAs<T>(this ResourceRecord record, T formatValue)
		=> TrySetDataAsInner(record, formatValue, ReflectionUtility.Instance);

	public static MessageHeader Clone(this MessageHeader source)
	{
		return new MessageHeader
		{
			Id = source.Id,
			IsResponse = source.IsResponse,
			OperationCode = source.OperationCode,
			IsAuthoritativeAnswer = source.IsAuthoritativeAnswer,
			IsTruncated = source.IsTruncated,
			IsRecursionDesired = source.IsRecursionDesired,
			IsRecursionAvailable = source.IsRecursionAvailable,
			ResponseCode = source.ResponseCode,
			QuestionCount = source.QuestionCount,
			AnswerRecordCount = source.AnswerRecordCount,
			NameServerRecordCount = source.NameServerRecordCount,
			AdditionalRecordCount = source.AdditionalRecordCount
		};
	}

	public static Question Clone(this Question source)
	{
		return new Question
		{
			Name = source.Name, 
			Class = source.Class,
			Type = source.Type
		};
	}

	public static ResourceRecord Clone(this ResourceRecord source)
	{
		return new ResourceRecord
		{
			Name = source.Name,
			Class = source.Class,
			Type = source.Type,
			TimeToLiveInSeconds = source.TimeToLiveInSeconds,
			DataLength = source.DataLength,
			Data = source.Data.ToArray()
		};
	}

	public static Message Clone(this Message source)
	{
		var message = new Message { Header = source.Header.Clone() };
		message.Questions.AddRange(source.Questions.Select(x => x.Clone()));
		message.Answers.AddRange(source.Answers.Select(x => x.Clone()));
		message.Authority.AddRange(source.Authority.Select(x => x.Clone()));
		message.Additional.AddRange(source.Additional.Select(x => x.Clone()));
		return message;
	}

	internal static bool TrySetDataAsInner<T>(this ResourceRecord record, T formatValue, IReflectionUtility reflection)
	{
		EnsureSerializers(reflection);
		if (!_serializers.TryGetValue(typeof(T), out var serializer)) return false;
		var writer = new WireWriter();
		var result = ((IWireSerializer<T>)serializer).Serialize(writer, formatValue);
		if (result.IsSuccess) record.Data = writer.ToArray();
		return result.IsSuccess;
	}

	internal static bool TryGetDataAsInner<T>(this ResourceRecord record, [NotNullWhen(true)] out T? formatValue, IReflectionUtility reflection)
	{
		EnsureSerializers(reflection);
		formatValue = default;
		if (!_serializers.TryGetValue(typeof(T), out var serializer)) return false;
		var reader = new WireReader(record.Data);
		var result = ((IWireSerializer<T>)serializer).Deserialize(reader);
		if (result.IsSuccess) formatValue = result.Value;
		return result.IsSuccess;
	}

	[MemberNotNull(nameof(_serializers))]
	private static void EnsureSerializers(IReflectionUtility reflection)
	{
		using var scope = _lock.EnterScope();
		if (_serializers is not null) return;
		var serializersAndType = reflection.GetNonFrameworkTypes().Select(x => GetSerializerAndType(x, reflection));
		_serializers = serializersAndType.Where(x => x is not null).ToDictionary(x => x!.Value.formatType, x => x!.Value.serializer);
	}

	private static (Type formatType, object serializer)? GetSerializerAndType(Type type, IReflectionUtility reflection)
	{
		if (!type.GetConstructors().Any(x => x.GetParameters().Length == 0)) return null;
		var serializerInterface = type.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IWireSerializer<>));
		if (serializerInterface is null) return null;
		var recordType = serializerInterface.GetGenericArguments()[0];
		return (recordType, reflection.Create(type));
	}
}
