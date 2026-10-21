using Code2.Net.Dns.Common;

namespace Code2.Net.Dns;

public interface IWireSerializer<T>
{
	IResult Serialize(IWireWriter writer, T value);
	IResult<T> Deserialize(IWireReader reader);
}
