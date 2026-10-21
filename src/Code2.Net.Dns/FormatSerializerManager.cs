using Code2.Net.Dns.Common;
using Code2.Net.Dns.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Code2.Net.Dns;

public class FormatSerializerManager : IFormatSerializerManager
{
	private readonly IReflectionUtility _reflection;
	private readonly Dictionary<ushort, FormatSerializerInfo> _formatSerializers;
	private record FormatSerializerInfo(Type ModelType, IFormatWireSerializer Instance);

	public FormatSerializerManager() : this(ReflectionUtility.Instance) { }
	internal FormatSerializerManager(IReflectionUtility reflection)
	{
		_reflection = reflection;
		_formatSerializers = GetFormatSerializers();
	}

	private static IFormatSerializerManager? _instance;
	public static IFormatSerializerManager Instance => _instance ?? (_instance = new FormatSerializerManager());

	public bool HasSerializer(ushort typeCode)
		=> _formatSerializers.ContainsKey(typeCode);

	public bool HasSerializer(Type formatType)
		=> _formatSerializers.Values.Any(x => x.ModelType == formatType);

	public bool IsMatch(ushort typeCode, Type formatType)
		=> _formatSerializers.TryGetValue(typeCode, out var info) && info.ModelType == formatType;

	public IResult<object> Deserialize(ushort typeCode, IWireReader reader)
	{
		if (!_formatSerializers.TryGetValue(typeCode, out var info)) return Result.Fail<object>($"Serializer not found for type {typeCode}");
		return (IResult<object>)_reflection.InvokePrivateGenericMethod(this, nameof(DeserializeInner), [info.ModelType], [info.Instance, reader])!;
	}

	public IResult Serialize(ushort typeCode, IWireWriter writer, object recordData)
	{
		if (!_formatSerializers.TryGetValue(typeCode, out var info)) return Result.Fail($"Serializer not found for type {typeCode}");
		return (IResult)_reflection.InvokePrivateGenericMethod(this, nameof(SerializeInner), [info.ModelType], [info.Instance, writer, recordData])!;
	}

	private IResult<object> DeserializeInner<T>(object serializer, IWireReader reader)
	{
		IResult<T> result = ((IWireSerializer<T>)serializer).Deserialize(reader);
		if (!result.IsSuccess) return Result.Fail<object>(result);
		return Result.Ok<object>(result.Value);
	}

	private IResult SerializeInner<T>(object serializer, IWireWriter writer, T record)
	{
		IResult result = ((IWireSerializer<T>)serializer).Serialize(writer, record);
		if (!result.IsSuccess) return result;
		return Result.Ok();
	}

	private Dictionary<ushort, FormatSerializerInfo> GetFormatSerializers()
	{
		var types = _reflection.GetNonFrameworkTypes(x => typeof(IFormatWireSerializer).IsAssignableFrom(x));
		return types.Select(GetFormatSerializerInfo)
				.ToDictionary(x => x.Instance.RecordTypeCode);
	}

	private FormatSerializerInfo GetFormatSerializerInfo(Type type)
	{
		var modelType = type.GetInterfaces().First(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IWireSerializer<>)).GetGenericArguments().First();
		return new FormatSerializerInfo(modelType, (IFormatWireSerializer)_reflection.Create(type));
	}
}
