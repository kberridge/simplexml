
Task NuPack {
  pushd SimpleXml
  nuget pack SimpleXml.csproj -Prop Configuration=Release
  popd
}
