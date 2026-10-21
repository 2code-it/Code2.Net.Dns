using System;
using System.Linq;
using System.Reflection;

namespace Code2.Net.Dns.Internal;

public class ReflectionUtility : IReflectionUtility
{
	public ReflectionUtility()
	{
		_nonFrameworkAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(x => !x.IsDynamic && !IsFrameworkAssembly(x)).ToArray();
		_nonFrameworkClasses = _nonFrameworkAssemblies.SelectMany(x => x.ExportedTypes).Where(x => x.IsClass).ToArray();
	}

	private readonly Assembly[] _nonFrameworkAssemblies;
	private readonly Type[] _nonFrameworkClasses;

	public static IReflectionUtility? _instance;
	public static IReflectionUtility Instance => _instance ?? (_instance = new ReflectionUtility());


	public object Create(Type type, params object?[]? args)
		=> Activator.CreateInstance(type, args) ?? throw new InvalidOperationException($"Can't create instance of type {type.Name}");

	public Type[] GetNonFrameworkTypes(Func<Type, bool>? filter = null)
		=> _nonFrameworkClasses.Where(x => filter is null || filter(x)).ToArray();

	public object? InvokePrivateGenericMethod(object instance, string methodName, Type[] genericArgumentTypes, object?[]? parameters)
	{
		MethodInfo? methodInfo = instance.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
		if (methodInfo is null) throw new InvalidOperationException($"Method '{methodName}' not found");
		if (!methodInfo.IsGenericMethod) throw new InvalidOperationException("Method is not generic");

		return methodInfo.MakeGenericMethod(genericArgumentTypes).Invoke(instance, parameters);
	}


	private static bool IsFrameworkAssembly(Assembly assembly)
		=> assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product == "Microsoft® .NET";
}
