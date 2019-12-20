using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ResaloliPT.AddInManager.Extensions.Microsoft.DependencyInjection
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseAddIns(this IApplicationBuilder applicationBuilder, IHostEnvironment env, IConfiguration configuration)
        {
            if(applicationBuilder is null)
            {
                throw new ArgumentNullException(nameof(applicationBuilder));
            }

            if(AddInProvider.Instance == null)
                throw new InvalidOperationException("You need to load the AddIn Provider to be able to register Pipeline AddIns.");

            AddInProvider.Instance.RegisterPipelineAddIns(applicationBuilder, env, configuration);

            return applicationBuilder;
        }
    }
}
