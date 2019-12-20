using Microsoft.Extensions.Configuration;
using ResaloliPT.AddInManager.Abstractions;

namespace ResaloliPT.AddInManager.Extensions.Microsoft.DependencyInjection
{
    public sealed class AddInManagerOptions : IAddInManagerOptions
    {
        public string PluginsDirectory { get; set; }

        /// <summary>
        /// Creates a AddInManagerOptions instance with values collected from the application settings Ex:. appsettings.json
        /// </summary>
        /// <returns>The AddIn Manager Options from the app settings</returns>
        public IAddInManagerOptions GetOptions(IConfiguration configuration)
        {
            var options = new AddInManagerOptions();

            configuration.GetSection(nameof(AddInManagerOptions)).Bind(options);

            return options;
        }
    }
}
