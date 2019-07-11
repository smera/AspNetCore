pushd C:\GitHub\AspNetCore\src\Components\Analyzers\src
dotnet build

if ($lastExitCode -ne 0)
{
	popd
	return
}

cp "C:\GitHub\AspNetCore\src\Components\Analyzers\src\bin\Debug\netstandard2.0\Microsoft.AspNetCore.Components.Analyzers.dll" "C:\Users\nimullen\.nuget\packages\microsoft.aspnetcore.components.analyzers\3.0.0-dev\analyzers\dotnet\cs\Microsoft.AspNetCore.Components.Analyzers.dll"

popd

pushd C:\Users\nimullen\source\repos\Preview7BlazorClientSide

dotnet build --no-incremental
popd