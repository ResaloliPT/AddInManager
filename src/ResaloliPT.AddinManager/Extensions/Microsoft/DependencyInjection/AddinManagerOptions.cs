using Microsoft.Extensions.Configuration;
using ResaloliPT.AddinManager.Abstractions;

namespace ResaloliPT.AddinManager.Extensions.Microsoft.DependencyInjection
{
    public sealed class AddinManagerOptions : IAddinManagerOptions
    {
        public string PluginsDirectory { get; set; }

        /// <summary>
        /// Creates a AddinManagerOptions instance with values collected from the application settings Ex:. appsettings.json
        /// </summary>
        /// <returns>The Addin Manager Options from the app settings</returns>
        public IAddinManagerOptions GetOptions(IConfiguration configuration)
        {
            var options = new AddinManagerOptions();

            configuration.GetSection(nameof(AddinManagerOptions)).Bind(options);

            return options;
        }
    }
}
