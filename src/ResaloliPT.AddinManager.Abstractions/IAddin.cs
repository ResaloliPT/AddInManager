using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ResaloliPT.AddinManager.Abstractions
{
    public interface IAddin
    {
        public AddinState PipelineState { get; set; }

        public AddinState ServicesState { get; set; }

        public string AddinId { get; set; }

        public IEnumerable<string> AddinPipelineDependencies { get; set; }

        public IEnumerable<string> AddinServicesDependencies { get; set; }

        virtual void ConfigureService(IServiceCollection services, IConfiguration configuration) { }

        virtual void Configure(IApplicationBuilder app, IHostEnvironment env, IConfiguration configuration) { }
    }
}
