@echo off
dotnet build src/Limbo.Umbraco.UrlPicker --configuration Release /t:rebuild /t:pack -p:PackageOutputPath=../../releases/nuget