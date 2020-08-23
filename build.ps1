param
(
    $config = 'Release',
	$outputPath = ".\artifacts\"
)

If(!(test-path $outputPath))
{
	New-Item -ItemType Directory -Force -Path $outputPath
}

$outputPath = Resolve-Path $outputPath

$solution = Resolve-Path .\ndiff.sln
dotnet clean $solution
dotnet build $solution -c $config
dotnet test ./ndiff.tests/ndiff.tests.csproj --logger "trx;LogFileName=$outputpath/testreport.trx"
dotnet pack $solution -c $config -o $outputPath --include-symbols
