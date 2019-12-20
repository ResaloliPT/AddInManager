using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ResaloliPT.AddInManager.Abstractions
{
    public interface IAddIn
    {
        /// <summary>
        /// Stores the Pipeline Loading State. (Managed by the AddInProvider)
        /// </summary>
        public AddInState PipelineState { get; set; }

        /// <summary>
        /// Stores the Services Loading State. (Managed by the AddInProvider)
        /// </summary>
        public AddInState ServicesState { get; set; }

        /// <summary>
        /// Stores AddIn Id.
        /// </summary>
        public abstract string AddInId { get; }

        /// <summary>
        /// Stores the Pipeline Dependencies.
        /// </summary>
        public IEnumerable<string> AddInPipelineDependencies { get; }

        /// <summary>
        /// Stores the Services Dependencies.
        /// </summary>
        public IEnumerable<string> AddInServicesDependencies { get; }

        /// <summary>
        /// Configures the AddIn Service DI. (Optional Override)
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        virtual void ConfigureService(IServiceCollection services, IConfiguration configuration) { }

        /// <summary>
        /// Configures the AddIn Pipeline. (Optional Override)
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="env">The env.</param>
        /// <param name="configuration">The configuration.</param>
        virtual void Configure(IApplicationBuilder app, IHostEnvironment env, IConfiguration configuration) { }
    }
}
