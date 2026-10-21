using Code2.Net.Dns.Internal;

namespace Code2.Net.Dns.Tests.Internals;

public class ReflectionUtilityTests
{
	[Fact]
	public void InvokePrivateGenericMethod_When_Invoked_Expect_PrivateGenericMethodInvocation()
	{
		var reflection = new ReflectionUtility();
		var type = typeof(uint);
		var value = 10U;
		string expected = $"{type.Name} = {value}";

		string result = (string)reflection.InvokePrivateGenericMethod(this, nameof(GetTypeName), [type], [value])!;

		Assert.Equal(expected, result);
	}

	private string GetTypeName<T>(T source)
		=> $"{typeof(T).Name} = {source}";
}
