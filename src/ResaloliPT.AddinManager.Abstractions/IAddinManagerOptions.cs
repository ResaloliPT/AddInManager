using Microsoft.Extensions.Configuration;

namespace ResaloliPT.AddinManager.Abstractions
{
    public interface IAddinManagerOptions
    {
        /// <summary>
        /// Relative path to the plugins folder. Ex:. /plugins
        /// </summary>
        public string PluginsDirectory { get; set; }

        IAddinManagerOptions GetOptions(IConfiguration configuration);
    }
}
