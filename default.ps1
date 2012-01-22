$psake.context.Peek().config.framework = '4.0x86'

properties {
  $BuildConfiguration = 'Debug'
}

Task default -Depends Build, Test

Task Build {
  msbuild SimpleXml.sln /v:q /nologo /p:Configuration=$BuildConfiguration /t:Build
}

Task Test {
  .\packages\NUnit.2.5.10.11092\tools\nunit-console.exe /nologo /framework=net-4.0 .\SimpleXmlTests\bin\Debug\SimpleXmlTests.dll
}

Task NuPack {
  pushd SimpleXml
  nuget pack SimpleXml.csproj -Prop Configuration=Release
  popd
}
