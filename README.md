# AddinManager

.Net Standard 2.1 Library for Loading Addins into your application

## Distribution

For the Manager Itself Refer to: [AddinManager NuGet](https://www.nuget.org/packages/ResaloliPT.AddinManager)

For the Abstractions Refer to: [Abstractions NuGet](https://www.nuget.org/packages/ResaloliPT.AddinManager.Abstractions)

## Usage

### Required

In Program.cs
`AddinProvider.Instance.ScanPlugins()`

### Optional

In Startup.cs
If you want to load Pipeline Addins add in Configure(): `app.UseAddins()`

If you want to load Service Addins add in ConfigureServices(): `services.AddAddins()`
