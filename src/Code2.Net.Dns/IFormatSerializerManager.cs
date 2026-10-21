using Code2.Net.Dns.Common;
using System;

namespace Code2.Net.Dns;

public interface IFormatSerializerManager
{
	IResult<object> Deserialize(ushort typeCode, IWireReader reader);
	bool HasSerializer(ushort typeCode);
	bool HasSerializer(Type formatType);
	bool IsMatch(ushort typeCode, Type formatType);
	IResult Serialize(ushort typeCode, IWireWriter writer, object recordData);
}