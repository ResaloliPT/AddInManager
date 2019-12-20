# AddInManager

.Net Standard 2.1 Library for Loading AddIns into your .NET Core 3.0  application

## Distribution

For the Manager Itself Refer to: [AddInManager NuGet](https://www.nuget.org/packages/ResaloliPT.AddInManager)

For the Abstractions Refer to: [Abstractions NuGet](https://www.nuget.org/packages/ResaloliPT.AddInManager.Abstractions)

## Usage

### Required

In Program.cs
`AddInProvider.Instance.ScanPlugins()`

### Optional

In Startup.cs
If you want to load Pipeline AddIns add in Configure(): `app.UseAddIns()`

If you want to load Service AddIns add in ConfigureServices(): `services.AddAddIns()`
