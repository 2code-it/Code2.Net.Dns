using System;

namespace Code2.Net.Dns.Internal;

public interface IReflectionUtility
{
	object Create(Type type, params object?[]? args);
	Type[] GetNonFrameworkTypes(Func<Type, bool>? filter = null);
}