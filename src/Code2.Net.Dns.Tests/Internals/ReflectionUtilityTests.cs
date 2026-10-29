using Code2.Net.Dns.Internal;
using Xunit;

namespace Code2.Net.Dns.Tests.Internals;

public class ReflectionUtilityTests
{
	[Fact]
	public void GetNonFrameworkTypes_When_FilterUsed_Expect_FilteredTypes()
	{
		var reflection = new ReflectionUtility();
		var typeSuffix = "Serializer";

		var types = reflection.GetNonFrameworkTypes(x => x.Name.EndsWith(typeSuffix));

		if (types.Length == 0)
		{
			Assert.Fail($"No types found ending with '{typeSuffix}'");
			return;
		}

		Assert.EndsWith(typeSuffix, types[0].Name);
	}

	private string GetTypeName<T>(T source)
		=> $"{typeof(T).Name} = {source}";
}
