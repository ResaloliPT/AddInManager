using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ResaloliPT.AddInManager.Abstractions
{
    public interface IAddInProvider
    {
        /// <summary>
        /// Registers the pipeline plugins.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="env">The env.</param>
        /// <param name="configuration">The configuration.</param>
        void RegisterPipelineAddIns(IApplicationBuilder app, IHostEnvironment env, IConfiguration configuration);

        /// <summary>
        /// Registers the service plugins.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        void RegisterServiceAddIns(IServiceCollection services, IConfiguration configuration);

        /// <summary>
        /// Scans the plugins.
        /// </summary>
        /// <param name="addinManagerOptions">The addin manager options.</param>
        void ScanAddIns(IAddInManagerOptions addinManagerOptions);
    }
}
