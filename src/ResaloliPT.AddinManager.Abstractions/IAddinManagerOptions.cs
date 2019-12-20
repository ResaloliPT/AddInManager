using Microsoft.Extensions.Configuration;

namespace ResaloliPT.AddInManager.Abstractions
{
    public interface IAddInManagerOptions
    {
        /// <summary>
        /// Relative path to the plugins folder. Ex:. /plugins
        /// </summary>
        public string PluginsDirectory { get; set; }

        IAddInManagerOptions GetOptions(IConfiguration configuration);
    }
}
