# AssemblyAttributeRemover

This nuget package automatically removes unwanted attributes from assemblies in your bin folder upon build.

## Motivation
For example, another nuget package might install a DLL with unwanted attributes. 
Instead of all the tedious steps of patching the DLL manually, creating a fork of the old nuget package, and maintaining it forever,
just use this nuget package.

# Configuration
By itself, adding the nuget package does nothing. 
You configure its actions in your `.csproj` file, specifying which attributes should be removed from which assemblies.
By using normal MsBuild conditions, you can fine-grain the scenario as much as you like.

# Example

```  
<Project Sdk="Microsoft.NET.Sdk">
...
  <ItemGroup>
    <PackageReference Include="AssemblyAttributeRemover" Version="0.0.1" />
    <AssemblyAttributeRemover
      Condition=" '$(TargetFramework)' == 'net472' "
      Include="System.Data.SQLite.EF6,System.Security.AllowPartiallyTrustedCallersAttribute" />
  </ItemGroup>
...
</Project>
```

Adding these lines to a `.csproj` file will have the following effect:

Upon build of .NET Framework 4.7.2, will look inside the `bin` folder for a DLL called `System.Data.SQLite.EF6.dll`, 
and - if found - will remove all attributes of type `AllowPartiallyTrustedCallersAttribute` from it, resaving the DLL in-place.
