
using Mono.Cecil;

if (args.Length < 2)
{
	Console.WriteLine(@"Usage: 
RemoveAssemblyAttributes <input.dll> <comma-separated list of attributes [full names] to remove>

Example:
RemoveAssemblyAttributes System.Data.SQLite.EF6.dll System.Security.AllowPartiallyTrustedCallersAttribute
");
	return 0;
}

if (string.IsNullOrWhiteSpace(args[0]))
	throw new ArgumentException("You did not specify a DLL to be patched.");

var inputDll = new FileInfo(args[0]);

var attributesToRemove = args[1].Split(',');

if (!inputDll.Exists)
	throw new ArgumentException($"Could not find DLL: {inputDll.FullName}");

var outputDll = new FileInfo($"{inputDll.FullName}.{Guid.NewGuid()}");

if (attributesToRemove.Length == 0)
	throw new ArgumentException("No attributes specified that should be removed");

var anyAttributeRemoved = false;

using (var assembly = AssemblyDefinition.ReadAssembly(
			inputDll.FullName,
			new ReaderParameters {ReadWrite = false}
		))
{
	var assemblyCustomAttributes = assembly.CustomAttributes;
	var matchedAttributes = assemblyCustomAttributes
		.Where(a => attributesToRemove.Contains(a.AttributeType.FullName))
		.ToArray();

	foreach (var matchedAttribute in matchedAttributes)
	{
		Console.WriteLine($"Removing [{matchedAttribute.AttributeType.FullName}] from {inputDll}");
		assemblyCustomAttributes.Remove(matchedAttribute);
		anyAttributeRemoved = true;
    }

	if (!anyAttributeRemoved)
	{
		Console.WriteLine("No changes. No attribute found that should be removed. Done.");
		return 0;
	}

	// 1. write to temp DLL
    assembly.Write(outputDll.FullName);
}

// 2. replace original DLL
outputDll.Replace(inputDll.FullName, null);

Console.WriteLine($"Patched assembly written back to {inputDll.FullName}");
return 0;